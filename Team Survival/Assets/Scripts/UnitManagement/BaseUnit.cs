using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(BaseMotor))]
public abstract class BaseUnit : NetworkBehaviour, IUnit {

    public BaseMotor Motor;
    protected List<BaseAbility> abilities = new List<BaseAbility>();

    public abstract Team GetTeam { get; }

    private void Awake() {
        Motor = GetComponent<BaseMotor>();
        GameManager.Instance.unitManager.AddUnit(this);
        UnitOnAwake();
    }

    private void OnDestroy() {
        GameManager.Instance.unitManager.KillUnit(this);
        UnitOnDestroy();
    }

    private void Update() {
        UpdateAbilities();
        UnitUpdate();
    }

    private void UpdateAbilities() {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].Update();
        }
    }

    public void GrantAbility(BaseAbility newAbility) {
        abilities.Add(newAbility);
    }

    public void ActivateAbility(int abilityIndex) {
        abilities[abilityIndex].Activate(this);
    }

    public void RemoveAbility(BaseAbility toRemove) {
        abilities.Remove(toRemove);
    }

    protected virtual void UnitUpdate() { }

    protected virtual void UnitOnAwake() { }

    protected virtual void UnitOnDestroy() { }
}
