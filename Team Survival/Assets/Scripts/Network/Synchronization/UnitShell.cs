using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
[RequireComponent(typeof(AnimationSync))]
public class UnitShell : NetworkBehaviour {

    [SyncVar]
    public string UnitPrefabToLoad;

    public Transform[] waypoints;

	// Use this for initialization
	void Start () {
        if (string.IsNullOrEmpty(UnitPrefabToLoad))
        {
            Debug.LogWarning(this.GetType().ToString()+" on object \""+gameObject.name+"\" can't load model because no prefab path is set.");
            return;
        }

        GameObject model = Instantiate(Resources.Load(UnitPrefabToLoad)) as GameObject;
        model.transform.SetParent(this.transform, false);

        //Only for non-player units. 
        BaseUnit unit = model.GetComponent<BaseUnit>();
        if (unit != null)
        {
            unit.Initialize(GetComponent<BaseMotor>(), GetComponent<AbilityList>(), GetComponent<AnimationSync>(), this.isServer);

            //TODO Hardcoded way of giving orders...
            if (unit is UnitController)
            {
                (unit as UnitController).SetPathWaypoints(waypoints);
            }
        }
    }
}
