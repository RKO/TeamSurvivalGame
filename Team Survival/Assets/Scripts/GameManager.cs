using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public SpawnManager spawnManager;

    // Use this for initialization
    void Awake () {
        Instance = this;
    }

    void Start() {
        spawnManager = gameObject.AddComponent<SpawnManager>();

        spawnManager.StartSpawning();
    }

    public void DisplayGlobalMessage(string message) {
        Debug.Log(message);
    }
}
