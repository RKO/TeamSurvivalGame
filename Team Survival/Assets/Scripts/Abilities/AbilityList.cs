using UnityEngine.Networking;
using System.Collections.Generic;

public class AbilityList : NetworkBehaviour {

    private List<BaseAbility> _abilities;
    public SyncListAbilityState _abilityStates;

    void Awake() {
        _abilities = new List<BaseAbility>();
        _abilityStates = new SyncListAbilityState();
    }

    [ServerCallback]
    private void Update()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            BaseAbility ability = _abilities[i];
            ability.Update();

            AbilityState abs = new AbilityState();
            abs.cooldownPercent = ability.CooldownPercent;
            abs.name = ability.Name;
            _abilityStates[i] = abs;
        }
    }

    [Server]
    public void GrantAbility(BaseAbility newAbility)
    {
        _abilities.Add(newAbility);
        _abilityStates.Add(new AbilityState());
    }

    [Server]
    public void ActivateAbility(int abilityIndex)
    {
        _abilities[abilityIndex].Activate();
    }

    [Server]
    public void RemoveAbility(BaseAbility toRemove)
    {
        _abilities.Remove(toRemove);
        _abilityStates.RemoveAt(_abilityStates.Count - 1);
    }

    //Stuff to make the syncList work...
    public class SyncListAbilityState : SyncListStruct<AbilityState> { }
    public struct AbilityState {
        public string name;
        public float cooldownPercent;
    }
}
