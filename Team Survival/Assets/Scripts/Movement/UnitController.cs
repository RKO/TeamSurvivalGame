using UnityEngine;

public class UnitController : BaseUnit {
    private const float MoveSpeed = 6;
    private NavMeshPath _path;
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;
    public Animation _animator;

    public override Team GetTeam
    {
        get { return Team.Enemies; } //TODO be able to spawn friendly and neutral units.
    }

    public string UnitName; //Set from prefab. For now...
    public override string Name
    {
        get { return UnitName; }
    }

    // Use this for initialization
    void Start () {
        Motor.Initialize(MoveSpeed);

        _animator = GetComponentInChildren<Animation>();

        _animator.clip = _animator.GetClip("idle");
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

        ClientSideUpdate();
	}

    private void ClientSideUpdate() {
        switch (CurrentAnimation)
        {
            case UnitAnimation.Walking:
                _animator.Play("walk");
                break;
            case UnitAnimation.Running:
                _animator.Play("run");
                break;
            case UnitAnimation.Dying:
            case UnitAnimation.Dead:
                _animator.wrapMode = WrapMode.ClampForever;
                _animator.Play("death");
                break;
            //Should fall through down to default.
            case UnitAnimation.Idle:
                _animator.Play("combat_idle");
                break;
            default:
                _animator.Play("idle");
                break;
        }
    }

    private void ServerSideUpdate() {
        if (Shell.AliveState != LifeState.Alive)
        {
            _path = null;
            Motor.SetMoveDirection(Vector3.zero);
            Motor.SetRotateDestination(Vector3.zero);

            if(Shell.AliveState == LifeState.Dying)
                CurrentAnimation = UnitAnimation.Dying;
            else if(Shell.AliveState == LifeState.Dead)
                CurrentAnimation = UnitAnimation.Dead;

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
            CurrentAnimation = UnitAnimation.Idle;
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
        CurrentAnimation = UnitAnimation.Running;

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