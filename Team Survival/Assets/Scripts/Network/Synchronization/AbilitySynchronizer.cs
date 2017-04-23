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

    private BaseAbility _ability;
    public BaseAbility Ability { get { return _ability; } }

    private void Start()
    {
        if (transform.parent == null && ParentObject != null)
        {
            transform.SetParent(ParentObject.transform);
        }

        if (!isServer)
        {
            GameObject go = Instantiate(AbilityInfoSync.GetAbilityPrefab(AbilityID), transform, false) as GameObject;
            _ability = go.GetComponent<BaseAbility>();
        }
    }

    [Server]
    public void SetOnServer(BaseAbility ability)
    {
        _ability = ability;
        AbilityID = _ability.UniqueID;
    }
}
