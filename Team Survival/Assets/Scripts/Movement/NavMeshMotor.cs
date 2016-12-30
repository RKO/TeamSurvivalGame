using UnityEngine;
using UnityEngine.Networking;
using System;

public class NavMeshMotor : NetworkBehaviour, IMotor
{
    private NavMeshAgent _agent;
    private GroundChecker _groundChecker;
    private float _moveSpeed;

    public Vector3 MoveDirection
    {
        get
        {
            return _agent.velocity;
        }
    }

    [Server]
    public void Initialize(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
        _agent.speed = moveSpeed;
    }

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = true;

        _groundChecker = new GroundChecker(transform);
    }

    [Server]
    public bool CalculateIsGrounded()
    {
        return _groundChecker.CalculateIsGrounded();
    }

    [ServerCallback]
    private void LateUpdate()
    {
        //Reset cached values.
        _groundChecker.ResetCache();
    }

    [Server]
    public void SetMoveDirection(Vector3 dir)
    {
        _agent.velocity = dir * _moveSpeed;
    }

    [Server]
    public void SetRotateDestination(Vector3 dir) {
        transform.rotation = Quaternion.LookRotation(dir);
    }

    [Server]
    public void SetMoveDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    [Server]
    public void AddForce(Vector3 force)
    {
        throw new NotImplementedException();
    }

    [Server]
    public void Stop()
    {
        _agent.Stop();
    }
}
