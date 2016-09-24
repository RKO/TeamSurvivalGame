using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public const float MoveSpeed = 6;

    public GameObject CameraPrefab;

    public int PlayerID { get; private set; }

    public string PlayerName { get; set; }

    private bool _initialized;

    [SyncVar]
    private NetworkIdentity _bodyIdentity;
    private PlayerController controller;
    private BaseMotor _motor;
    private AbilityList _abilities;
    private AnimationSync _animationSync;

    public void Initialize(int id, NetworkIdentity body) {
        PlayerID = id;
        _bodyIdentity = body;
    }

    void Start() {
        _motor = _bodyIdentity.gameObject.GetComponent<BaseMotor>();
        _abilities = _bodyIdentity.gameObject.GetComponent<AbilityList>();
        _animationSync = _bodyIdentity.gameObject.GetComponent<AnimationSync>();


        if (isServer) {
            _abilities.GrantAbility(new AbilityJump(_motor, _animationSync), AbilitySlot.Jump);
            _abilities.GrantAbility(new AbilityBasicAttack(_motor, _animationSync), AbilitySlot.Attack1);
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
        if (moveDir != Vector3.zero)
            _animationSync.CurrentAnimation = UnitAnimation.Running;
        else
            _animationSync.CurrentAnimation = UnitAnimation.Idle;
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
