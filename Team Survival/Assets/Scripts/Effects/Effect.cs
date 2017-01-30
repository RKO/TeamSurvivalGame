using UnityEngine;

public class Effect : ScriptableObject {
    public EffectId id;
    public ParticleSystem particle;

    public enum EffectId { MeleeHit }
}