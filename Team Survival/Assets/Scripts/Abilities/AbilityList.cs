using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class AbilityList : NetworkBehaviour {

    private List<BaseAbility> _abilities;
    public SyncListAbilityState _abilityStates;

    private Dictionary<AbilitySlot, int> _abilitySlotMap;
    private Dictionary<AbilitySlot, AbilitySynchronizer> _abilitySyncs;

    private List<AbilitySynchronizer> _clientSideSynchronizers;

    public List<AbilitySynchronizer> AbilitySynchronizers { get { return _clientSideSynchronizers; } }

    void Awake() {
        _abilities = new List<BaseAbility>();
        _abilityStates = new SyncListAbilityState();
        _abilitySlotMap = new Dictionary<AbilitySlot, int>();
        _abilitySyncs = new Dictionary<AbilitySlot, AbilitySynchronizer>();
        _clientSideSynchronizers = new List<AbilitySynchronizer>();

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

            AbilitySlot slot = AbilitySlot.None;
            foreach (var item in _abilitySlotMap)
            {
                if (item.Value == i)
                {
                    slot = item.Key;
                    break;
                }
            }
                    AbilitySynchronizer sync;
            _abilitySyncs.TryGetValue(slot, out sync);
            if (sync != null) {
                sync.CooldownPercent = ability.CooldownPercent;
                sync.IsAbilityActive = ability.IsActive;
                sync.CanActivateAbility = ability.CanActivate;
            }
        }
    }

    [Server]
    public void GrantAbility(BaseAbility newAbility, AbilitySlot slot, Transform syncParent = null)
    {
        _abilities.Add(newAbility);
        _abilityStates.Add(new AbilityState());

        //It is possible to overwrite assigned abilities.
        if (slot != AbilitySlot.None) {
            _abilitySlotMap[slot] = _abilities.IndexOf(newAbility);

            if (syncParent != null) {
                AbilitySynchronizer sync;

                _abilitySyncs.TryGetValue(slot, out sync);
                if (sync == null) {
                    GameObject go = Instantiate(AbilityInfoSync.GetAbilitySyncPrefab());
                    NetworkServer.Spawn(go);
                    go.transform.SetParent(syncParent);
                    sync = go.GetComponent<AbilitySynchronizer>();
                    _abilitySyncs.Add(slot, sync);

                    RpcSynchronizerCreated(go.GetComponent<NetworkIdentity>());
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
            {
                _abilitySlotMap[item.Key] = -1;

                if (_abilitySyncs.ContainsKey(item.Key))
                {
                    var sync = _abilitySyncs[item.Key];
                    _abilitySyncs.Remove(item.Key);
                    Destroy(sync.gameObject);
                }
                break;
            }
        }
    }

    [ClientRpc]
    private void RpcSynchronizerCreated(NetworkIdentity id) {
        Debug.Log("ID: "+id);
        _clientSideSynchronizers.Add(id.GetComponent<AbilitySynchronizer>());
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
