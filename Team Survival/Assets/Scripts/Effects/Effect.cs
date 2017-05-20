using UnityEngine;

public class Effect : ScriptableObject {
    public EffectId id;
    public GameObject particle;

    public enum EffectId { MeleeHit, Conversion }
}