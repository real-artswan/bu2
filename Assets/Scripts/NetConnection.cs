using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using BaboNetwork;
using UnityEngine;
using UnityEngine.UI;

public class BaboRawPacket
{
    public static ushort HEADER_SIZE = 4;

    public byte[] header = new byte[4]; //(UInt16 Size, UInt16 TypeID)
    public byte[] data = null;     //un array dynamique qui tien le data a envoyer

    public UInt16 dataSize
    {
        get { return BitConverter.ToUInt16(header, 0); }
    }

    public UInt16 typeID
    {
        get { return BitConverter.ToUInt16(header, 2); }
    }

    public object packetToStruct()
    {
        var type = Type.GetType(typeof(BaboPacketTypeID).Namespace + "." + ((BaboPacketTypeID)typeID).ToString().ToLower(),
            throwOnError: false);

        if (type == null)
        {
            return null;
            //throw new InvalidOperationException(dtoSelection.ToString() + " is not a known dto type");
        }
        object obj;
        //int size = Marshal.SizeOf(type);
        int size = dataSize;
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(data, 0, ptr, size);

            obj = Marshal.PtrToStructure(ptr, type);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return obj;
    }

    public BaboRawPacket() { }

    public BaboRawPacket(object dataStruct)
    {
        string typeIDName = dataStruct.GetType().Name.ToUpper();

        BaboPacketTypeID id = (BaboPacketTypeID)Enum.Parse(typeof(BaboPacketTypeID), typeIDName);

        UInt16 size = (UInt16)Marshal.SizeOf(dataStruct);
        data = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(dataStruct, ptr, true);
            Marshal.Copy(ptr, data, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        //fill the header
        Array.Copy(BitConverter.GetBytes(size), header, 2);
        Array.Copy(BitConverter.GetBytes((UInt16)id), 0, header, 2, 2);
        /*header[0] = (byte)(size >> 8);
        header[1] = (byte)(size << 8);
        UInt16 typeIDValue = (UInt16)id;
        header[2] = (byte)(typeIDValue >> 8);
        header[3] = (byte)(typeIDValue << 8);*/
    }
}

public delegate void AddPacketCallback(BaboRawPacket packet);
public delegate bool GetPacketCallback(out BaboRawPacket packet);

public class NetConnection : MonoBehaviour
{
    public Text ip;
    public Text port;
    public GameState gameState;

    internal bool connected = false;

    public Queue<BaboRawPacket> recievedPackets = new Queue<BaboRawPacket>();
    public Queue<BaboRawPacket> packetsToSend = new Queue<BaboRawPacket>();

    private BaboNetReadProcessor readBufferProcessor;
    private BaboNetWriteProcessor writeBufferProcessor;
    private TcpClient client;

    private BaboNetPacketProcessor packetsProcessor;
    void Start()
    {
        //prepare buffer processors
        readBufferProcessor = new BaboNetReadProcessor((p) =>
        {
            /*if (p.typeID == 106)
                Debug.Log(DateTime.Now.ToString() + " Q<-PING");*/
            recievedPackets.Enqueue(p);
        });
        writeBufferProcessor = new BaboNetWriteProcessor(
            (out BaboRawPacket p) =>
            {
                bool res = packetsToSend.Count > 0;
                p = null;
                if (res)
                {
                    p = packetsToSend.Dequeue();
                    /*if (p.typeID == 1)
                        Debug.Log(DateTime.Now.ToString() + " Q->PONG");*/
                }
                return res;
            });
        packetsProcessor = new BaboNetPacketProcessor(gameState, p => { packetsToSend.Enqueue(p); });
    }

    private IEnumerator readLoop()
    {
        while (connected)
        {
            while (recievedPackets.Count > 0)
            {
                BaboRawPacket packet = recievedPackets.Dequeue();
                packetsProcessor.processPacket(packet); //update game state
                                                        /*if (packet.typeID == 106)
                                                            Debug.Log(DateTime.Now.ToString() + " <PING> " + connection.recievedPackets.Count.ToString());*/
                //yield return new WaitForSeconds(0.001f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator writeLoop()
    {
        while (connected)
        {
            while (packetsToSend.Count > 0)
            {
                while ((writeResult != null) && (!writeResult.IsCompleted)) //wait while writing prev buffer
                {
                    yield return new WaitForSeconds(0.001f);
                }
                byte[] buffer = writeBufferProcessor.prepareBuffer();
                try
                {
                    //Debug.Log("===>" + DateTime.Now.ToString());
                    writeResult = netStream.BeginWrite(buffer, 0, buffer.Length, null, null);
                }
                catch (Exception e)
                {
                    Debug.Log("net write: " + e.ToString());
                    shutdownError = true;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private NetworkStream netStream;
    private bool shutdownError = false;
    private IAsyncResult writeResult = null;

    void Update()
    {
        if (shutdownError)
        {
            Debug.Log("disconnected shutdown");
            disconnect();
            return;
        }
    }

    private bool prepareConnection()
    {
        try
        {
            client = new TcpClient(AddressFamily.InterNetwork);
            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ReceiveTimeout = 100000;
            client.SendTimeout = 100000;
            //client.SendBufferSize = BaboNetWriteProcessor.dataRate;
            //client.ReceiveBufferSize = 3072;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("pr_con: " + e.ToString());
            return false;
        }
    }

    public void startConnection()
    {
        shutdownError = false;
        if (connected)
            disconnect();
        if (!prepareConnection())
            return;
        try
        {
            //try to connect
            client.Connect(ip.text, int.Parse(port.text));

            //We are connected successfully.
            netStream = client.GetStream();

            //now we need 37 bytes from server where we get packetID
            recieveConnInfo();

            byte[] readBuffer = new byte[512];

            //Now we are connected start async read operation in loop
            netStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, readBuffer);

            //StartCoroutine(doSend()); //send portion of packets from packetsToSend queue

            connected = true;
            gameState.startGame();

            StartCoroutine(writeLoop());
            StartCoroutine(readLoop());
        }
        catch (Exception e)
        {
            Debug.Log("st_con: " + e.ToString());
            shutdownError = true;
        }
    }

    private bool recieveConnInfo()
    {
        int needBytes = 37;
        byte[] buffer = new byte[needBytes];
        needBytes -= netStream.Read(buffer, 0, buffer.Length);
        if (needBytes > 0)
            return false;
        //UInt32 connID = BitConverter.ToUInt32(buffer, 0);
        UInt32 packetID = BitConverter.ToUInt32(buffer, sizeof(UInt32) + sizeof(byte) + sizeof(byte) * 28);
        writeBufferProcessor.packetID = packetID;
        readBufferProcessor.packetID = packetID + 1;
        return true;
    }

    private void ReadCallback(IAsyncResult result)
    {
        try
        {
            byte[] buffer = result.AsyncState as byte[];
            int readBytes = netStream.EndRead(result);
            if (!readBufferProcessor.parseBuffer(readBytes, buffer))
            {
                Debug.Log("Fail to parse buffer");
                shutdownError = true;
                return;
            }
            //Start reading from the network again.
            netStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }
        catch (Exception e)
        {
            Debug.Log("read: " + e.ToString());
            shutdownError = true;
        }
    }

    public void disconnect()
    {
        shutdownError = false;
        if (!connected)
            return;

        connected = false;
        try
        {
            StopAllCoroutines();
            client.Close();
        }
        catch (Exception e)
        {
            Debug.Log("dis: " + e.ToString());
            //close socket silently
        }
        gameState.closeGame();
    }
}
