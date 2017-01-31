using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMotor : NetworkBehaviour, IMotor
{
    private NavMeshAgent _agent;
    private GroundChecker _groundChecker;
    private float _moveSpeed;

    public Transform Body { get; private set; }
    public Transform Head { get; private set; }

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

    //TODO This should not be available on the client.
    private void Start() {
        Body = transform.FindChild("Body");
        Head = transform.FindChild("Head");
    }

    [ServerCallback]
    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = true;

        _groundChecker = new GroundChecker(transform);
    }

    public override void OnStartClient() {
        //Disable on the client.
        if (!isServer)
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            enabled = false;
        }
    }

    [ServerCallback]
    private void Update() {
        Debug.DrawLine(transform.position, _agent.steeringTarget, Color.blue);
        Debug.DrawLine(transform.position, _agent.destination, Color.green);
        Debug.DrawRay(transform.position, _agent.desiredVelocity, Color.yellow);
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
