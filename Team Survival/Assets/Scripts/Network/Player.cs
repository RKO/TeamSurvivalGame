using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public GameObject CameraPrefab;

    public int PlayerID { get; private set; }

    public string PlayerName { get; set; }

    private bool _initialized;

    [SyncVar]
    private PlayerController controller;
    private BaseMotor _motor;
    private AbilityList _abilities;
    private UnitShell _shell;

    public void Initialize(int id) {
        PlayerID = id;
    }

    void Start() {
        _motor = gameObject.GetComponent<BaseMotor>();
        _abilities = gameObject.GetComponent<AbilityList>();
        _shell = gameObject.GetComponent<UnitShell>();

        _shell.OnKillCallback += OnKill;

        if (isServer) {
            
            _abilities.GrantAbility(new AbilityJump(_motor, _shell), AbilitySlot.Jump);
            //_abilities.GrantAbility(new AbilityBasicAttack(_motor, _shell, 1.1f, 0.3f), AbilitySlot.Attack1);
            _abilities.GrantAbility(new AbilitySweepingStrike(_motor, _shell, 1.1f, 0.3f), AbilitySlot.Attack1);
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

            //If it should ever be needed, that the client can tell the server to grant an ability, this is the way to do it.
            //GrantAbility(typeof(AbilityJump), AbilityList.AbilitySlot.Jump);
            //GrantAbility(typeof(AbilityBasicAttack), AbilityList.AbilitySlot.Attack1);
        }
    }

    [Command]
    public void CmdSetMoveDir(Vector3 moveDir)
    {
        _motor.SetMoveDirection(moveDir);
    }

    [Command]
    public void CmdSetRotateDestination(Vector3 dir)
    {
        _motor.SetRotateDestination(dir);
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

    /// <summary>
    /// The server should really be the one to grant abilities, but this makes it possible from the client.
    /// </summary>
    public void GrantAbility(Type type, AbilitySlot slot = AbilitySlot.None) {
        CmdGrantAbility(type.FullName, (int)slot);
    }

    [Command]
    /// <summary>
    /// The server should really be the one to grant abilities, but this makes it possible from the client.
    /// </summary>
    private void CmdGrantAbility(string abilityType, int slotInt)
    {
        Type type = Type.GetType(abilityType);
        System.Object instance = Activator.CreateInstance(type, new object[] { _motor });

        if (instance is BaseAbility)
        {
            AbilitySlot slot = (AbilitySlot)slotInt;
            _abilities.GrantAbility(instance as BaseAbility, slot);
        }
        else {
            Debug.LogError("Type \""+abilityType+"\" can't be assigned as a BaseAbility object. Aborting CmdGrantAbility");
        }
    }
}
