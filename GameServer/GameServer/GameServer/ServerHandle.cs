using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string userName = packet.ReadString();

            Console.WriteLine($"{Server.clients[clientIdCheck].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}, with user name {userName}");

            if (fromClient != clientIdCheck)
                Console.WriteLine($"Player \" {userName}\" (ID: {fromClient}) has assumed to wrong client ID ({clientIdCheck})");

            Server.clients[fromClient].SendIntoGame(userName);
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }

            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInputs(inputs, rotation);
            
        }

        //public static void UDPTestReceived(int fromClient, Packet packet)
        //{
        //    string msg = packet.ReadString();

        //    Console.WriteLine($"Received packet via UDP. Contains message: {msg}");
        //}
    }
}
