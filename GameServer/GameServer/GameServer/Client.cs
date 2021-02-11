using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    class Client
    {

        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;
        public UDP udp;

        public Player player;

        public Client(int clientId)
        {
            id = clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            //This is where we store the instance that we get in the server callback
            public TcpClient socket;

            private readonly int id;


            private NetworkStream stream;

            private Packet receivedData;

            private byte[] receiveBuffer;

            public TCP(int id)
            {
                this.id = id;
            }

            public void Connect(TcpClient socket)
            {
                this.socket = socket;

                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLenght = stream.EndRead(result);
                    if (byteLenght <= 0)
                    {
                        //TODO: disconnect
                        return;
                    }

                    byte[] data = new byte[byteLenght];
                    Array.Copy(receiveBuffer, data, byteLenght);

                    receivedData.Reset(HandledData(data));


                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving TCP data: {e}");
                    //TODO: disconnet
                }
            }

            private bool HandledData(byte[] data)
            {
                int packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0)
                        return true;
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
                {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);

                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.packetHandlers[packetId](id, packet);
                        }
                    });

                    packetLength = 0;

                    if (receivedData.UnreadLength() >= 4)
                    {
                        packetLength = receivedData.ReadInt();

                        if (packetLength <= 0)
                            return true;
                    }
                }

                if (packetLength <= 1)
                    return true;

                return false;
            }

        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int id)
            {
                this.id = id;
            }
            
            public void Connect(IPEndPoint endPoint)
            {
                this.endPoint = endPoint;
               // ServerSend.UDPTest(id);
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using(Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }

                });
            }
        }

        public void SendIntoGame(string playerName)
        {
            player = new Player(id, playerName, Vector3.Zero);

            foreach(Client client in Server.clients.Values)
            {
                if(client.player != null)
                {
                    if(client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, client.player);
                    }
                }
            }

            foreach(Client client in Server.clients.Values)
            {
                if(client.player != null)
                {
                    ServerSend.SpawnPlayer(client.id, player);
                }
            }

        }

    }
}
