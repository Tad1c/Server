using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string userName = packet.ReadString();

        Debug.Log($"{Server.clients[clientIdCheck].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}, with user name {userName}");

        if (fromClient != clientIdCheck)
            Debug.Log($"Player \" {userName}\" (ID: {fromClient}) has assumed to wrong client ID ({clientIdCheck})");

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

    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 shootingDireciton = packet.ReadVector3();
        Server.clients[fromClient].player.Shoot(shootingDireciton);
    }

    //public static void UDPTestReceived(int fromClient, Packet packet)
    //{
    //    string msg = packet.ReadString();

    //    Debug.Log($"Received packet via UDP. Contains message: {msg}");
    //}
}
