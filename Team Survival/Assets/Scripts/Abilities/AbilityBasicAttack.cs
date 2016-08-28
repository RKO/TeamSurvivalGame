using UnityEngine;

public class AbilityBasicAttack : BaseAbility
{
    private const float Cooldown = 0.5f;

    public override string Name { get { return "Attack"; } }

    public AbilityBasicAttack(BaseMotor caster) : base(caster, Cooldown) { }

    protected override void DoActivate()
    {
        
    }
}
