using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public GameObject CameraPrefab;

    public UnitData HeroUnitData;

    public int PlayerID { get; private set; }

    public string PlayerName { get; set; }

    private bool _initialized;

    [SyncVar]
    private PlayerController controller;
    private AbilityList _abilities;
    private UnitShell _shell;

    [Server]
    public void Initialize(int id) {
        PlayerID = id;
    }

    void Start() {
        _abilities = gameObject.GetComponent<AbilityList>();
        _shell = gameObject.GetComponent<UnitShell>();

        _shell.OnKillCallback += OnKill;

        if (isServer)
        {
            _shell.Initialize(HeroUnitData);
        }
    }

    private void OnKill()
    {
        Debug.LogError("Player died!");
    }

    void Update() {
        if (!isLocalPlayer)
            return;

        if (!_initialized)
        {
            _initialized = true;

            controller = gameObject.GetComponentInChildren<PlayerController>();
            controller.PlayerInitialize(CameraPrefab, this);
        }
    }

    [Command]
    public void CmdSetMoveDir(Vector3 moveDir)
    {
        _shell.Motor.SetMoveDirection(moveDir);
    }

    [Command]
    public void CmdSetRotateDestination(Vector3 dir)
    {
        _shell.Motor.SetRotateDestination(dir);
    }

    [Command]
    public void CmdActivateAbilityIndex(int index)
    {
        _abilities.ActivateAbility(index);
    }

    [Command]
    public void CmdActivateAbilitySlot(AbilitySlot slot)
    {
        _abilities.ActivateAbility(slot);
    }
}
