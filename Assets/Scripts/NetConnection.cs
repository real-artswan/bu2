using BaboNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public delegate void AddPacketCallback(BaboRawPacket packet);
public delegate bool GetPacketCallback(out BaboRawPacket packet);

public class NetConnection : MonoBehaviour
{
    public GameState gameState;

    internal bool connected = false;

    public Queue<BaboRawPacket> recievedPackets = new Queue<BaboRawPacket>();
    public Queue<BaboRawPacket> packetsToSend = new Queue<BaboRawPacket>();

    private BaboNetReadProcessor readBufferProcessor;
    private BaboNetWriteProcessor writeBufferProcessor;
    private TcpClient client;

    private BaboNetPacketProcessor packetsProcessor;
    void Start() {
        //prepare buffer processors
        readBufferProcessor = new BaboNetReadProcessor((p) => {
            /*if (p.typeID == 106)
                Debug.Log(DateTime.Now.ToString() + " Q<-PING");*/
            recievedPackets.Enqueue(p);
        });
        writeBufferProcessor = new BaboNetWriteProcessor(
            (out BaboRawPacket p) => {
                bool res = packetsToSend.Count > 0;
                p = null;
                if (res) {
                    p = packetsToSend.Dequeue();
                    /*if (p.typeID == 1)
                        Debug.Log(DateTime.Now.ToString() + " Q->PONG");*/
                }
                return res;
            });
        packetsProcessor = new BaboNetPacketProcessor(gameState, p => { packetsToSend.Enqueue(p); });
    }

    private IEnumerator readLoop() {
        if (Debug.isDebugBuild)
            Debug.Log("Read loop started");
        while (connected) {
            while (recievedPackets.Count > 0) {
                BaboRawPacket packet = recievedPackets.Dequeue();
                packetsProcessor.processPacket(packet); //update game state
                /*if (packet.typeID == 106)
                    Debug.Log(DateTime.Now.ToString() + " <PING> " + connection.recievedPackets.Count.ToString());*/

                //yield return new WaitForSeconds(0.001f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator writeLoop() {
        if (Debug.isDebugBuild)
            Debug.Log("Write loop started");
        while (connected) {
            while (packetsToSend.Count > 0) {
                while ((writeResult != null) && (!writeResult.IsCompleted)) //wait while writing prev buffer
                {
                    yield return new WaitForSeconds(0.001f);
                }
                byte[] buffer = writeBufferProcessor.prepareBuffer();
                try {
                    //Debug.Log("===>" + DateTime.Now.ToString());
                    writeResult = netStream.BeginWrite(buffer, 0, buffer.Length, null, null);
                }
                catch (Exception e) {
                    Debug.Log("net write: " + e.ToString());
                    shutdownError = true;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private NetworkStream netStream;
    private bool shutdownError = false;
    private IAsyncResult writeResult = null;

    void Update() {
        if (shutdownError) {
            Debug.Log("disconnected shutdown");
            disconnect();
            return;
        }
    }

    private bool prepareConnection() {
        try {
            client = new TcpClient(AddressFamily.InterNetwork);
            client.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ReceiveTimeout = 100000;
            client.SendTimeout = 100000;
            //client.SendBufferSize = BaboNetWriteProcessor.dataRate;
            //client.ReceiveBufferSize = 3072;
            return true;
        }
        catch (Exception e) {
            Debug.Log("pr_con: " + e.ToString());
            return false;
        }
    }

    public void startConnection() {
        shutdownError = false;
        if (connected)
            disconnect();
        if (!prepareConnection())
            return;
        try {
            //try to connect
            client.Connect(gameState.gameVars.lastIP.text, int.Parse(gameState.gameVars.lastPort.text));

            //We are connected successfully.
            netStream = client.GetStream();

            //now we need 37 bytes from server where we get packetID
            recieveConnInfo();

            byte[] readBuffer = new byte[512];

            //Now we are able to communicate with server, start async read operation in loop
            netStream.BeginRead(readBuffer, 0, readBuffer.Length, ReadCallback, readBuffer);

            connected = true;
            //run game logic
            gameState.startGame();

            StartCoroutine(writeLoop());
            StartCoroutine(readLoop());
        }
        catch (Exception e) {
            Debug.Log("st_con: " + e.ToString());
            shutdownError = true;
        }
    }

    private bool recieveConnInfo() {
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

    private void ReadCallback(IAsyncResult result) {
        try {
            byte[] buffer = result.AsyncState as byte[];
            int readBytes = netStream.EndRead(result);
            if (!readBufferProcessor.parseBuffer(readBytes, buffer)) {
                Debug.Log("Fail to parse buffer");
                shutdownError = true;
                return;
            }
            //Start reading from the network again.
            netStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }
        catch (Exception e) {
            Debug.Log("read: " + e.ToString());
            shutdownError = true;
        }
    }

    public void disconnect() {
        shutdownError = false;
        if (!connected)
            return;

        connected = false;
        try {
            StopAllCoroutines();
            client.Close();
        }
        catch (Exception e) {
            Debug.Log("dis: " + e.ToString());
            //close socket silently
        }
        //stop game logic
        gameState.closeGame();
    }
}
