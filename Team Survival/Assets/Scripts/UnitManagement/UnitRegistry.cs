using UnityEngine;
using System.Collections.Generic;

public class UnitRegistry : MonoBehaviour {

    [SerializeField]
    private List<UnitData> UnitDataToRegister;

    private static Dictionary<string, UnitData> _unitDataRegistry;


    public void Initialize() {
        _unitDataRegistry = new Dictionary<string, UnitData>();

        foreach (var item in UnitDataToRegister)
        {
            _unitDataRegistry.Add(item.UnitID, item);
        }
    }

    public static UnitData GetUnitData(string unitID) {

        if(!_unitDataRegistry.ContainsKey(unitID)) {
            Debug.LogError("UnitData not found for key: "+unitID);
            return null;
        }

        return _unitDataRegistry[unitID];
    }
}
