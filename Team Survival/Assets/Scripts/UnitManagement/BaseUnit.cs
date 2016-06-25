using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
public abstract class BaseUnit : NetworkBehaviour, IUnit {

    public BaseMotor Motor;
    public AbilityList Abilities;

    public abstract Team GetTeam { get; }

    private void Awake() {
        Motor = GetComponent<BaseMotor>();
        Abilities = GetComponent<AbilityList>();
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
