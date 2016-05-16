using UnityEngine;
using UnityEngine.Networking;

public abstract class BaseUnit : NetworkBehaviour, IUnit {
    public abstract Team GetTeam { get; }

    private void Awake() {
        GameManager.Instance.unitManager.AddUnit(this);
        UnitOnAwake();
    }

    private void OnDestroy() {
        GameManager.Instance.unitManager.KillUnit(this);
        UnitOnDestroy();
    }

    protected virtual void UnitOnAwake() { }

    protected virtual void UnitOnDestroy() { }
}
