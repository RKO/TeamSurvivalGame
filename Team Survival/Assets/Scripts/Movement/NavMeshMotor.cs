using UnityEngine;
using System;

public class NavMeshMotor : IMotor
{
    private NavMeshAgent _agent;
    private GroundChecker _groundChecker;
    private float _moveSpeed;
    private Transform _transform;

    public Transform Body { get; private set; }
    public Transform Head { get; private set; }

    public Vector3 MoveDirection
    {
        get
        {
            return _agent.velocity;
        }
    }

    public void Initialize(Transform parent, float moveSpeed)
    {
        _transform = parent;
        _moveSpeed = moveSpeed;

        _agent = parent.GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;

        _agent = _transform.GetComponent<NavMeshAgent>();
        _agent.updateRotation = true;
        //Should be disabled by default, so it doesn't run on clients.
        _agent.enabled = true;

        _groundChecker = new GroundChecker(_transform);
    }

    public bool CalculateIsGrounded()
    {
        return _groundChecker.CalculateIsGrounded();
    }

    public void Update()
    {
        Debug.DrawLine(_transform.position, _agent.steeringTarget, Color.blue);
        Debug.DrawLine(_transform.position, _agent.destination, Color.green);
        Debug.DrawRay(_transform.position, _agent.desiredVelocity, Color.yellow);
    }

    public void LateUpdate()
    {
        //Reset cached values.
        _groundChecker.ResetCache();
    }

    public void SetMoveDirection(Vector3 dir)
    {
        _agent.velocity = dir * _moveSpeed;
    }

    public void SetRotateDestination(Vector3 dir) {
        _transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetMoveDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    public void AddForce(Vector3 force)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        _agent.Stop();
    }
}
