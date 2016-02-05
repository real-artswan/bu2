using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace BaboNetwork
{
    public class MasterServer
    {
        string ip;
        int port;

        public MasterServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        private static void WriteDataCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReadDataCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.stream.Write(state.buffer, 0, bytesRead);

                    // Get the rest of the data.
                   // client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                     //   new AsyncCallback(ReadDataCallback), state);
                }
                else
                {
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
                receiveDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static int pollTimeout = 1000000;

        public void refreshGames()
        {
            try
            {
                Socket tcpSocket = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream, ProtocolType.Tcp);
                
                tcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                tcpSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                tcpSocket.ReceiveTimeout = 10000;
                tcpSocket.SendTimeout = 10000;

                tcpSocket.Connect(ip, port);

                /*tcpSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), tcpSocket);
                connectDone.WaitOne();

                if (!tcpSocket.Connected)
                {
                    Console.WriteLine("Can not connect to master server");
                    return;
                }
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = tcpSocket;

                // Begin receiving the data from the remote device.
                tcpSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadDataCallback), state);

                receiveDone.WaitOne();

                MemoryStream packet = state.stream;*/
                int readBytes = 0;
                byte[] buffer = new byte[3072];
                MemoryStream packet = new MemoryStream();
                while (tcpSocket.Poll(pollTimeout, SelectMode.SelectRead))
                {
                    readBytes = tcpSocket.Receive(buffer);
                    packet.Write(buffer, 0, readBytes);
                };

                if (packet.Length == 0)
                {
                    Console.WriteLine("No data retrieved from master server!");
                    return;
                }
                //parse server response
                packet.Position = 0;
                BinaryReader reader = new BinaryReader(packet);
                /*uint connID = */reader.ReadUInt32();
                /*byte udp = */reader.ReadByte();
                /*byte[] junk = */reader.ReadBytes(28);
                lpID = reader.ReadUInt32();

                //compile response packet
                packet.SetLength(0);
                BinaryWriter writer = new BinaryWriter(packet);

                writer.Write(Encoding.ASCII.GetBytes("RND1"));
                writer.Write(getLastPacketID());

                byte[] data = Encoding.ASCII.GetBytes("2.11" + '\0');

                ushort dataLen = (ushort)data.Length;
                writer.Write(dataLen);
                writer.Write((ushort)999);
                writer.Write(data);
                packet.Seek(8, SeekOrigin.Begin);
                writer.Write((byte)1);
                byte[] newPacket = packet.ToArray();
                //send it
                /*tcpSocket.BeginSend(newPacket, 0, newPacket.Length, 0,
                    new AsyncCallback(WriteDataCallback), tcpSocket);

                sendDone.WaitOne();
                //recieve games list
                state = new StateObject();
                state.workSocket = tcpSocket;

                // Begin receiving the data from the remote device.
                tcpSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadDataCallback), state);

                receiveDone.WaitOne();*/
                //send it
                tcpSocket.Send(newPacket);
                
                //recieve games list
                packet.SetLength(0);
                //read key
                try
                {
                    while (true)
                    {
                        readBytes = tcpSocket.Receive(buffer);
                        packet.Write(buffer, 0, readBytes);
                    };
                }
                catch (SocketException e) {
                    Console.WriteLine("err: {0}", e.ToString());
                }

                FileStream fs = new FileStream("data.dat", FileMode.Create);
                packet.WriteTo(fs);
                fs.Close();

                tcpSocket.Shutdown(SocketShutdown.Both);
                tcpSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }
        /*ConnectionConfig config = new ConnectionConfig();

			int myReiliableChannelId  = config.AddChannel(QosType.Reliable);
			HostTopology topology = new HostTopology(config, 10);
			int hostId = NetworkTransport.AddHost(topology, masterPort);
			byte error;
			int connectionId = NetworkTransport.Connect(hostId, masterIP, masterPort, 0, out error);

			int recHostId;
			byte[] recBuffer = new byte[1024]; 
			int bufferSize = 1024;
			int dataSize;
			NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out myReiliableChannelId, recBuffer, bufferSize, out dataSize, out error);
			switch (recData)
			{
				case NetworkEventType.Nothing:         //1
					break;
				case NetworkEventType.ConnectEvent:    //2
					break;
				case NetworkEventType.DataEvent:       //3
					break;
				case NetworkEventType.DisconnectEvent: //4
					break;
			}
			//NetworkTransport.Send(hostId, connectionId, myReiliableChannelId, buffer, bufferLength,  out error);
			NetworkTransport.Disconnect(hostId, connectionId, out error);

			stBV2list = new stBV2list(gameVersion);
			//send this packet*/

        private uint lpID = 0;

        static string GetMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        char[] getLastPacketID()
        {
            ++lpID;

            //char digest[33];
            string digest = lpID.ToString();

            //create md5 hash from current PendingID
            digest = GetMd5Hash(digest);

            char[] newID = new char[5];

            newID[0] = digest[1];
            newID[1] = digest[3];
            newID[2] = digest[5];
            newID[3] = digest[7];
            newID[4] = '\0';
            return newID;
        }

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // State object for receiving data from remote device.
        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public MemoryStream stream = new MemoryStream();
        }


        /*private void parseResponse()
        {
            int nread = 0; //nombre de byte qu'on a lu du recv()

            //tant quia des byte a lire
            while (nread < nbytes)
            {

                if (WaitingForKey)
                {
                    //key partial
                    if (nbytes - nread < KEY_SIZE)
                    {
                        //partial key, lets take what we can
                        memcpy(lastKey, buf + nread, nbytes - nread);

                        //let's check if what we partialy have is valid
                        int nb = nbytes - nread;
                        if (memcmp(lastKey, "RND1", nb > 4 ? 4 : nb))
                        {
                            //rnd key is corrupted, potential hacking
                            Disconnect();
                        }

                        bytesRemaining = bytesRemaining - (unsigned short)(nbytes - nread);
                        nread += nbytes - nread;
                        PartialKey = true;

                        return 0;
                    }
                    //rest of the key is in the packet
                    else
                    {
                        //we are terminating a key
                        if (PartialKey)
                        {
                            //lets complete that key
                            memcpy(lastKey + (KEY_SIZE - bytesRemaining), buf + nread, bytesRemaining);
                            nread += bytesRemaining;

                            bytesRemaining = TCP_HEADER_SIZE;
                            WaitingForKey = false;
                            WaitingForHeader = true;

                            PartialKey = false;
                        }
                        else
                        {
                            //we take the key from the buffer as its all available
                            memcpy(lastKey, buf + nread, bytesRemaining);
                            nread += bytesRemaining;
                            bytesRemaining = TCP_HEADER_SIZE;
                            WaitingForKey = false;

                        }

                        //Key is complete, lets analyze it
                        char key[5];
                        char pid[5];    //packet ID
                        memcpy(key, lastKey, sizeof(UINT4));
                        key[4] = '\0';
                        if (stricmp("RND1", key)) return 1; //RndLabs key is corrupted, potential hacker

                        memcpy(pid, lastKey + sizeof(UINT4), sizeof(UINT4));

                        if (GetPendingID(pid)) return 1;        //potential hacker
                        PendingID++;

                        //grab le nombre de packet a recevoir
                        memcpy(&NbPacket, lastKey + (KEY_SIZE - 1), sizeof(char));

                    }
                }

                if (WaitingForHeader)
                {
                    //on test si c un partial header ou complete
                    if (nbytes - nread < TCP_HEADER_SIZE)
                    {
                        //header partiel
                        memcpy(&lastHeader[0], &buf[nread], nbytes - nread);
                        bytesRemaining = bytesRemaining - (unsigned short)(nbytes - nread);
                        nread += nbytes - nread;
                        PartialHeader = true;

                        //fp = fopen("_sdebug.txt","a");
                        //fprintf(fp,"On attend un header partiel\n");
                        //fclose(fp);

                    }
                    else
                    {
                        //on va lire le header qui sen vient
                        if (PartialHeader)
                        {
                            //fp = fopen("_sdebug.txt","a");
                            //fprintf(fp,"On tente de reconstituer un header partiel\n");
                            //fclose(fp);

                            memcpy(&lastHeader[TCP_HEADER_SIZE - bytesRemaining], buf + nread, bytesRemaining);
                            memcpy(&lastPacket.Size, &lastHeader, 2);
                            memcpy(&lastPacket.TypeID, &(lastHeader[2]), 2);
                            nread += bytesRemaining;
                            PartialHeader = false;
                            WaitingForHeader = false;
                            bytesRemaining = lastPacket.Size;
                            if (lastPacket.Size > 0)    //si notre packet a du data
                            {
                                lastPacket.data = new char[lastPacket.Size];
                            }
                            else    //si notre packet a suelement un type id
                            {
                                lastPacket.data = 0;
                                WaitingForHeader = true;
                                bytesRemaining = TCP_HEADER_SIZE;

                                //on va l'ajouter au queue des packet disponible
                                AddReceivedPacket(new cPacket(lastPacket.data, lastPacket.Size, lastPacket.TypeID));

                                lastPacket.Size = 0;
                                delete[] lastPacket.data;
                                lastPacket.data = 0;

                                NbPacket--;

                                //all sub packets have been received in the master packet
                                if (!NbPacket)
                                {
                                    WaitingForKey = true;
                                    bytesRemaining = KEY_SIZE;
                                }
                            }
                        }
                        else
                        {
                            memcpy(&lastHeader[TCP_HEADER_SIZE - bytesRemaining], buf + nread, bytesRemaining);
                            memcpy(&lastPacket.Size, &lastHeader, 2);
                            memcpy(&lastPacket.TypeID, &(lastHeader[2]), 2);
                            nread += bytesRemaining;
                            WaitingForHeader = false;
                            bytesRemaining = lastPacket.Size;
                            if (lastPacket.Size > 0)    //si notre packet a du data
                            {
                                lastPacket.data = new char[lastPacket.Size];
                            }
                            else    //si notre packet a suelement un type id
                            {
                                lastPacket.data = 0;
                                WaitingForHeader = true;
                                bytesRemaining = TCP_HEADER_SIZE;

                                //on va l'ajouter au queue des packet disponible
                                AddReceivedPacket(new cPacket(lastPacket.data, lastPacket.Size, lastPacket.TypeID));

                                lastPacket.Size = 0;
                                delete[] lastPacket.data;
                                lastPacket.data = 0;

                                NbPacket--;

                                //all sub packets have been received in the master packet
                                if (!NbPacket)
                                {
                                    WaitingForKey = true;
                                    bytesRemaining = KEY_SIZE;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //est-ce que le reste du packet est dans le buffer ?
                    if (nread + bytesRemaining <= nbytes)
                    {
                        //on pogne le reste du data directement
                        memcpy(lastPacket.data + (lastPacket.Size - bytesRemaining), buf + nread, bytesRemaining);
                        WaitingForHeader = true;
                        nread += bytesRemaining;
                        bytesRemaining = TCP_HEADER_SIZE; //pour le prochain header

                        //ici le packet est complet

                        //on va l'ajouter au queue des packet disponible
                        AddReceivedPacket(new cPacket(lastPacket.data, lastPacket.Size, lastPacket.TypeID));

                        lastPacket.Size = 0;
                        delete[] lastPacket.data;
                        lastPacket.data = 0;

                        NbPacket--;

                        //all sub packets have been received in the master packet
                        if (!NbPacket)
                        {
                            WaitingForKey = true;
                            bytesRemaining = KEY_SIZE;
                        }
                    }
                    else
                    {
                        //on prend ce qu'on peut
                        memcpy(lastPacket.data, buf + nread, nbytes - nread);
                        bytesRemaining = bytesRemaining - (unsigned short)(nbytes - nread);
                        nread = nbytes;

                        //fp = fopen("_sdebug.txt","a");
                        //fprintf(fp,"On est en presence d'un packet data partiel\n");
                        //fclose(fp);
                    }
                }
            }

            return 0;
        }*/

    }
}
