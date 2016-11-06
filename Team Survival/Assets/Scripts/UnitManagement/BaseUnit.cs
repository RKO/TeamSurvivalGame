using UnityEngine;

public abstract class BaseUnit : MonoBehaviour, IUnit {

    public UnitShell Shell { get; protected set; }
    public BaseMotor Motor { get; protected set; }
    public AbilityList Abilities { get; protected set; }
    public Animator UnitAnimator { get; protected set; }
    public bool IsOnServer;

    public abstract Team GetTeam { get; }

    public abstract string Name { get; }

    public int MaxHealth;

    public UnitAnimation CurrentAnimation = UnitAnimation.Idle;

    public void Initialize(UnitShell shell, BaseMotor motor, AbilityList abilities, bool isOnServer) {
        Shell = shell;
        Motor = motor;
        Abilities = abilities;
        IsOnServer = isOnServer;

        UnitAnimator = GetComponent<Animator>();
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
        UnitUpdate();
    }

    protected virtual void UnitUpdate() { }

    protected virtual void UnitOnAwake() { }

    protected virtual void UnitOnDestroy() { }

    public virtual void TriggerAnimation(UnitTriggerAnimation triggerAnim) { }
}
