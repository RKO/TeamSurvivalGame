using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public SpawnManager spawnManager;
    public Transform goal;

    // Use this for initialization
    void Awake () {
        Instance = this;
    }

    void Start() {
        spawnManager = gameObject.AddComponent<SpawnManager>();

        spawnManager.StartSpawning();

        goal = GameObject.FindObjectOfType<GoalZone>().transform;
    }

    public void DisplayGlobalMessage(string message) {
        Debug.Log(message);
    }
}
