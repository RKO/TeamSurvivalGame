using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float JumpForce = 6;

    protected override bool CheckCanActivate()
    {
        return _unit.Motor.CalculateIsGrounded();
    }

    protected override void DoActivate()
    {
        _unit.Motor.AddForce(Vector3.up * JumpForce);
        _unit.TriggerAnimation(UnitTriggerAnimation.Jump);
    }
}
