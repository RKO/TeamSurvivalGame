using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private const float MoveSpeed = 6;
    //private const float RotationSpeed = 270;
    private const float CameraRotationSpeed = 300;
    private const float JumpForce = 6;

    public GameObject cameraPrefab;

    private BaseMotor motor;
    private GameObject cameraObj;
    

	// Use this for initialization
	void Start () {
        motor = GetComponent<BaseMotor>();
        motor.Initialize(MoveSpeed);

        Transform headTrans = this.transform.FindChild("Head");
        cameraObj = GameObject.Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        cameraObj.transform.SetParent(headTrans, false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SmoothMouseLook mouseLook = gameObject.AddComponent<SmoothMouseLook>();
        mouseLook.axes = SmoothMouseLook.RotationAxes.MouseX;
        mouseLook.sensitivityX = 7;
    }

    // Update is called once per frame
    void Update() {
        SetMovement();
        //SetRotation();
        ControlCamera();
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

        motor.SetMoveDirection(moveDir);

        if (Input.GetKeyDown(KeyCode.Space) && motor.IsGrounded)
        {
            motor.AddForce(Vector3.up * JumpForce);
        }
    }

    /*private void SetRotation() {
        Vector3 rotationVel = transform.rotation.eulerAngles;

        if (Input.GetAxis("Mouse X") > 0)
            rotationVel += Vector3.up * RotationSpeed * Time.deltaTime;
        if (Input.GetAxis("Mouse X") < 0)
            rotationVel += Vector3.up * -RotationSpeed * Time.deltaTime;

        motor.SetRotateDestination(rotationVel);
    }*/

    private void ControlCamera()
    {
        float rotationY = Input.GetAxis("Mouse Y") * CameraRotationSpeed * Time.deltaTime;

        float newRotY = cameraObj.transform.localEulerAngles.x - rotationY;
        newRotY = MathUtil.ClampAngle(newRotY, -35, 35);
        cameraObj.transform.localEulerAngles = new Vector3(newRotY, 0, 0);
    }

    public void SetLookQuaternion(Quaternion q) {
        Vector3 rot = q.eulerAngles;

        Vector3 rotation = new Vector3(0, rot.y, 0);
        motor.SetRotateDestination(rotation);
    }
}
