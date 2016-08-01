using UnityEngine;

public abstract class BaseUnit : MonoBehaviour, IUnit {

    public BaseMotor Motor;
    public AbilityList Abilities;
    public bool IsOnServer;

    public abstract Team GetTeam { get; }

    public void Initialize(BaseMotor motor, AbilityList abilities, bool isOnServer) {
        Motor = motor;
        Abilities = abilities;
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
