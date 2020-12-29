﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _ClientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} " +
            $"connected successfully and is now player {_fromClient}");
        if (_fromClient != _ClientIdCheck)
        {
            Debug.Log($"Player \"{_username}\"(ID: {_fromClient}) has assumed the wrong ID ({_ClientIdCheck})!");
        }

        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {/*
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; ++i)
        {
            _inputs[i] = _packet.ReadBool();
        }*/
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_position, _rotation);
    }
}
