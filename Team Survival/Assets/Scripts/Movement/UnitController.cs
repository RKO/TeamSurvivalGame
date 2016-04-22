using UnityEngine;

[RequireComponent(typeof(BaseMotor))]
public class UnitController : MonoBehaviour {
    private const float MoveSpeed = 6;
    private BaseMotor _motor;
    private NavMeshPath _path;
    private Transform _goal;

    // Use this for initialization
    void Start () {
        _motor = GetComponent<BaseMotor>();
        _motor.Body.GetComponent<Renderer>().material.color = Color.red;

        _motor.Initialize(MoveSpeed);

        _goal = GameManager.Instance.goal;
        if (_goal == null)
            Debug.LogError("ERROR: No Goal transform found on GameManager!");
    }
	
	// Update is called once per frame
	void Update () {
        if (_path == null) {
            if(_motor.IsGrounded)
                _path = FindPath(transform.position, _goal.position);
            return;
        }
        
        for (int i = 1; i < _path.corners.Length; i++)
        {
            Vector3 start = _path.corners[i-1];
            Vector3 end = _path.corners[i];

            Debug.DrawLine(start, end, Color.yellow);
        }

        Stear();
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
        if (Vector3.Distance(transform.position, nextPoint) < 0.25)
        {
            _path = FindPath(nextPoint, _goal.position);
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