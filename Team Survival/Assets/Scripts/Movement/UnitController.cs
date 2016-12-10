using UnityEngine;

public class UnitController : BaseUnit {
    private const float MoveSpeed = 6;
    private NavMeshPath _path;
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;

    public override Team GetTeam
    {
        get { return Team.Enemies; } //TODO be able to spawn friendly and neutral units.
    }

    public string UnitName; //Set from prefab. For now...
    public override string Name
    {
        get { return UnitName; }
    }

    void Start () {
        Motor.Initialize(MoveSpeed);
    }

    public void SetPathWaypoints(Transform[] waypoints) {
        _waypoints = waypoints;
        _waypointIndex = 0;
    }
	
    protected override void UnitUpdate()
    {
        //Only update on server
        if (IsOnServer)
            ServerSideUpdate();
	}

    private void ServerSideUpdate() {
        if (Shell.AliveState != LifeState.Alive)
        {
            _path = null;
            Motor.SetMoveDirection(Vector3.zero);
            Motor.SetRotateDestination(Vector3.zero);

            if (Shell.AliveState == LifeState.Dying)
            {
                UnitAnimator.SetBool("Dying", true);
            }
            else if (Shell.AliveState == LifeState.Dead)
            {
                UnitAnimator.SetBool("Dead", true);
            }

            return;
        }

        CheckWaypoint();

        if (_currentWaypoint == null)
        {
            _path = null;
        }

        if (_path == null)
        {
            if (Motor.IsGrounded && _waypointIndex < _waypoints.Length)
                _path = FindPath(transform.position, _waypoints[_waypointIndex].position);
        }

        if (_path != null)
        {
            //Debug lines
            for (int i = 1; i < _path.corners.Length; i++)
            {
                Vector3 start = _path.corners[i - 1];
                Vector3 end = _path.corners[i];

                Debug.DrawLine(start, end, Color.yellow);
            }
        }

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

    private void Stear() {
        if (_path == null)
        {
            Motor.SetMoveDirection(Vector3.zero);
            //Idle
            UnitAnimator.SetInteger("Speed", 0);
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
        Motor.SetMoveDirection(dir);
        //Running
        UnitAnimator.SetInteger("Speed", 2);

        Quaternion rotation = Quaternion.LookRotation(dir);
        Vector3 rotDir = rotation.eulerAngles;

        //TODO Smooth rotation instead of instant.
        Motor.SetRotateDestination(new Vector3(0, rotDir.y, 0));
    }

    private static NavMeshPath FindPath(Vector3 startPoint, Vector3 endPoint) {
        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path);

        if (!pathFound)
            path = null;

        return path;
    }
}