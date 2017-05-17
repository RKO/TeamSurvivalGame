using UnityEngine;

public class UnitController : MonoBehaviour {
    private const float AttackRange = 2f;

    private Transform[] _waypoints;
    private int _waypointIndex;
    private Transform _currentWaypoint;

    private UnitShell _shell;
    private IMotor _motor;
    private UnitShell _enemyTarget;
    private Team _enemyTeam;

    [SerializeField]
    private GameObject NavigatorPrefab;
    private NavMeshAgent _navAgent;

    private void Start () {
        _shell = GetComponent<UnitShell>();

        //Disable this component on clients.
        if (!_shell.isServer)
        {
            this.enabled = false;
            return;
        }

        _shell.EventHandle.OnKill += OnUnitKill;
        _motor = _shell.Motor;

        GameObject navGo = Instantiate(NavigatorPrefab);
        _navAgent = navGo.GetComponent<NavMeshAgent>();
        _navAgent.transform.SetParent(transform.parent);
        _navAgent.enabled = true;
        ResetNavigator();

        if (_shell.CurrentTeam == Team.Enemies)
            _enemyTeam = Team.Players;
        else
            _enemyTeam = Team.Enemies;
    }

    public void SetPathWaypoints(Transform[] waypoints) {
        _waypoints = waypoints;
        _waypointIndex = 0;
    }
	
    private void Update()
    {
        //Only update on server
        if (_shell.isServer)
            ServerSideUpdate();
	}

    private void ServerSideUpdate() {
        if (_shell.AliveState != LifeState.Alive)
        {
            return;
        }

        if (_enemyTarget == null)
        {
            CheckForEnemyTargets();
        }

        UpdateNavigation();

        if (_enemyTarget != null)
        {
            AttackEnemy();
            return;
        }

        CheckWaypoint();
        if (_currentWaypoint == null)
        {
            _motor.Stop();
            ResetNavigator();
        }
    }

    private void UpdateNavigation()
    {
        const float maxDistance = 2f;
        float distance = Vector3.Distance(transform.position, _navAgent.transform.position);
        Vector3 direction = _navAgent.transform.position - transform.position;
        direction.y = 0;

        if (distance > 0) {
            _navAgent.speed = Mathf.Max(_shell.DefaultMoveSpeed * (maxDistance - distance), 0);
        }

        if (distance > _navAgent.radius)
        {
            float speed = _shell.DefaultMoveSpeed * (distance / maxDistance);
            _motor.SetMoveSpeed(speed);
            _motor.SetMoveDirection(direction);

            if (_navAgent.desiredVelocity != Vector3.zero)
            {
                _motor.SetRotateDestination(Quaternion.LookRotation(_navAgent.desiredVelocity));
            }
        }
        else {
            _motor.SetMoveDirection(Vector3.zero);
        }
    }

    private void OnUnitKill()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        _shell.EventHandle.OnKill -= OnUnitKill;
        Destroy(_navAgent.gameObject);
    }

    private void CheckForEnemyTargets() {
        var enemies = GameManager.Instance.UnitManager.GetUnits(_enemyTeam);

        foreach (var potentialEnemy in enemies)
        {
            if (Vector3.Distance(transform.position, potentialEnemy.Position) > 10)
                continue;
            else if(potentialEnemy.AliveState == LifeState.Alive)
            {
                _enemyTarget = potentialEnemy;
                break;
            }
        }
    }

    private void AttackEnemy() {
        if (_enemyTarget.AliveState != LifeState.Alive)
        {
            _enemyTarget = null;
            _currentWaypoint = null;
            return;
        }

        if (Vector3.Distance(transform.position, _enemyTarget.Position) > AttackRange)
        {
            _navAgent.SetDestination(_enemyTarget.Position);
        }
        else {
            Vector3 dir = _enemyTarget.Position - transform.position;
            dir.y = 0;
            ResetNavigator();
            _motor.Stop();

            _motor.SetRotateDestination(Quaternion.LookRotation(dir));
            var state = _shell.Abilities.GetAbilityState(AbilitySlot.Attack1);

            if (state.CanActivate)
                _shell.Abilities.ActivateAbility(AbilitySlot.Attack1);
        }
    }

    private void CheckWaypoint() {
        if (_waypoints.Length == 0 || _waypointIndex >= _waypoints.Length)
            return;

        if (_currentWaypoint == null)
        {
            _currentWaypoint = _waypoints[_waypointIndex];
            _navAgent.SetDestination(_currentWaypoint.position);
        }
        else if (Vector3.Distance(transform.position, _currentWaypoint.position) < 2)
        {
            _waypointIndex++;
            if (_waypointIndex < _waypoints.Length)
            {
                _currentWaypoint = _waypoints[_waypointIndex];
                _navAgent.SetDestination(_currentWaypoint.position);
            }
            else {
                _motor.Stop();
                ResetNavigator();
            }
        }
    }

    private void ResetNavigator() {
        _navAgent.transform.position = transform.position;
        _navAgent.SetDestination(transform.position);
    }
}