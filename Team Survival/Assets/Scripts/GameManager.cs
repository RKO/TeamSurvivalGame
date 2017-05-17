using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    public static GameManager Instance { get; private set; }

    public SpawnManager SpawnManager { get; private set; }
    public Transform Goal { get; private set; }
    public UnitManager UnitManager { get; private set; }
    public bool IsGUIOpen { get; set; }

    public EffectSync EffectManager { get; private set; }
    public UnitRegistry UnitRegistry { get; private set; }

    [SerializeField]
    private GameObject UnitDecoratorPrefab;
    private UnitDecorator _unitDecorator;

    // Use this for initialization
    void Awake () {
        Instance = this;

        SpawnManager = gameObject.AddComponent<SpawnManager>();
        UnitManager = new UnitManager();

        GameObject decoratorObj = Instantiate(UnitDecoratorPrefab, transform) as GameObject;
        _unitDecorator = decoratorObj.GetComponent<UnitDecorator>();
        _unitDecorator.Initialize(UnitManager);

        EffectManager = GetComponent<EffectSync>();
        UnitRegistry = GetComponent<UnitRegistry>();
        UnitRegistry.Initialize();
    }

    public override void OnStartServer() {
        Goal = FindObjectOfType<GoalZone>().transform;

        SpawnManager.StartSpawning();
    }

    public void DisplayGlobalMessage(string message) {
        Debug.Log(message);
    }
}
