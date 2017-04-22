﻿using System;
using UnityEngine;

public class AbilityJump : BaseAbility
{
    private const float Cooldown = 0.5f;
    private const float JumpForce = 6;

    public override string GetUniqueID { get { return "AbilityJumpStandard"; } }

    public AbilityJump(UnitShell unit, AbilityInfo info) : base(unit, info) { }

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
