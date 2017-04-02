using UnityEngine.Networking;

public class AbilitySynchronizer : NetworkBehaviour {

    [SyncVar]
    public string AbilityID;

    [SyncVar]
    public float CooldownPercent;

    [SyncVar]
    public bool IsAbilityActive;

    [SyncVar]
    public bool CanActivateAbility;

    public AbilityInfo GetAbilityInfo() {
        return AbilityInfoSync.GetAbilityInfo(AbilityID);
    }
}
