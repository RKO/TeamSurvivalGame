using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
[RequireComponent(typeof(AnimationSync))]
public class UnitShell : NetworkBehaviour {

    public Transform[] waypoints;
    
    private BaseUnit _unit;
    
    public BaseUnit ChildUnit{ get { return _unit; } }

    public AnimationSync AnimationSync { get; private set; }

    public LifeState AliveState { get; private set; }

    // Use this for initialization
    void Start() {

        _unit = GetComponentInChildren<BaseUnit>();
        _unit.Initialize(this, GetComponent<BaseMotor>(), GetComponent<AbilityList>(), this.isServer);
        AnimationSync = GetComponent<AnimationSync>();

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
        _unit.UnitOnKill();
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
        _unit.UnitOnDeath();

        yield return new WaitForSeconds(10);

        NetworkServer.Destroy(this.gameObject);
    }
}
