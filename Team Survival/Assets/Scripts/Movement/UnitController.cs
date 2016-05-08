using UnityEngine;

[RequireComponent(typeof(BaseMotor))]
public class UnitController : MonoBehaviour {
    private const float MoveSpeed = 6;
    private BaseMotor _motor;
    private NavMeshPath _path;
    private Transform[] _waypoints;
    private int _waypointIndex;
    public Transform _currentWaypoint;

    // Use this for initialization
    void Start () {
        _motor = GetComponent<BaseMotor>();
        _motor.Body.GetComponent<Renderer>().material.color = Color.red;

        _motor.Initialize(MoveSpeed);
    }

    public void SetPathWaypoints(Transform[] waypoints) {
        _waypoints = waypoints;
        _waypointIndex = 0;
    }
	
	// Update is called once per frame
	void Update () {
        CheckWaypoint();

        if (_currentWaypoint == null)
        {
            _path = null;
            return;
        }

        if (_path == null) {
            if(_motor.IsGrounded)
                _path = FindPath(transform.position, _waypoints[_waypointIndex].position);
            return;
        }
        
        //Debug lines
        for (int i = 1; i < _path.corners.Length; i++)
        {
            Vector3 start = _path.corners[i-1];
            Vector3 end = _path.corners[i];

            Debug.DrawLine(start, end, Color.yellow);
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
            newPath = true;
            _waypointIndex++;
            _currentWaypoint = _waypoints[_waypointIndex];
        }

        if (newPath) {
            _path = FindPath(transform.position, _currentWaypoint.position);
        }
    }

    private void Stear() {
        if (_path == null)
            _motor.SetMoveDirection(Vector3.zero);

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

        Debug.DrawRay(_motor.Head.position, dir, Color.cyan);
        _motor.SetMoveDirection(dir);
    }

    private static NavMeshPath FindPath(Vector3 startPoint, Vector3 endPoint) {
        NavMeshPath path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, path);

        if (!pathFound)
            path = null;

        return path;
    }
}