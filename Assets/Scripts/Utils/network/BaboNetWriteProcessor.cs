using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace BaboNetwork
{
    class BaboNetWriteProcessor
    {
        public static ushort dataRate = 1400;
        private GetPacketCallback getPacketToSend;
        public UInt32 packetID;
        
        public BaboNetWriteProcessor(GetPacketCallback getPacketToSend)
        {
            this.getPacketToSend = getPacketToSend;
        }

        public byte[] prepareBuffer()
        {
            BaboRawPacket packet;
            if (!getPacketToSend(out packet))
                return new byte[0]; //nothing to do
            MemoryStream batchStream = new MemoryStream(); //for the batch data
            BinaryWriter batchWriter = new BinaryWriter(batchStream, Encoding.BigEndianUnicode);
            byte packetsInBatch = 0; //how much packets will be in batch

            //write key
            batchWriter.Write(Encoding.ASCII.GetBytes(BaboKey.RND_KEY));
            batchWriter.Write(getLastPacketID());
			//packets next
            ushort batchSize = BaboKey.KEY_SIZE;
            do
            {
                //Debug.Log((BaboPacketTypeID)packet.typeID);
                //write packet
                batchSize += (ushort)(BaboRawPacket.HEADER_SIZE + packet.dataSize);
                batchWriter.Write(packet.header);
                batchWriter.Write(packet.data);
                packetsInBatch++;
                //I do not follow dataRate strictly to simplify code.
                //Should not be the problem while dataRate is much bigger than the biggest packet size
            } while ((batchSize < dataRate) && getPacketToSend(out packet));
            //update packets count field
            batchWriter.BaseStream.Position = BaboKey.KEY_SIZE - 1;
            batchWriter.Write(packetsInBatch);

            return batchStream.ToArray();
        }

        private byte[] getLastPacketID()
        {
            packetID++;

            //create md5 hash
            string digest = BaboUtils.GetMd5Hash(packetID.ToString());

            byte[] newID = new byte[5];

            for (int i = 0; i < 4; i++)
            {
                newID[i] = (byte)digest[i * 2 + 1];
            }
            return newID;
        }
    }
}
