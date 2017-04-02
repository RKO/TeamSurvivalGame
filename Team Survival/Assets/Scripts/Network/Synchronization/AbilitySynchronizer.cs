using UnityEngine;
using UnityEngine.Networking;

public class AbilitySynchronizer : NetworkBehaviour {

    [SyncVar]
    public NetworkIdentity ParentObject;

    [SyncVar]
    public string AbilityID;

    [SyncVar]
    public float CooldownPercent;

    [SyncVar]
    public bool IsAbilityActive;

    [SyncVar]
    public bool CanActivateAbility;

    private void Start() {
        if (transform.parent == null && ParentObject != null)
        {
            transform.SetParent(ParentObject.transform);
        }
    }

    public AbilityInfo GetAbilityInfo() {
        return AbilityInfoSync.GetAbilityInfo(AbilityID);
    }
}
