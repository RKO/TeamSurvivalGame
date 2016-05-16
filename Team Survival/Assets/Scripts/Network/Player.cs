using UnityEngine;
using UnityEngine.Networking;


public class Player : NetworkBehaviour {
    public GameObject CameraPrefab;

    public int PlayerID { get; private set; }

    private bool _initialized;

    [SyncVar]
    private NetworkIdentity _body;

    public void Initialize(int id, NetworkIdentity body) {
        PlayerID = id;
        _body = body;
    }

    void Update() {
        if (_initialized)
            return;

        _initialized = true;

        Debug.LogWarning("Is local? " + isLocalPlayer);
        if (isLocalPlayer)
        {
            PlayerController controller = _body.gameObject.AddComponent<PlayerController>();
            controller.Initialize(CameraPrefab);
        }
    }
}
