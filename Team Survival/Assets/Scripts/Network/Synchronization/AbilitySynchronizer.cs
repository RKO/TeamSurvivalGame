using UnityEngine.Networking;

public class AbilitySynchronizer : NetworkBehaviour {

    [SyncVar]
    public string AbilityID;

    [SyncVar]
    public float cooldownPercent;

    [SyncVar]
    public bool isActive;

    [SyncVar]
    public bool canActivate;
}
