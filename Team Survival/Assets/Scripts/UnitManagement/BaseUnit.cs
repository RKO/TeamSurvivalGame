using System;
using UnityEngine;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(AbilityList))]
public class BaseUnit : MonoBehaviour, IUnit {

    public UnitShell Shell { get; protected set; }
    public IMotor Motor { get; protected set; }
    public AbilityList Abilities { get; protected set; }
    private AnimationSync _animationSync;
    public bool IsOnServer;

    public float UnitMoveSpeed;
    public float MoveSpeed { get { return UnitMoveSpeed; } }

    public Team UnitTeam;
    //TODO Rename!
    public Team GetTeam { get { return UnitTeam; } }

    public string UnitName;
    public string Name { get { return UnitName; } }

    public Vector3 Position { get { return transform.position; } }

    public int MaxHealth;

    public void Initialize(UnitShell shell) {
        Shell = shell;
        IsOnServer = Shell.isServer;
        Motor = GetComponent<IMotor>();
        Abilities = GetComponent<AbilityList>();
        _animationSync = GetComponent<AnimationSync>();

        Motor.Initialize(MoveSpeed);

        if (IsOnServer)
            _animationSync.SetNewAnimation(UnitAnimation.Idle);
    }

    private void Awake() {
        GameManager.Instance.unitManager.AddUnit(this);
        UnitOnAwake();
    }

    private void OnDestroy() {
        GameManager.Instance.unitManager.KillUnit(this);
        UnitOnDestroy();
    }

    private void Update() {
        if (IsOnServer)
        {
            if (Shell.AliveState == LifeState.Alive)
            {
                float actualSpeed = Motor.MoveDirection.magnitude;
                
                if (actualSpeed == 0)
                    _animationSync.SetNewAnimation(UnitAnimation.Idle);
                else if(actualSpeed < MoveSpeed * 0.5f)
                    _animationSync.SetNewAnimation(UnitAnimation.Walking);
                else 
                    _animationSync.SetNewAnimation(UnitAnimation.Running);
            }
            ServerSideUpdate();
        }
        else {
            ClientSideUpdate();
        }
    }

    public void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        _animationSync.TriggerAnimation(triggerAnim);
    }

    protected virtual void ServerSideUpdate() { }

    protected virtual void ClientSideUpdate() { }

    protected virtual void UnitOnAwake() { }

    protected virtual void UnitOnDestroy() { }


    //TODO Virtual or not virtual? (Make sure the required code is always called, or allow override?)
    public virtual void UnitOnKill() {
        _animationSync.SetNewAnimation(UnitAnimation.Dying);
        GetComponent<Collider>().enabled = false;
        var obstacle = GetComponent<NavMeshObstacle>();
        if(obstacle != null)
            obstacle.enabled = false;

        Motor.Stop();
        if (OnKillCallback != null)
            OnKillCallback();
    }

    public virtual void UnitOnDeath() {
        _animationSync.SetNewAnimation(UnitAnimation.Dead);

        if (OnDeathCallback != null)
            OnDeathCallback();
    }

    public delegate void OnKillDelegate();
    public OnKillDelegate OnKillCallback;

    public delegate void OnDeathDelegate();
    public OnKillDelegate OnDeathCallback;
}
