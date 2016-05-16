using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
    public int PlayerID { get; private set; }

    public void Initialize(int id) {
        PlayerID = id;

        Vector3 spawnPoint = new Vector3(0, 5, 11);
        //TODO Get the prefab from somewhere that makes sense.
        GameObject playerPrefab = NetworkManager.singleton.spawnPrefabs[0];

        GameObject spawned = Instantiate(playerPrefab, spawnPoint, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(spawned);
    }
}
