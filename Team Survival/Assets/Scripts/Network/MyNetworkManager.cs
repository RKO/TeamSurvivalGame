using UnityEngine;
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
        player.Initialize(playerNumber);
        _players.Add(playerNumber, player);

        //Spawn on network.
        NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
	}

    public override void OnMatchCreate(UnityEngine.Networking.Match.CreateMatchResponse resp) {
        base.OnMatchCreate(resp);

        if (!resp.success) {
            Debug.LogError("Failed to create Match -> "+resp.extendedInfo);
        }

        m_networkID = (ulong)resp.networkId;
    }

    public void DestroyMatch(ulong id) {
        this.matchMaker.DestroyMatch((NetworkID)id, OnDestroyMatch);
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
        this.matchMaker.DestroyMatch((NetworkID)m_networkID, OnDestroyMatch);
    }

    public void OnDestroyMatch(UnityEngine.Networking.Match.BasicResponse response)
    {
        if (response.success)
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
