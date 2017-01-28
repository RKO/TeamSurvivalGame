using UnityEngine.Networking;
using UnityEngine;

public class TransformSync : NetworkBehaviour {
    [SyncVar]
    public Vector3 _syncPosition;

    [SyncVar]
    public Vector3 _syncRotation;


    private void Update() {
        if (this.isServer)
            ServerUpdate();
        else
            ClientUpdate();

        ExtDebug.DrawBox(_syncPosition + Vector3.up, new Vector3(0.3f, 1, 0.3f), Quaternion.Euler(_syncRotation), Color.blue);
    }

    [Server]
    private void ServerUpdate()
    {
        if (transform.hasChanged)
        {
            _syncPosition = transform.position;
            _syncRotation = transform.rotation.eulerAngles;
        }
    }

    [Client]
    private void ClientUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _syncPosition, Time.deltaTime * 5);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_syncRotation), Time.deltaTime * 5);
    }
}
