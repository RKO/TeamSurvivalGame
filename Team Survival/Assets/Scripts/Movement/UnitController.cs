using UnityEngine;

public class UnitController : MonoBehaviour {
    private const float MoveSpeed = 6;
    private NavMeshPath _path;
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;

    private BaseUnit _unit;
    private BaseMotor _motor;

    private void Start () {
        _unit = GetComponent<BaseUnit>();
        _motor = _unit.Motor;

        _motor.Initialize(MoveSpeed);
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
            _path = null;
            return;
        }

        CheckWaypoint();

        if (_currentWaypoint == null)
        {
            _path = null;
        }

        if (_path == null)
        {
            if (_waypointIndex < _waypoints.Length && _motor.CalculateIsGrounded())
                _path = FindPath(transform.position, _waypoints[_waypointIndex].position);
        }

        /*if (_path != null)
        {
            //Debug lines
            for (int i = 1; i < _path.corners.Length; i++)
            {
                Vector3 start = _path.corners[i - 1];
                Vector3 end = _path.corners[i];

                Debug.DrawLine(start, end, Color.yellow);
            }
        }*/

        Stear();
    }

    private void CheckWaypoint() {
        if (_waypoints.Length == 0 || _waypointIndex >= _waypoints.Length)
            return;

        bool newPath = false;
        if (_currentWaypoint == null)
        {
            _currentWaypoint = _waypoints[_waypointIndex];
            newPath = true;
        }
        else if (Vector3.Distance(transform.position, _currentWaypoint.position) < 2)
        {
            _waypointIndex++;
            if (_waypointIndex < _waypoints.Length)
            {
                _currentWaypoint = _waypoints[_waypointIndex];
                newPath = true;
            }
            else {
                _path = null;
            }
        }

        if (newPath) {
            _path = FindPath(transform.position, _currentWaypoint.position);
        }
    }

    //Only called on server.
    private void Stear() {
        if (_path == null)
        {
            _motor.SetMoveDirection(Vector3.zero);
            return;
        }

        if (_path.corners.Length <= 1)
        {
            _path = null;
            return;
        }

        Vector3 nextPoint = _path.corners[1];
        //When the unit gets to the point, calculate a route to the next point.
        if (Vector3.Distance(transform.position, nextPoint) < 0.25)
        {
            _path = FindPath(nextPoint, _currentWaypoint.position);
            return;
        }


        Vector3 dir = nextPoint - transform.position;
        _motor.SetMoveDirection(dir);

        Quaternion rotation = Quaternion.LookRotation(dir);
        Vector3 rotDir = rotation.eulerAngles;

        //TODO Smooth rotation instead of instant.
        _motor.SetRotateDestination(new Vector3(0, rotDir.y, 0));
    }

    /*public override void UnitOnDeath() {
        SetNewAnimation(UnitAnimation.Dead);

        _path = null;
        Motor.SetMoveDirection(Vector3.zero);
        Motor.SetRotateDestination(Vector3.zero);
    }*/

    private static NavMeshPath FindPath(Vector3 startPoint, Vector3 endPoint) {
        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path);

        if (!pathFound)
            path = null;

        return path;
    }
}