using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float Cooldown = 1f;
    private const float JumpForce = 6;
    private BaseUnit _caster;

    public AbilityJump() : base(Cooldown) { }

    protected override bool CanActivate(BaseUnit caster)
    {
        return caster.Motor.IsGrounded;
    }

    protected override void DoActivate(BaseUnit caster)
    {
        caster.Motor.AddForce(Vector3.up * JumpForce);
    }
}
