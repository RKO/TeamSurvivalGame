using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(AbilityList))]
public class UnitShell : NetworkBehaviour
{
    private UnitData _unitData;

    private AnimationSync _animationSync;

    private UnitEventHandle _clientEventHandle;

    public IMotor Motor { get; protected set; }

    public AbilityList Abilities { get; protected set; }

    public LifeState AliveState { get { return _aliveState; } }

    public Vector3 Position { get { return transform.position; } }

    public Team CurrentTeam { get { return _team; } }

    public Transform AbilityRoot { get { return abilityRootTransform; } }

    public float DefaultMoveSpeed { get { return _unitData.MoveSpeed; } }

    public UnitEventHandle EventHandle { get { return _clientEventHandle; } }

    public string UnitId { get { return _unitID; } }
    public float Health { get { return _health; } }
    public float MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    private Transform Head;

    [SerializeField]
    private Transform Body;

    [SerializeField]
    private Transform abilityRootTransform;

    #region Stats
    [SyncVar]
    private string _unitID;

    [SyncVar]
    private int _maxHealth;

    [SyncVar(hook = "OnHealthChanged")]
    private float _health;

    [SyncVar(hook = "OnLifeStateChanged")]
    private LifeState _aliveState;

    [SyncVar(hook = "OnTeamChanged")]
    private Team _team;
    #endregion

    [SerializeField]
    private GameObject HealthBarPrefab;

    [Server]
    public void Initialize(UnitData data) {
        _unitData = data;
        _unitID = _unitData.UnitID;
    }

    // Use this for initialization
    void Start() {
        _clientEventHandle = new UnitEventHandle();

        _animationSync = GetComponent<AnimationSync>();
        Abilities = GetComponent<AbilityList>();

        if (this.isServer)
        {
            ServerSideSetup(_unitData);
        }

        GameManager.Instance.unitManager.AddUnit(this);

        if (this.isClient) {
            _unitData = UnitRegistry.GetUnitData(_unitID);
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

            TriggerHooks();
        }
    }

    [Client]
    private void TriggerHooks() {
        OnTeamChanged(_team);
        OnLifeStateChanged(_aliveState);
    }

    [Server]
    private void ServerSideSetup(UnitData unit) {
        Motor = new BaseMotor();

        Motor.Initialize(this.transform, unit.MoveSpeed);
        _animationSync.SetNewAnimation(UnitAnimation.Idle);

        //Initialize health from the model.
        _health = _maxHealth = unit.MaxHealth;

        _team = unit.DefaultTeam;

        foreach (var ability in unit.Abilities)
        {
            Abilities.GrantAbility(ability, this, transform);
        }
    }

    [ServerCallback]
    void Update() {
        if (_aliveState == LifeState.Alive && _health <= 0) {
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
            _health -= damage;
            if (_health < 0)
                _health = 0;
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

        if (_clientEventHandle.OnKill != null)
            _clientEventHandle.OnKill();

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

    private void OnTeamChanged(Team newTeam) {
        _team = newTeam;
        if(_clientEventHandle.OnTeamChanged != null)
            _clientEventHandle.OnTeamChanged(_team);
    }

    private void OnLifeStateChanged(LifeState newState) {
        _aliveState = newState;
        if (_clientEventHandle.OnLifeStateChanged != null)
            _clientEventHandle.OnLifeStateChanged(_aliveState);
    }

    private void OnHealthChanged(float health) {
        _health = health;
        if (_clientEventHandle.OnHealthChanged != null)
            _clientEventHandle.OnHealthChanged(_health);
    }
}
