using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public SpawnManager spawnManager;
    public Transform goal;
    public UnitManager unitManager;

    // Use this for initialization
    void Awake () {
        Instance = this;

        spawnManager = gameObject.AddComponent<SpawnManager>();
        unitManager = new UnitManager();
    }

    void Start() {
        goal = FindObjectOfType<GoalZone>().transform;

        spawnManager.StartSpawning();
    }

    public void DisplayGlobalMessage(string message) {
        Debug.Log(message);
    }
}
