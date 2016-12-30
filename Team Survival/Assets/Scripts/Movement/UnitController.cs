using UnityEngine;

public class UnitController : MonoBehaviour {
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;

    private BaseUnit _unit;
    private IMotor _motor;
    private IUnit _enemyTarget;
    private Team _enemyTeam;

    private void Start () {
        _unit = GetComponent<BaseUnit>();
        _unit.OnKillCallback += OnUnitKill;
        _motor = _unit.Motor;

        _unit.Abilities.GrantAbility(new AbilityBasicAttack(_motor, _unit), AbilitySlot.Attack1);

        if (_unit.GetTeam == Team.Enemies)
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
        if (_unit.IsOnServer)
            ServerSideUpdate();
	}

    private void ServerSideUpdate() {
        if (_unit.Shell.AliveState != LifeState.Alive)
        {
            return;
        }

        if (_enemyTarget == null)
        {
            CheckForEnemyTargets();
        }


        if (_enemyTarget != null)
        {
            AttackEnemy();
            return;
        }

        CheckWaypoint();
        if (_currentWaypoint == null)
        {
            _motor.Stop();
        }
    }

    private void OnUnitKill()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void CheckForEnemyTargets() {
        var enemies = GameManager.Instance.unitManager.GetUnits(_enemyTeam);

        foreach (var potentialEnemy in enemies)
        {
            if (Vector3.Distance(transform.position, potentialEnemy.Position) > 10)
                continue;
            else if(potentialEnemy.Shell.AliveState == LifeState.Alive)
            {
                _enemyTarget = potentialEnemy;
                break;
            }
        }
    }

    private void AttackEnemy() {
        if (_enemyTarget.Shell.AliveState != LifeState.Alive)
        {
            _enemyTarget = null;
            _currentWaypoint = null;
            return;
        }

        if (Vector3.Distance(transform.position, _enemyTarget.Position) > 2)
        {
            _motor.SetMoveDestination(_enemyTarget.Position);
        }
        else {
            Vector3 dir = _enemyTarget.Position - transform.position;
            dir.y = 0;
            _motor.SetRotateDestination(dir);
            AbilityList.AbilityState state = _unit.Abilities.GetAbilityState(AbilitySlot.Attack1);

            if (!state.isGarbage && state.canActivate)
                _unit.Abilities.ActivateAbility(AbilitySlot.Attack1);
        }
    }

    private void CheckWaypoint() {
        if (_waypoints.Length == 0 || _waypointIndex >= _waypoints.Length)
            return;

        if (_currentWaypoint == null)
        {
            _currentWaypoint = _waypoints[_waypointIndex];
            _motor.SetMoveDestination(_currentWaypoint.position);
        }
        else if (Vector3.Distance(transform.position, _currentWaypoint.position) < 2)
        {
            _waypointIndex++;
            if (_waypointIndex < _waypoints.Length)
            {
                _currentWaypoint = _waypoints[_waypointIndex];
                _motor.SetMoveDestination(_currentWaypoint.position);
            }
            else {
                _motor.Stop();
            }
        }
    }
}