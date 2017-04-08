using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BaseUnit))]
[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(AbilityList))]
[RequireComponent(typeof(IMotor))]
public class UnitShell : NetworkBehaviour {

    public Transform[] waypoints;
    
    private BaseUnit _unit;

    private AnimationSync _animationSync;

    public IMotor Motor { get; protected set; }

    public AbilityList Abilities { get; protected set; }

    public BaseUnit ChildUnit{ get { return _unit; } }

    public LifeState AliveState { get { return _aliveState; } }

    public Vector3 Position { get { return transform.position; } }

    public Team CurrentTeam { get { return _team; } }

    #region Stats
    [SyncVar]
    public int MaxHealth;

    [SyncVar]
    public float Health;

    [SyncVar]
    private LifeState _aliveState;

    [SyncVar]
    private Team _team;
    #endregion

    [SerializeField]
    private GameObject HealthBarPrefab;

    // Use this for initialization
    void Start() {
        _unit = GetComponentInChildren<BaseUnit>();
        GameManager.Instance.unitManager.AddUnit(this);

        _animationSync = GetComponent<AnimationSync>();
        Motor = GetComponent<IMotor>();
        Abilities = GetComponent<AbilityList>();

        if (this.isServer)
        {
            ServerSideSetup(_unit);
        }

        if (this.isClient) {
            if (HealthBarPrefab != null)
            {
                var obj = Instantiate(HealthBarPrefab, transform, false) as GameObject;
                obj.transform.localPosition = transform.FindChild("Head").localPosition + Vector3.up * 0.5f;
            }
        }
    }

    [Server]
    private void ServerSideSetup(BaseUnit unit) {
        //TODO Hardcoded way of giving orders to Units
        UnitController controller = GetComponent<UnitController>();
        if (controller != null)
        {
            controller.SetPathWaypoints(waypoints);
        }

        Motor.Initialize(_unit.MoveSpeed);
        _animationSync.SetNewAnimation(UnitAnimation.Idle);

        //Initialize health from the model.
        Health = MaxHealth = unit.MaxHealth;

        _team = unit.GetTeam;
    }

    [ServerCallback]
    void Update() {
        if (_aliveState == LifeState.Alive && Health <= 0) {
            Kill();
            return;
        }

        if (_aliveState == LifeState.Alive)
        {
            float actualSpeed = Motor.MoveDirection.magnitude;

            if (actualSpeed == 0)
                _animationSync.SetNewAnimation(UnitAnimation.Idle);
            else if (actualSpeed < _unit.MoveSpeed * 0.5f)
                _animationSync.SetNewAnimation(UnitAnimation.Walking);
            else
                _animationSync.SetNewAnimation(UnitAnimation.Running);
        }
    }

    [Server]
    public void DealDamage(float damage) {
        if (_aliveState == LifeState.Alive)
        {
            Health -= damage;
            if (Health < 0)
                Health = 0;
        }
    }

    [Server]
    private void Kill() {
        _aliveState = LifeState.Dying;

        _animationSync.SetNewAnimation(UnitAnimation.Dying);
        GetComponent<Collider>().enabled = false;
        var obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
            obstacle.enabled = false;

        Motor.Stop();

        GameManager.Instance.unitManager.KillUnit(this);

        if (OnKillCallback != null)
            OnKillCallback();

        RpcOnKill();
        StartCoroutine(Die());
    }

    [ClientRpc]
    private void RpcOnKill() {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.detectCollisions = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        GetComponent<Collider>().enabled = false;
        
        if(!isServer)
            GameManager.Instance.unitManager.KillUnit(this);
    }

    [Server]
    private IEnumerator Die() {
        yield return new WaitForSeconds(2);

        _aliveState = LifeState.Dead;
        _animationSync.SetNewAnimation(UnitAnimation.Dead);

        yield return new WaitForSeconds(10);

        NetworkServer.Destroy(this.gameObject);
    }

    [Server]
    public void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        _animationSync.TriggerAnimation(triggerAnim);
    }

    private void OnDestroy() {
        GameManager.Instance.unitManager.RemoveUnit(this);
    }

    public delegate void OnKillDelegate();
    public OnKillDelegate OnKillCallback;
}
