using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Client
    {

        public static int dataBufferSize = 4096;

        public int id;
        public TCP tcp;

        public Client(int clientId)
        {
            id = clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            //This is where we store the instance that we get in the server callback
            public TcpClient socket;

            private readonly int id;


            private NetworkStream stream;
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

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                //TODO: send welcome packet
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLenght = stream.EndRead(result);
                    if(byteLenght <= 0)
                    {
                        //TODO: disconnect
                        return;
                    }

                    byte[] data = new byte[byteLenght];
                    Array.Copy(receiveBuffer, data, byteLenght);

                    //TODO: handle data
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                }catch(Exception e)
                {
                    Console.WriteLine($"Error receiving TCP data: {e}");
                    //TODO: disconnet
                }
            }

        }

    }
}
