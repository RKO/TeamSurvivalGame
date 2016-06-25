using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float Cooldown = 0.5f;
    private const float JumpForce = 6;

    public AbilityJump(BaseUnit caster) : base(caster, Cooldown) { }

    protected override bool CanActivate()
    {
        return _caster.Motor.IsGrounded;
    }

    protected override void DoActivate()
    {
        _caster.Motor.AddForce(Vector3.up * JumpForce);
    }

    protected override void CalculateCooldown()
    {
        if (_cooldownCounter > 0 && _caster.Motor.IsGrounded)
        {
            _cooldownCounter -= Time.deltaTime;
        }
    }
}
