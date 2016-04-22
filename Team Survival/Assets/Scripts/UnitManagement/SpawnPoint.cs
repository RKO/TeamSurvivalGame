using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public BoxCollider spawnBox;
    public bool HasSpawnArea { get; private set; }

	// Use this for initialization
	void Start () {
        spawnBox = GetComponent<BoxCollider>();

        HasSpawnArea = (spawnBox != null);
    }
}
