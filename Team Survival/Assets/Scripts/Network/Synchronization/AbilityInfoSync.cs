using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class AbilityInfoSync : MonoBehaviour {

    [SerializeField]
    private GameObject AbilitySyncPrefab;

    [SerializeField]
    private List<AbilityInfo> RegisteredAbilities;

    [SerializeField]
    private MonoBehaviour[] TestList;

    private static Dictionary<string, AbilityInfo> _abilityInfoMap;
    private static Dictionary<string, Type> _abilityMap;
    private static GameObject _abilitySyncPrefab;


    private void Awake()
    {
        _abilityInfoMap = new Dictionary<string, AbilityInfo>();
        _abilityMap = new Dictionary<string, Type>();
        _abilitySyncPrefab = AbilitySyncPrefab;

        var types = GetAbilityTypes();

        foreach (var objectType in types)
        {
            try {
                object obj = CreateInstanceOf(objectType, null, null);
                _abilityMap.Add((obj as BaseAbility).GetUniqueID, objectType);
            }
            catch (Exception e) {
                Debug.LogError(e.GetType().Name + "\n" + e.Message + e.StackTrace);
            }
            
        }

        foreach (var ability in RegisteredAbilities)
        {
            _abilityInfoMap.Add(ability.UniqueID, ability);
        }
    }

    public static AbilityInfo GetAbilityInfo(string uniqueID) {
        if (!_abilityInfoMap.ContainsKey(uniqueID))
            throw new KeyNotFoundException("No AbilityInfo registered for ID: \""+uniqueID+"\".");

        return _abilityInfoMap[uniqueID];
    }

    public static GameObject GetAbilitySyncPrefab() {
        return _abilitySyncPrefab;
    }

    public static BaseAbility GetAbilityFromID(AbilityInfo info, UnitShell unit) {
        return CreateInstanceOf(_abilityMap[info.UniqueID], unit, info) as BaseAbility;
    }

    private static IEnumerable<Type> GetAbilityTypes() {
        var type = typeof(BaseAbility);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

        return types;
    }

    private static object CreateInstanceOf(Type t, UnitShell unit, AbilityInfo info) {
        return Activator.CreateInstance(t, new object[] { unit, info });
    }
}
