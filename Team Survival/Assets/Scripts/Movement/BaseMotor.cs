using System;
using UnityEngine;

public class BaseMotor : MonoBehaviour {
    private Vector3 moveDirection;
    private Vector3 eulerAngleTarget;
    private Vector3 addedForce;

    private float moveSpeed;
    private Rigidbody myRigidbody;

    private LayerMask terrainLayerMask;
    private bool grounded = false;

    public bool IsGrounded { get { return grounded; } }

    public Transform Body { get; private set; }
    public Transform Head { get; private set; }

    public void Initialize(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
        moveDirection = Vector3.zero;
        eulerAngleTarget = transform.rotation.eulerAngles;
        addedForce = Vector3.zero;

        terrainLayerMask = 1 << 8; //1 << 8;

        Body = transform.FindChild("Body");
        Head = transform.FindChild("Head");
    }

    private void RaycastToGround() {
        Ray[] rays = new Ray[5];

        const float rayLength = 1.0f;

        Vector3 pos = transform.position + Vector3.up * 0.25f;
        rays[0] = new Ray(pos, Vector3.down);
        rays[1] = new Ray(pos - transform.right * 0.25f + transform.forward * 0.25f, Vector3.down);
        rays[2] = new Ray(pos + transform.right * 0.25f + transform.forward * 0.25f, Vector3.down);
        rays[3] = new Ray(pos - transform.right * 0.25f - transform.forward * 0.25f, Vector3.down);
        rays[4] = new Ray(pos + transform.right * 0.25f - transform.forward * 0.25f, Vector3.down);

        for (int i = 0; i < rays.Length; i++)
        {
            Ray ray = rays[i];

            Debug.DrawRay(ray.origin, ray.direction, Color.blue);

            if (Physics.Raycast(ray, rayLength, terrainLayerMask)) //terrainLayerMask
            {
                grounded = true;
                return;
            }
        }

        grounded = false;
    }
	
	void FixedUpdate () {
        RaycastToGround();

        if (moveDirection != Vector3.zero)// || addedForce != Vector3.zero)
            Move();

        if(eulerAngleTarget != Vector3.zero)
            Rotate();

        if (addedForce != Vector3.zero)
            ProcessForces();
	}

    private void Move() {
        Vector3 movement = moveDirection * Time.deltaTime * moveSpeed;
        myRigidbody.MovePosition(transform.position + movement);
    }

    private void Rotate() {
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleTarget);
        myRigidbody.MoveRotation(deltaRotation);
    }

    private void ProcessForces()
    {
        myRigidbody.AddForce(addedForce, ForceMode.Impulse);
        addedForce = Vector3.zero;
    }

    public void SetMoveDirection(Vector3 dir) {
        moveDirection = dir;
    }

    public void SetRotateDestination(Vector3 dir)
    {
        eulerAngleTarget = dir;
    }

    public void AddForce(Vector3 force) {
        this.addedForce += force;
    }
}
