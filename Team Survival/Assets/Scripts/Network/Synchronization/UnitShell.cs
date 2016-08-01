using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
public class UnitShell : NetworkBehaviour {

    [SyncVar]
    public string UnitPrefabToLoad;

    public Transform[] waypoints;

	// Use this for initialization
	void Start () {
        GameObject model = Instantiate(Resources.Load(UnitPrefabToLoad)) as GameObject;
        model.transform.SetParent(this.transform, false);

        BaseUnit unit = model.GetComponent<BaseUnit>();
        unit.Initialize(GetComponent<BaseMotor>(), GetComponent<AbilityList>(), this.isServer);

        //TODO Hardcoded way of giving orders...
        if (unit is UnitController)
        {
            (unit as UnitController).SetPathWaypoints(waypoints);
        }
    }
}
