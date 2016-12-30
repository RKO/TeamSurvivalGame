using System;
using UnityEngine;

public class UnitController : MonoBehaviour {
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;

    private BaseUnit _unit;
    private IMotor _motor;

    private void Start () {
        _unit = GetComponent<BaseUnit>();
        _unit.OnKillCallback += OnUnitKill;
        _motor = _unit.Motor;

        _unit.Abilities.GrantAbility(new AbilityBasicAttack(_motor, _unit), AbilitySlot.Attack1);
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