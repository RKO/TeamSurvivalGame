using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public const float MoveSpeed = 6;

    public GameObject CameraPrefab;

    public int PlayerID { get; private set; }

    private bool _initialized;

    [SyncVar]
    private NetworkIdentity _bodyIdentity;
    private PlayerController controller;
    private BaseMotor _motor;
    private AbilityList _abilities;

    public void Initialize(int id, NetworkIdentity body) {
        PlayerID = id;
        _bodyIdentity = body;
    }

    void Start() {
        _motor = _bodyIdentity.gameObject.GetComponent<BaseMotor>();
        _abilities = _bodyIdentity.gameObject.GetComponent<AbilityList>();


        if (isServer) {
            _abilities.GrantAbility(new AbilityJump(_motor), AbilityList.AbilitySlot.Jump);
            _abilities.GrantAbility(new AbilityBasicAttack(_motor), AbilityList.AbilitySlot.Attack1);
        }
    }

    void Update() {
        if (!isLocalPlayer)
            return;

        if (!_initialized)
        {
            _initialized = true;

            controller = _bodyIdentity.gameObject.AddComponent<PlayerController>();
            controller.Initialize(CameraPrefab, this);

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
    public void CmdActivateAbilitySlot(AbilityList.AbilitySlot slot)
    {
        _abilities.ActivateAbility(slot);
    }

    private void GrantAbility(Type type, AbilityList.AbilitySlot slot = AbilityList.AbilitySlot.None) {
        CmdGrantAbility(type.FullName, (int)slot);
    }

    [Command]
    public void CmdGrantAbility(string abilityType, int slotInt)
    {
        Type type = Type.GetType(abilityType);
        System.Object instance = Activator.CreateInstance(type, new object[] { _motor });

        if (instance is BaseAbility)
        {
            AbilityList.AbilitySlot slot = (AbilityList.AbilitySlot)slotInt;
            _abilities.GrantAbility(instance as BaseAbility, slot);
        }
        else {
            Debug.LogError("Type \""+abilityType+"\" can't be assigned as a BaseAbility object. Aborting CmdGrantAbility");
        }
    }
}
