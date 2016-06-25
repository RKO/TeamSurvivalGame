﻿using UnityEngine;

public class PlayerController : BaseUnit {
    private const float CameraRotationSpeed = 300;

    private Player _player;

    private GameObject cameraObj;
    private float rotationX = 0;

    private bool _initialized = false;

    public override Team GetTeam {
        get { return Team.Players; }
    }

    public void Initialize (GameObject cameraPrefab, Player player) {
        _player = player;

        cameraObj = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SmoothMouseLook mouseLook = gameObject.AddComponent<SmoothMouseLook>();
        mouseLook.axes = SmoothMouseLook.RotationAxes.MouseX;
        mouseLook.sensitivityX = 7;

        GrantAbility(new AbilityJump(this));

        _initialized = true;
    }

    protected override void UnitUpdate() {
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

        if (Input.GetKey(KeyCode.Space))
        {
            _player.CmdActivateAbility(0);
        }
    }

    private void ControlCamera()
    {
        cameraObj.transform.position = Motor.Head.position;

        //Don't rotate the camera, if GUI is open.
        if (!GameManager.Instance.IsGUIOpen)
        {
            //Calculate the y rotation (up/down)
            float rotationY = Input.GetAxis("Mouse Y") * CameraRotationSpeed * Time.deltaTime;
            float newRotY = cameraObj.transform.localEulerAngles.x - rotationY;
            newRotY = MathUtil.ClampAngle(newRotY, -35, 35);

            //And apply it with the saved x (left/right) rotation.
            cameraObj.transform.localEulerAngles = new Vector3(newRotY, rotationX, 0);
        }
    }

    public void SetLookQuaternion(Quaternion q) {
        //Save the rotation locally, for the players camera
        rotationX = q.eulerAngles.y;

        //And send it to the server, for the actual object rotation.
        Vector3 rotation = new Vector3(0, rotationX, 0);
        _player.CmdSetRotateDestination(rotation);

        //Apply the desired rotation immediately so the user can see it, but the server will decide if it's correct.
        Motor.transform.localRotation = q;
    }
}
