using System;
using System.Collections.Generic;
using System.Text;

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

            //TODO: send player into game
        }
    }
}
