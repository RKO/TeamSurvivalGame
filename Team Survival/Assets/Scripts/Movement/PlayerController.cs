using UnityEngine;

public class PlayerController : MonoBehaviour {
    private const float CameraRotationSpeed = 300;

    private Player _player;
    private UnitShell _unitShell;
    private AbilityList _abilities;
    private SmoothMouseLook _mouseLook;

    private GameObject cameraObj;
    private float rotationX = 0;

    private bool _initialized = false;

    private Vector3 _moveDir = Vector3.zero;

    private Transform Body { get; set; }
    private Transform Head { get; set; }

    public void PlayerInitialize (GameObject cameraPrefab, Player player) {
        _player = player;

        _unitShell = GetComponent<UnitShell>();
        _abilities = _unitShell.Abilities;

        cameraObj = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _mouseLook = gameObject.AddComponent<SmoothMouseLook>();
        _mouseLook.axes = SmoothMouseLook.RotationAxes.MouseX;
        _mouseLook.sensitivityX = 7;

        Body = transform.Find("Body");
        Head = transform.Find("Head");

        _initialized = true;
    }

    private void Update() {
        if (!_initialized)
            return;

        if (_unitShell.AliveState == LifeState.Alive)
        {
            SetMovement();
            ControlCamera();
        }
        else if (_unitShell.AliveState != LifeState.Alive && _mouseLook.enabled) {
            _mouseLook.enabled = false;
        }
    }

    private void SetMovement() {
        Vector3 newDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            newDir += transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            newDir += (transform.forward * -1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            newDir += (transform.right * -1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            newDir += (transform.right);
        }

        if (newDir != _moveDir)
        {
            _player.CmdSetMoveDir(newDir);
            _moveDir = newDir;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _player.CmdActivateAbilitySlot(AbilitySlot.Jump);
        }

        if (Input.GetMouseButtonDown(0))
        {
            _player.CmdActivateAbilitySlot(AbilitySlot.Attack1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _player.CmdActivateAbilitySlot(AbilitySlot.Attack2);
        }
    }

    void OnGUI() {
        if (!_initialized)
            return;

        const int size = 50;

        int count = 0;
        foreach (var state in _abilities.AbilitySynchronizers)
        {
            int x = 100 + (count * size * 2);
            int y = Screen.height - 10 - size;

            BaseAbility info = state.Ability;

            GUI.enabled = state.CanActivateAbility;
            {
                Rect rect = new Rect(x, y, size * 2, size);
                if(info.TempIcon != null)
                    GUI.DrawTexture(rect, info.TempIcon);

                GUI.Label(rect, info.DisplayName + ":\n" + state.CooldownPercent);
            }
            GUI.enabled = true;
            count++;
        }
    }

    private void ControlCamera()
    {
        cameraObj.transform.position = Head.position;

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

        //TODO Possiby limit how often this is sent.
        if (Mathf.Abs(_unitShell.transform.localRotation.eulerAngles.y - rotationX) > 0.001f)
        {
            //And send it to the server, for the actual object rotation.
            Vector3 rotation = new Vector3(0, rotationX, 0);
            _player.CmdSetRotateDestination(rotation);
        }

        //Apply the desired rotation immediately so the user can see it, but the server will decide if it's correct.
        _unitShell.transform.localRotation = q;
    }
}
