using UnityEngine;

public class AbilitySweepingStrike : AbilityBasicAttack
{
    private bool _hasHitOnce;

    public override string GetUniqueID { get { return "AbilitySweepingStrike"; } }

    public AbilitySweepingStrike(UnitShell unit, AbilityInfo info) : base(unit, info)
    {
        _hitDelay = 0.3f;
    }

    protected override void DoActivate()
    {
        base.DoActivate();
        _hasHitOnce = false;
    }

    protected override void AbilityUpdate()
    {
        if (_done)
            return;

        if (_hitDelayTimer > 0)
        {
            _hitDelayTimer -= Time.deltaTime;
            return;
        }

        HitCheck();

        if (_hasHitOnce)
            _done = true;
        else {
            _hitTable.Clear();
            _hitDelayTimer = _hitDelay;
            _hasHitOnce = true;
        }
    }
}
