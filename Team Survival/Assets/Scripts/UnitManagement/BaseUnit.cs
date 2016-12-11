using UnityEngine;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
public abstract class BaseUnit : MonoBehaviour, IUnit {

    public UnitShell Shell { get; protected set; }
    public BaseMotor Motor { get; protected set; }
    public AbilityList Abilities { get; protected set; }
    private AnimationSync _animationSync;
    public bool IsOnServer;

    public abstract Team GetTeam { get; }

    public abstract string Name { get; }

    public int MaxHealth;

    public void Initialize(UnitShell shell) {
        Shell = shell;
        IsOnServer = Shell.isServer;
        Motor = GetComponent<BaseMotor>();
        Abilities = GetComponent<AbilityList>();
        _animationSync = GetComponent<AnimationSync>();
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

    public void SetNewAnimation(UnitAnimation newAnimation)
    {
        _animationSync.SetNewAnimation(newAnimation);
    }

    public void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        _animationSync.TriggerAnimation(triggerAnim);
    }

    protected virtual void UnitUpdate() { }

    protected virtual void UnitOnAwake() { }

    protected virtual void UnitOnDestroy() { }

    public virtual void UnitOnKill() { }

    public virtual void UnitOnDeath() { }
}
