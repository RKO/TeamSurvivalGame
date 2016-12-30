using System;
using UnityEngine;
using UnityEngine.Networking;

public class BaseMotor : NetworkBehaviour, IMotor {
    private Vector3 moveDirection;
    private Quaternion rotationTarget;
    private Vector3 addedForce;

    private Rigidbody myRigidbody;

    private GroundChecker _groundChecker;
    public Vector3 MoveDirection { get { return moveDirection; } }

    private float moveSpeed;

    public Transform Body { get; private set; }
    public Transform Head { get; private set; }

    [Server]
    public void Initialize(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
        moveDirection = MathUtil.VectorZero;
        rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles);
        addedForce = MathUtil.VectorZero;

        _groundChecker = new GroundChecker(transform);

        Body = transform.FindChild("Body");
        Head = transform.FindChild("Head");
    }

    [Server]
    public bool CalculateIsGrounded() {
        return _groundChecker.CalculateIsGrounded();
    }

    [ServerCallback]
    private void LateUpdate() {
        //Reset cached values.
        _groundChecker.ResetCache();
    }
	
    [ServerCallback]
	void Update () {
        if (moveDirection != MathUtil.VectorZero)// || addedForce != Vector3.zero)
            Move();

        if(rotationTarget != MathUtil.QuatIdentity && rotationTarget != transform.rotation)
            Rotate();

        if (addedForce != MathUtil.VectorZero)
            ProcessForces();
	}

    private Vector3 _movement;
    private void Move() {
        
        _movement = moveDirection * Time.deltaTime;
        myRigidbody.MovePosition(transform.position + _movement);
    }

    private void Rotate() {
        //Currently instant rotation.
        myRigidbody.MoveRotation(rotationTarget);
    }

    private void ProcessForces()
    {
        myRigidbody.AddForce(addedForce, ForceMode.Impulse);
        addedForce = MathUtil.VectorZero;
    }

    [Server]
    public void SetMoveDirection(Vector3 dir) {
        //Normalize the direction, as controllers might forget it. 
        //And apply speed, as it is used every frame, but dir is not changed very often.
        moveDirection = dir.normalized * moveSpeed;
    }

    [Server]
    public void SetRotateDestination(Vector3 dir)
    {
        rotationTarget = Quaternion.Euler(dir);
    }

    [Server]
    public void AddForce(Vector3 force) {
        this.addedForce += force;
    }

    public void SetMoveDestination(Vector3 destination)
    {
        //Nothing to do here.
    }

    public void Stop()
    {
        moveDirection = MathUtil.VectorZero;
        rotationTarget = transform.rotation;
        addedForce = MathUtil.VectorZero;
    }
}
