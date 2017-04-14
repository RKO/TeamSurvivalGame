using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance { get; private set; }

    public SpawnManager spawnManager;
    public Transform goal;
    public UnitManager unitManager;
    public bool IsGUIOpen { get; set; }

    public EffectSync EffectManager { get; private set; }
    public UnitRegistry UnitRegistry { get; private set; }

    // Use this for initialization
    void Awake () {
        Instance = this;

        spawnManager = gameObject.AddComponent<SpawnManager>();
        unitManager = new UnitManager();

        EffectManager = GetComponent<EffectSync>();
        UnitRegistry = GetComponent<UnitRegistry>();
        UnitRegistry.Initialize();
    }

    public override void OnStartServer() {
        goal = FindObjectOfType<GoalZone>().transform;

        spawnManager.StartSpawning();
    }

    public void DisplayGlobalMessage(string message) {
        Debug.Log(message);
    }
}
