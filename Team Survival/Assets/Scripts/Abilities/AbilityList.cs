﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class AbilityList : NetworkBehaviour {

    private List<BaseAbility> _abilities;
    public SyncListAbilityState _abilityStates;

    private Dictionary<AbilitySlot, int> _abilitySlotMap;
    private Dictionary<AbilitySlot, AbilitySynchronizer> _abilitySyncs;

    void Awake() {
        _abilities = new List<BaseAbility>();
        _abilityStates = new SyncListAbilityState();
        _abilitySlotMap = new Dictionary<AbilitySlot, int>();
        _abilitySyncs = new Dictionary<AbilitySlot, AbilitySynchronizer>();

        foreach (AbilitySlot slot in Enum.GetValues(typeof(AbilitySlot)))
        {
            _abilitySlotMap.Add(slot, -1);
        }
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
            abs.isActive = ability.IsActive;
            abs.canActivate = ability.CanActivate;
            _abilityStates[i] = abs;
        }
    }

    [Server]
    public void GrantAbility(BaseAbility newAbility, AbilitySlot slot, bool spawnSynchronization = false)
    {
        _abilities.Add(newAbility);
        _abilityStates.Add(new AbilityState());

        //It is possible to overwrite assigned abilities.
        if (slot != AbilitySlot.None) {
            _abilitySlotMap[slot] = _abilities.IndexOf(newAbility);

            if (spawnSynchronization) {
                AbilitySynchronizer sync;

                _abilitySyncs.TryGetValue(slot, out sync);
                if (sync == null) {
                    GameObject go = Instantiate(AbilityInfoSync.GetAbilitySyncPrefab());
                    sync = go.GetComponent<AbilitySynchronizer>();
                    _abilitySyncs.Add(slot, sync);
                }

                sync.AbilityID = newAbility.GetInfo().UniqueID;
            }
        }
    }

    [Server]
    public void ActivateAbility(int abilityIndex)
    {
        _abilities[abilityIndex].Activate();
    }

    [Server]
    public void ActivateAbility(AbilitySlot slot)
    {
        int index = _abilitySlotMap[slot];
        if (index != -1)
            ActivateAbility(index);
    }

    public AbilityState GetAbilityState(int abilityIndex) {
        return _abilityStates[abilityIndex];
    }

    public AbilityState GetAbilityState(AbilitySlot slot)
    {
        int index = _abilitySlotMap[slot];
        if (index != -1)
            return GetAbilityState(index);

        return new AbilityState() { isGarbage = true };
    }

    [Server]
    public void RemoveAbility(BaseAbility toRemove)
    {
        int index = _abilities.IndexOf(toRemove);
        _abilities.Remove(toRemove);
        //We just remove any abilityState object, because their values are assigned every update anyway.
        _abilityStates.RemoveAt(_abilityStates.Count - 1);

        //Remove the reference from the Slot->Ability map.
        foreach (var item in _abilitySlotMap)
        {
            if (item.Value == index)
                _abilitySlotMap[item.Key] = -1;
        }
    }

    //Stuff to make the syncList work...
    public class SyncListAbilityState : SyncListStruct<AbilityState> { }
    public struct AbilityState {
        public string name;
        public float cooldownPercent;
        public bool isActive;
        public bool canActivate;
        public bool isGarbage;
    }
}
