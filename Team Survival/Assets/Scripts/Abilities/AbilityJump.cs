using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float Cooldown = 0.5f;
    private const float JumpForce = 6;

    public override string Name { get { return "Jump"; } }

    public AbilityJump(IMotor caster, BaseUnit unit) : base(caster, unit, Cooldown) { }

    protected override bool CheckCanActivate()
    {
        return _caster.CalculateIsGrounded();
    }

    protected override void DoActivate()
    {
        _caster.AddForce(Vector3.up * JumpForce);
        _unit.TriggerAnimation(UnitTriggerAnimation.Jump);
    }
}
