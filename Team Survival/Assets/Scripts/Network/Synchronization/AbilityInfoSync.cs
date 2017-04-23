using UnityEngine;
using System.Collections.Generic;

public class AbilityInfoSync : MonoBehaviour {

    [SerializeField]
    private GameObject AbilitySyncPrefab;

    [SerializeField]
    private GameObject[] AbilityPrefabs;

    private static Dictionary<string, GameObject> _abilityPrefabMap;
    private static GameObject _abilitySyncPrefab;

    private void Awake()
    {
        _abilityPrefabMap = new Dictionary<string, GameObject>();
        _abilitySyncPrefab = AbilitySyncPrefab;

        foreach (var ability in AbilityPrefabs) {
            GameObject go = Instantiate(ability);
            BaseAbility ab = go.GetComponent<BaseAbility>();
            _abilityPrefabMap.Add(ab.UniqueID, ability);
            Destroy(go);
        }
    }

    public static GameObject GetAbilitySyncPrefab() {
        return _abilitySyncPrefab;
    }

    public static GameObject GetAbilityPrefab(string uniqueID)
    {
        return _abilityPrefabMap[uniqueID];
    }
}
