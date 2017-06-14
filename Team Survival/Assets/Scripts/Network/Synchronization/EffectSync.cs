using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

public class EffectSync : NetworkBehaviour {
    [SerializeField]
    private List<Effect> RegisteredEffects;

    private Dictionary<Effect.EffectId, Effect> _effectMap;

    private void Start() {
        _effectMap = new Dictionary<Effect.EffectId, Effect>();

        foreach (var effect in RegisteredEffects)
        {
            _effectMap.Add(effect.id, effect);
        }
    }

    [Server]
    public void TriggerEffect(Effect.EffectId effect, Vector3 position) {
        RpcTriggerEffect(effect, position);
    }

    [ClientRpc]
    private void RpcTriggerEffect(Effect.EffectId effectId, Vector3 position) {
        Effect e = _effectMap[effectId];

        Instantiate(e.particle, position, Quaternion.identity);
    }

    [Server]
    public void TriggerEffectOnTarget(Effect.EffectId effect, Vector3 localPosition, GameObject target)
    {
        RpcTriggerEffectOnTarget(effect, localPosition, target);
    }

    [ClientRpc]
    private void RpcTriggerEffectOnTarget(Effect.EffectId effectId, Vector3 localPosition, GameObject target)
    {
        Effect e = _effectMap[effectId];

        GameObject particle = Instantiate(e.particle, target.transform, false) as GameObject;
        particle.transform.localPosition = localPosition;

    }
}
