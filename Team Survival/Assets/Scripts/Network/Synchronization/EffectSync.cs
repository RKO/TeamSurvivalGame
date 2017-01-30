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
}
