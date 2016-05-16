using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Reflection;
using UnityEngine.Networking.Types;

public class MyNetworkManager : NetworkManager {
    private ulong m_networkID;

    private int[] players = new int[] { -1, -1, -1, -1 };


	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
        int playerNumber = GetNewPlayerNumber(conn);
        Debug.Log("Player joined. Assigning id: "+playerNumber);

		var player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        //Set values on player script.
        //player.GetComponent<Player>().playerColor = playerColors[playerNumber];

        //Spawn on network.
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
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
        int playerNumber = FindPlayerNumber(conn);

        players[playerNumber] = -1;

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

    private int GetNewPlayerNumber(NetworkConnection conn)
    {
        int connId = conn.connectionId;

        //For some reason, the first player has id -1. So we have to check for that.
        if (connId == -1)
            connId = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] < 0)
            {
                players[i] = connId;
                return i;
            }
        }

        return -1;
    }

    private int FindPlayerNumber(NetworkConnection conn)
    {
        int connId = conn.connectionId;

        //For some reason, the first player has id -1. So we have to check for that.
        if (connId == -1)
            connId = 0;


        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == connId)
            {
                return i;
            }
        }

        return -1;
    }
}
