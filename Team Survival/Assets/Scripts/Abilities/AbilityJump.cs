using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float Cooldown = 0.5f;
    private const float JumpForce = 6;

    public override string Name { get { return "Jump"; } }

    public AbilityJump(BaseMotor caster, AnimationSync animSync) : base(caster, animSync, Cooldown) { }

    protected override bool CheckCanActivate()
    {
        return _caster.IsGrounded;
    }

    protected override void DoActivate()
    {
        _caster.AddForce(Vector3.up * JumpForce);
        _animSync.RpcTriggerAnimation(UnitTriggerAnimation.Jump);
    }

    protected override void CalculateCooldown()
    {
        if (_cooldownCounter > 0 && _caster.IsGrounded)
        {
            _cooldownCounter -= Time.deltaTime;
        }
    }
}
