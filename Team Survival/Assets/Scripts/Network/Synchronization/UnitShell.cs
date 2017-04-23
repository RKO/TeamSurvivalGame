using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(AbilityList))]
public class UnitShell : NetworkBehaviour
{
    private UnitData _unitData;

    private AnimationSync _animationSync;

    public IMotor Motor { get; protected set; }

    public AbilityList Abilities { get; protected set; }

    public LifeState AliveState { get { return _aliveState; } }

    public Vector3 Position { get { return transform.position; } }

    public Team CurrentTeam { get { return _team; } }

    public Transform AbilityRoot { get { return abilityRootTransform; } }

    [SerializeField]
    private Transform Head;

    [SerializeField]
    private Transform Body;

    [SerializeField]
    private Transform abilityRootTransform;

    #region Stats
    [SyncVar]
    public string UnitID;

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

    [Server]
    public void Initialize(UnitData data) {
        _unitData = data;
        UnitID = _unitData.UnitID;
    }

    // Use this for initialization
    void Start() {
        _animationSync = GetComponent<AnimationSync>();
        Abilities = GetComponent<AbilityList>();

        if (this.isServer)
        {
            ServerSideSetup(_unitData);
        }

        GameManager.Instance.unitManager.AddUnit(this);

        if (this.isClient) {
            _unitData = UnitRegistry.GetUnitData(UnitID);
            if (_unitData.Model != null)
            {
                var model = Instantiate(_unitData.Model);
                model.transform.SetParent(Body, false);
            }

            if (HealthBarPrefab != null)
            {
                var obj = Instantiate(HealthBarPrefab, transform, false) as GameObject;
                obj.transform.localPosition = Head.localPosition + Vector3.up * 0.5f;
            }
        }
    }

    [Server]
    private void ServerSideSetup(UnitData unit) {
        if (GetComponent<NavMeshAgent>() != null)
            Motor = new NavMeshMotor();
        else
            Motor = new BaseMotor();

        Motor.Initialize(this.transform, unit.MoveSpeed);
        _animationSync.SetNewAnimation(UnitAnimation.Idle);

        //Initialize health from the model.
        Health = MaxHealth = unit.MaxHealth;

        _team = unit.DefaultTeam;

        foreach (var ability in unit.Abilities)
        {
            Abilities.GrantAbility(ability, this, transform);
        }
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
            else if (actualSpeed < _unitData.MoveSpeed * 0.5f)
                _animationSync.SetNewAnimation(UnitAnimation.Walking);
            else
                _animationSync.SetNewAnimation(UnitAnimation.Running);
        }

        Motor.Update();
    }

    [ServerCallback]
    void LateUpdate() {
        Motor.LateUpdate();
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

    [Server]
    public void SetIMotor(IMotor newMotor) {
        Motor.Stop();
        Motor = newMotor;
        Motor.Initialize(this.transform, _unitData.MoveSpeed);
    }

    private void OnDestroy() {
        GameManager.Instance.unitManager.RemoveUnit(this);
    }

    public delegate void OnKillDelegate();
    public OnKillDelegate OnKillCallback;
}
