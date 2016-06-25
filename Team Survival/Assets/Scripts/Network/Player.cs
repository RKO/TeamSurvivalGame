using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    public const float MoveSpeed = 6;

    public GameObject CameraPrefab;

    public int PlayerID { get; private set; }

    private bool _initialized;

    [SyncVar]
    private NetworkIdentity _bodyIdentity;
    private PlayerController controller;
    private BaseMotor _motor;

    public void Initialize(int id, NetworkIdentity body) {
        PlayerID = id;
        _bodyIdentity = body;
    }

    void Start() {
        _motor = _bodyIdentity.gameObject.GetComponent<BaseMotor>();
    }

    void Update() {
        if (!isLocalPlayer)
            return;

        if (!_initialized)
        {
            _initialized = true;

            //Debug.LogWarning("Is local? " + isLocalPlayer);
            //if (isLocalPlayer)
            {
                controller = _bodyIdentity.gameObject.AddComponent<PlayerController>();
                controller.Initialize(CameraPrefab, this);
            }
        }
    }

    [Command]
    public void CmdSetMoveDir(Vector3 moveDir)
    {
        _motor.SetMoveDirection(moveDir);
    }

    [Command]
    public void CmdSetRotateDestination(Vector3 dir)
    {
        _motor.SetRotateDestination(dir);
    }

    [Command]
    public void CmdActivateAbility(int index)
    {
        controller.ActivateAbility(index);
    }
}
