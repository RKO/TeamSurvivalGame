using UnityEngine;
using UnityEngine.Networking;

public class BaseMotor : NetworkBehaviour {
    private Vector3 moveDirection;
    private Vector3 eulerAngleTarget;
    private Vector3 addedForce;

    public Rigidbody myRigidbody;
    private LayerMask terrainLayerMask;

    private bool _cachedIsGrounded = false;
    private bool _hasIsGroundedCache = false;

    [SyncVar]
    private float moveSpeed;

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

    private static Vector3 RayUp = Vector3.up * 0.25f;
    private static Vector3 RayDown = Vector3.down;
    private const float RayLength = 1.0f;

    public bool CalculateIsGrounded() {
        if (_hasIsGroundedCache)
            return _cachedIsGrounded;

        Vector3 pos = transform.position + RayUp;
        Vector3 right = transform.right * 0.25f;
        Vector3 forward = transform.forward * 0.25f;

        bool grounded = CastRay(new Ray(pos - right + forward, RayDown));
        if(!grounded)
            grounded = CastRay(new Ray(pos + right + forward, RayDown));
        if (!grounded)
            grounded = CastRay(new Ray(pos - right - forward, RayDown));
        if (!grounded)
            grounded = CastRay(new Ray(pos + right - forward, RayDown));

        _cachedIsGrounded = grounded;
        _hasIsGroundedCache = true;

        return grounded;
    }

    private void LateUpdate() {
        //Reset cached values.
        _cachedIsGrounded = false;
        _hasIsGroundedCache = false;
    }

    private bool CastRay(Ray ray) {
        return Physics.Raycast(ray, RayLength, terrainLayerMask);
    }
	
    [ServerCallback]
	void FixedUpdate () {
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

    [Server]
    public void SetMoveDirection(Vector3 dir) {
        //Normalize the direction, as controllers might forget it.
        moveDirection = dir.normalized;
    }

    [Server]
    public void SetRotateDestination(Vector3 dir)
    {
        eulerAngleTarget = dir;
    }

    [Server]
    public void AddForce(Vector3 force) {
        this.addedForce += force;
    }
}
