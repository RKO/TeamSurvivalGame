using UnityEngine;

[RequireComponent(typeof(BaseMotor))]
public class PlayerController : BaseUnit {
    private const float CameraRotationSpeed = 300;
    private const float JumpForce = 6;

    private Player _player;

    private BaseMotor motor;
    private GameObject cameraObj;
    private float rotationX = 0;

    private bool _initialized = false;

    public override Team GetTeam {
        get { return Team.Players; }
    }

    public void Initialize (GameObject cameraPrefab, Player player) {
        _player = player;

        motor = GetComponent<BaseMotor>();

        cameraObj = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SmoothMouseLook mouseLook = gameObject.AddComponent<SmoothMouseLook>();
        mouseLook.axes = SmoothMouseLook.RotationAxes.MouseX;
        mouseLook.sensitivityX = 7;

        _initialized = true;
    }

    // Update is called once per frame
    void Update() {
        if (_initialized)
        {
            SetMovement();
            ControlCamera();
        }
    }

    private void SetMovement() {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDir += (transform.forward * -1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir += (transform.right * -1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir += (transform.right);
        }

        _player.CmdSetMoveDir(moveDir);

        if (Input.GetKeyDown(KeyCode.Space) && motor.IsGrounded)
        {
            _player.CmdAddForce(Vector3.up * JumpForce);
        }
    }

    private void ControlCamera()
    {
        cameraObj.transform.position = motor.Head.position;

        //Calculate the y rotation (up/down)
        float rotationY = Input.GetAxis("Mouse Y") * CameraRotationSpeed * Time.deltaTime;
        float newRotY = cameraObj.transform.localEulerAngles.x - rotationY;
        newRotY = MathUtil.ClampAngle(newRotY, -35, 35);

        //And apply it with the saved x (left/right) rotation.
        cameraObj.transform.localEulerAngles = new Vector3(newRotY, rotationX, 0);
    }

    public void SetLookQuaternion(Quaternion q) {
        //Save the rotation locally, for the players camera
        rotationX = q.eulerAngles.y;

        //And send it to the server, for the actual object rotation.
        Vector3 rotation = new Vector3(0, rotationX, 0);
        _player.CmdSetRotateDestination(rotation);

        //Apply the desired rotation immediately so the user can see it, but the server will decide if it's correct.
        motor.transform.localRotation = q;
    }
}
