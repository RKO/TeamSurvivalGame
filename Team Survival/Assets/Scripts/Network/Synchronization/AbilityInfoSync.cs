using UnityEngine;
using System.Collections.Generic;

public class AbilityInfoSync : MonoBehaviour {

    [SerializeField]
    private List<AbilityInfo> RegisteredAbilities;

    private static Dictionary<string, AbilityInfo> _abilityMap;

    private void Awake()
    {
        _abilityMap = new Dictionary<string, AbilityInfo>();

        foreach (var ability in RegisteredAbilities)
        {
            _abilityMap.Add(ability.UniqueID, ability);
        }
    }

    public static AbilityInfo GetAbilityInfo(string uniqueID) {
        if (!_abilityMap.ContainsKey(uniqueID))
            throw new KeyNotFoundException("No AbilityInfo registered for ID: \""+uniqueID+"\".");

        return _abilityMap[uniqueID];
    }
}
