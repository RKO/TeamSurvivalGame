using System;
using UnityEngine;

public class BaseMotor : IMotor {
    private const float RotationSpeed = 5f;

    private Vector3 moveDirection;
    private Quaternion rotationTarget;
    private Vector3 addedForce;

    private Rigidbody myRigidbody;

    private GroundChecker _groundChecker;
    public Vector3 MoveDirection { get { return moveDirection; } }

    private float moveSpeed;

    private Transform _transform;

    //TODO This should not be available on the client.
    public Transform Body { get; private set; }
    public Transform Head { get; private set; }

    public void Initialize(Transform parent, float moveSpeed) {
        _transform = parent;
        this.moveSpeed = moveSpeed;

        myRigidbody = _transform.GetComponent<Rigidbody>();
        moveDirection = MathUtil.VectorZero;
        rotationTarget = Quaternion.Euler(_transform.rotation.eulerAngles);
        addedForce = MathUtil.VectorZero;

        _groundChecker = new GroundChecker(_transform);
    }

    public bool CalculateIsGrounded() {
        return _groundChecker.CalculateIsGrounded();
    }

    public void LateUpdate() {
        //Reset cached values.
        _groundChecker.ResetCache();
    }
	
	public void Update () {
        if (moveDirection != MathUtil.VectorZero)// || addedForce != Vector3.zero)
            Move();

        if(rotationTarget != MathUtil.QuatIdentity && rotationTarget != _transform.rotation)
            Rotate();

        if (addedForce != MathUtil.VectorZero)
            ProcessForces();

        myRigidbody.velocity = new Vector3(0, myRigidbody.velocity.y, 0);
	}

    private Vector3 _movement;
    private void Move() {
        
        _movement = moveDirection * Time.deltaTime;
        myRigidbody.MovePosition(_transform.position + _movement);
    }

    private void Rotate() {
        //Currently instant rotation.
        Quaternion rot = Quaternion.Lerp(myRigidbody.rotation, rotationTarget, Time.deltaTime * RotationSpeed);
        myRigidbody.MoveRotation(rot);
    }

    private void ProcessForces()
    {
        myRigidbody.AddForce(addedForce, ForceMode.Impulse);
        addedForce = MathUtil.VectorZero;
    }

    public void SetMoveDirection(Vector3 dir) {
        //Normalize the direction, as controllers might forget it. 
        //And apply speed, as it is used every frame, but dir is not changed very often.
        moveDirection = dir.normalized * moveSpeed;
    }

    public void SetRotateDestination(Vector3 dir)
    {
        rotationTarget = Quaternion.Euler(dir);
    }

    public void SetRotateDestination(Quaternion dir)
    {
        rotationTarget = dir;
    }

    public void AddForce(Vector3 force) {
        this.addedForce += force;
    }

    public void Stop()
    {
        moveDirection = MathUtil.VectorZero;
        rotationTarget = _transform.rotation;
        addedForce = MathUtil.VectorZero;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
