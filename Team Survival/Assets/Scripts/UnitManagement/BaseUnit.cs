using UnityEngine;

public abstract class BaseUnit : MonoBehaviour, IUnit {

    public BaseMotor Motor { get; protected set; }
    public AbilityList Abilities { get; protected set; }
    public AnimationSync Animations { get; protected set; }
    public bool IsOnServer;

    public abstract Team GetTeam { get; }

    public abstract string Name { get; }

    public int MaxHealth;

    public void Initialize(BaseMotor motor, AbilityList abilities, AnimationSync animations, bool isOnServer) {
        Motor = motor;
        Abilities = abilities;
        Animations = animations;
        IsOnServer = isOnServer;
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
}
