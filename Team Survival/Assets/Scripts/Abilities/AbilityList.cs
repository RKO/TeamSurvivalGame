using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class AbilityList : NetworkBehaviour {

    private List<BaseAbility> _abilities;

    private Dictionary<AbilitySlot, int> _abilitySlotMap;
    private Dictionary<AbilitySlot, AbilitySynchronizer> _abilitySyncs;
    private List<AbilitySynchronizer> _clientSynchronizerObjects;

    public List<AbilitySynchronizer> AbilitySynchronizers { get { return _clientSynchronizerObjects; } }

    void Awake() {
        _abilities = new List<BaseAbility>();
        _abilitySlotMap = new Dictionary<AbilitySlot, int>();
        _abilitySyncs = new Dictionary<AbilitySlot, AbilitySynchronizer>();
        _clientSynchronizerObjects = new List<AbilitySynchronizer>();

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
            ability.RunUpdate();

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
    public void GrantAbility(GameObject abilityPrefab, UnitShell shell, Transform syncParent)
    {
        GameObject abilityInstance = Instantiate(abilityPrefab, shell.AbilityRoot, false) as GameObject;
        BaseAbility newAbility = abilityInstance.GetComponent<BaseAbility>();
        newAbility.Setup(shell);
        AbilitySlot slot = newAbility.Slot;

        _abilities.Add(newAbility);

        //It is possible to overwrite assigned abilities.
        if (slot != AbilitySlot.None)
        {
            _abilitySlotMap[slot] = _abilities.IndexOf(newAbility);

            if (syncParent != null)
            {
                AbilitySynchronizer sync;

                _abilitySyncs.TryGetValue(slot, out sync);
                if (sync == null)
                {
                    GameObject go = Instantiate(AbilityInfoSync.GetAbilitySyncPrefab(), shell.AbilityRoot, false) as GameObject;
                    NetworkServer.Spawn(go);
                    sync = go.GetComponent<AbilitySynchronizer>();
                    //On the clients, they will just be children of the root object.
                    sync.ParentObject = syncParent.GetComponent<NetworkIdentity>();
                    _abilitySyncs.Add(slot, sync);

                    RpcSynchronizerCreated(go.GetComponent<NetworkIdentity>());
                }

                sync.SetOnServer(newAbility);
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

    [Server]
    public BaseAbility GetAbilityState(AbilitySlot slot)
    {
        int index = _abilitySlotMap[slot];
        return _abilities[index];
    }

    [Server]
    public void RemoveAbility(BaseAbility toRemove)
    {
        int index = _abilities.IndexOf(toRemove);
        _abilities.Remove(toRemove);

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
    private void RpcSynchronizerCreated(NetworkIdentity syncObject) {
        _clientSynchronizerObjects.Add(syncObject.GetComponent<AbilitySynchronizer>());
    }
}
