using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
[RequireComponent(typeof(AnimationSync))]
public class UnitShell : NetworkBehaviour {

    [SyncVar]
    public string UnitPrefabToLoad;

    public Transform[] waypoints;

    private BaseUnit _unit;
    public BaseUnit ChildUnit{ get { return _unit; } }

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
        _unit = model.GetComponent<BaseUnit>();
        if (_unit == null)
        {
            Debug.LogError("No BaseUnit component on spawned model \""+ UnitPrefabToLoad + "\"!");
            return;
        }

        _unit.Initialize(this, GetComponent<BaseMotor>(), GetComponent<AbilityList>(), GetComponent<AnimationSync>(), this.isServer);

        if (this.isServer) {
            ServerSideSetup(_unit);
        }
    }

    [Server]
    private void ServerSideSetup(BaseUnit unit) {
        //TODO Hardcoded way of giving orders to Units
        if (unit is UnitController)
        {
            (unit as UnitController).SetPathWaypoints(waypoints);
        }

        //Initialize health from the model.
        Health = MaxHealth = unit.MaxHealth;
    }

    [ServerCallback]
    void Update() {
        if (AliveState == LifeState.Alive && Health <= 0) {
            Kill();
        }
    }

    #region Stats
    [SyncVar]
    public int MaxHealth;

    [SyncVar]
    public float Health;

    [SyncVar]
    public LifeState AliveState;

    #endregion

    [Server]
    public void DealDamage(float damage) {
        if (AliveState == LifeState.Alive)
        {
            Health -= damage;
            if (Health < 0)
                Health = 0;
        }
    }

    [Server]
    private void Kill() {
        AliveState = LifeState.Dying;
        RpcOnKill();
        StartCoroutine(Die());
    }

    [ClientRpc]
    private void RpcOnKill() {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.detectCollisions = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        GetComponent<Collider>().enabled = false;
    }

    [Server]
    private IEnumerator Die() {
        yield return new WaitForSeconds(2);

        AliveState = LifeState.Dead;

        yield return new WaitForSeconds(10);

        NetworkServer.Destroy(this.gameObject);
    }
}
