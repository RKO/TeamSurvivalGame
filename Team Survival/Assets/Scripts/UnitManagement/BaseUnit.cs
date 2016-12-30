using UnityEngine;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
public class BaseUnit : MonoBehaviour, IUnit {

    public UnitShell Shell { get; protected set; }
    public BaseMotor Motor { get; protected set; }
    public AbilityList Abilities { get; protected set; }
    private AnimationSync _animationSync;
    public bool IsOnServer;

    public float UnitMoveSpeed;
    public float MoveSpeed { get { return UnitMoveSpeed; } }

    public Team UnitTeam;
    public Team GetTeam { get { return UnitTeam; } }

    public string UnitName;
    public string Name { get { return UnitName; } }

    public int MaxHealth;

    public void Initialize(UnitShell shell) {
        Shell = shell;
        IsOnServer = Shell.isServer;
        Motor = GetComponent<BaseMotor>();
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
                if (Motor.MoveDirection != Vector3.zero)
                    _animationSync.SetNewAnimation(UnitAnimation.Running);
                else
                    _animationSync.SetNewAnimation(UnitAnimation.Idle);
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
    }

    public virtual void UnitOnDeath() {
        _animationSync.SetNewAnimation(UnitAnimation.Dead);

        Motor.SetMoveDirection(Vector3.zero);
        Motor.SetRotateDestination(Vector3.zero);
    }
}
