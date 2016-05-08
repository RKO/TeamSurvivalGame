using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public BoxCollider spawnBox;
    public bool HasSpawnArea { get; private set; }
    public Transform[] waypoints;

	// Use this for initialization
	void Start () {
        spawnBox = GetComponent<BoxCollider>();

        HasSpawnArea = (spawnBox != null);
    }
}
