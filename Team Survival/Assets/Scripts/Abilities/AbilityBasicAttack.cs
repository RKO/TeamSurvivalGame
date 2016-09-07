using UnityEngine;

public class AbilityBasicAttack : BaseAbility
{
    private const float Cooldown = 0.5f;

    public override string Name { get { return "Attack"; } }

    public AbilityBasicAttack(BaseMotor caster, AnimationSync animSync) : base(caster, animSync, Cooldown) { }

    protected override void DoActivate()
    {
        _animSync.RpcTriggerAnimation(UnitTriggerAnimation.Attack1);
    }
}
