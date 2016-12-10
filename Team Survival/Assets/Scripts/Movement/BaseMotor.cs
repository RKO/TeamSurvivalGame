using UnityEngine;
using UnityEngine.Networking;

public class BaseMotor : NetworkBehaviour {
    private static LayerMask TerrainLayerMask = 1 << 8;
    private static Vector3 RayUp = Vector3.up * 0.25f;
    private static Vector3 RayDown = Vector3.down;
    private const float RayLength = 1.0f;

    private Vector3 moveDirection;
    private Quaternion rotationTarget;
    private Vector3 addedForce;

    public Rigidbody myRigidbody;

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
        moveDirection = MathUtil.VectorZero;
        rotationTarget = Quaternion.Euler(transform.rotation.eulerAngles);
        addedForce = MathUtil.VectorZero;

        Body = transform.FindChild("Body");
        Head = transform.FindChild("Head");
    }

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
        return Physics.Raycast(ray, RayLength, TerrainLayerMask);
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
}
