﻿using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

public class MyNetworkManager : NetworkManager {
    private ulong m_networkID;

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
        int playerNumber = GetPlayerNumber(conn);
        Debug.Log("Player joined. Assigning id: "+playerNumber);

		var playerObj = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        Player player = playerObj.GetComponent<Player>();


        // -------------- Player Body -------------- //

        //Spawn the body of the player.
        Vector3 spawnPoint = new Vector3(0, 5, 11);
        //TODO Get the prefab from somewhere that makes sense.
        GameObject bodyPrefab = spawnPrefabs[0];

        GameObject playerBody = Instantiate(bodyPrefab, spawnPoint, Quaternion.identity) as GameObject;
        BaseMotor motor = playerBody.GetComponent<BaseMotor>();

        UnitShell shell = playerBody.GetComponent<UnitShell>();
        //TODO Load abilities from this too?
        shell.UnitPrefabToLoad = "Spawnable/Characters/WarriorPrincess";
        
        motor.Initialize(Player.MoveSpeed);

        NetworkServer.Spawn(playerBody);

        // -------------- Player -------------- //

        player.Initialize(playerNumber, playerBody.GetComponent<NetworkIdentity>());
        _players.Add(playerNumber, player);

        //Spawn on network.
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
    }

   public override void OnMatchCreate(bool success, string extendedInfo, UnityEngine.Networking.Match.MatchInfo resp) {
        base.OnMatchCreate(success, extendedInfo, resp);

        if (!success) {
            Debug.LogError("Failed to create Match -> "+extendedInfo);
        }

        m_networkID = (ulong)resp.networkId;
    }

    public void DestroyMatch(ulong id) {
        this.matchMaker.DestroyMatch((NetworkID)id, 0, OnDestroyMatch);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        this.matchMaker.DestroyMatch((NetworkID)m_networkID, 0, OnDestroyMatch);
    }

    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        base.OnDestroyMatch(success, extendedInfo);

        if (success)
        {
            Debug.Log("The match has been destroyed");
        }
        else {
            Debug.LogError("Destroying match failed.");
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
	{
        int playerNumber = GetPlayerNumber(conn);
        _players.Remove(playerNumber);

        Debug.LogWarning("Player disconnected with assigned id: " + playerNumber);

        base.OnServerDisconnect (conn);
	}

    //Hack to remove bugged warnings from MatchMaking.
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Type serverType = typeof(NetworkServer);

        FieldInfo info = serverType.GetField("maxPacketSize", BindingFlags.NonPublic | BindingFlags.Static);

        ushort maxPackets = 1500;

        info.SetValue(null, maxPackets);
    }

    private int GetPlayerNumber(NetworkConnection conn)
    {
        int connId = conn.connectionId;

        //For some reason, the first player has id -1. So we have to check for that.
        if (connId == -1)
            connId = 0;

        return connId;
    }
}
