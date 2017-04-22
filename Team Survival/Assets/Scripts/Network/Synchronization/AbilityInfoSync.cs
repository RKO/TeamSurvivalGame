using UnityEngine;
using System.Collections.Generic;

public class AbilityInfoSync : MonoBehaviour {

    [SerializeField]
    private GameObject AbilitySyncPrefab;

    [SerializeField]
    private List<AbilityInfo> RegisteredAbilities;

    [SerializeField]
    private GameObject[] AbilityPrefabs;

    private static Dictionary<string, AbilityInfo> _abilityInfoMap;
    private static Dictionary<string, GameObject> _abilityPrefabMap;
    private static GameObject _abilitySyncPrefab;


    private void Awake()
    {
        _abilityInfoMap = new Dictionary<string, AbilityInfo>();
        _abilityPrefabMap = new Dictionary<string, GameObject>();
        _abilitySyncPrefab = AbilitySyncPrefab;

        foreach (var ability in AbilityPrefabs) {
            GameObject go = Instantiate(ability);
            BaseAbility ab = go.GetComponent<BaseAbility>();
            _abilityPrefabMap.Add(ab.GetUniqueID, ability);
            _abilityInfoMap.Add(ab.GetUniqueID, ab.GetInfo());
            Destroy(go);
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
}
