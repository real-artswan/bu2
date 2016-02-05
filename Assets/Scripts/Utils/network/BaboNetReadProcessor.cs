using System;
using System.Text;
using UnityEngine;

namespace BaboNetwork
{
    class BaboNetReadProcessor
    {
        private AddPacketCallback addReceivedPacket;
        public UInt32 packetID;

		public BaboNetReadProcessor(AddPacketCallback addReceivedPacket)
        {
			this.addReceivedPacket = addReceivedPacket;
        }

        class ParsingStateObject
        {
            public enum ParsingState
            {
                KEY, HEADER, DATA
            }
            public BaboKey key = new BaboKey();
            public BaboRawPacket packet = null;
            public ParsingState state = ParsingState.KEY;
            public int lastIndex = 0;

            public byte packetsBeforeNextKey = 0;
        }

        private ParsingStateObject stateObject = new ParsingStateObject();

        public bool parseBuffer(int receivedBytes, byte[] buf)
        {
            int currentByteInd = 0; //place where we are in the buffer
            int readBytes = 0; //to use inside cases (C# specific); how many bytes we need to finish current state

            while (currentByteInd < receivedBytes)
            {
                switch (stateObject.state)
                {
                    case ParsingStateObject.ParsingState.KEY:
                        readBytes = BaboKey.KEY_SIZE - stateObject.lastIndex;
                        if (receivedBytes - currentByteInd > readBytes) //can finish key here
                        {
                            Array.Copy(buf, currentByteInd, stateObject.key.value,
                                stateObject.lastIndex, readBytes);
                            //check key
                            if (Encoding.ASCII.GetString(stateObject.key.value, 0, 4) != BaboKey.RND_KEY)
                                return false;
                            //check id
                            byte[] pid = new byte[4];
                            Array.Copy(stateObject.key.value, 4, pid, 0, 4);
                            checkPendingID(pid);
                            //get count of packets before next key == last byte of key
                            stateObject.packetsBeforeNextKey = stateObject.key.value[BaboKey.KEY_SIZE - 1];
                            
                            if (stateObject.packetsBeforeNextKey > 0) //next will be header
                                stateObject.state = ParsingStateObject.ParsingState.HEADER;
                            stateObject.lastIndex = 0;

                            currentByteInd += readBytes;
                        }
                        else //partial key, no buffer more
                        {
                            Array.Copy(buf, currentByteInd, stateObject.key.value,
                                stateObject.lastIndex, receivedBytes - currentByteInd);
                            stateObject.lastIndex += receivedBytes - currentByteInd;

                            currentByteInd = receivedBytes;
                        }
                        break;
                    case ParsingStateObject.ParsingState.HEADER:
                        if (stateObject.packet == null)
                            stateObject.packet = new BaboRawPacket(); //start header here

                        readBytes = BaboRawPacket.HEADER_SIZE - stateObject.lastIndex;
                        if (receivedBytes - currentByteInd > readBytes) //can complete header here
                        {
                            Array.Copy(buf, currentByteInd, stateObject.packet.header, stateObject.lastIndex, readBytes);

                            //instantiate data
                            stateObject.packet.data = new byte[stateObject.packet.dataSize];

                            //next will be data
                            stateObject.state = ParsingStateObject.ParsingState.DATA;
                            stateObject.lastIndex = 0;

                            currentByteInd += readBytes;
                        }
                        else //partial header, no buffer more
                        {
                            //append unfinished header
                            Array.Copy(buf, currentByteInd, stateObject.packet.header, 
                                stateObject.lastIndex, receivedBytes - currentByteInd);
                            stateObject.lastIndex += receivedBytes - currentByteInd;

                            currentByteInd = receivedBytes;
                        }
                        break;
                    case ParsingStateObject.ParsingState.DATA:
                        readBytes = stateObject.packet.dataSize - stateObject.lastIndex;
                        if (receivedBytes - currentByteInd > readBytes) //finish packet data here
                        {
                            Array.Copy(buf, currentByteInd, stateObject.packet.data, stateObject.lastIndex, readBytes);

                            //complete packet
                            addReceivedPacket(stateObject.packet);
                            stateObject.packetsBeforeNextKey--;
                            //reset state
                            if (stateObject.packetsBeforeNextKey == 0)
                                //all packets in batch completed, next batch will run from scratch
                                stateObject = new ParsingStateObject();
                            else {
                                //next will be header
                                stateObject.state = ParsingStateObject.ParsingState.HEADER;
                                stateObject.lastIndex = 0;
                                stateObject.packet = null;
                            }
                            currentByteInd += readBytes;
                        }
                        else //partial data, no buffer more
                        {
                            //append unfinished data
                            Array.Copy(buf, currentByteInd, stateObject.packet.data, 
                                stateObject.lastIndex, receivedBytes - currentByteInd);
                            stateObject.lastIndex += receivedBytes - currentByteInd;

                            currentByteInd = receivedBytes;
                        }
                        break;
                }
            }
            return true;
        }

        private bool checkPendingID(byte[] pid)
        {
            packetID++;
            //create md5 hash from current PendingID
            string digest = BaboUtils.GetMd5Hash(packetID.ToString());

            //check hash
            for (int i = 0; i < 4; i++)
            {
                if (pid[i] != digest[i * 2 + 1])
                    return false;
            }
            return true;
        }
    }
}
