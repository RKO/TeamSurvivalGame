using UnityEngine;
using System.Collections.Generic;

public class AbilityBasicAttack : BaseAbility
{
    private const float Cooldown = 0f;
    private const float Duration = 1.1f;
    private const float MaxDistance = 0.5f;
    private Vector3 HalfBox = new Vector3(1f, 1f, 0.5f);
    private LayerMask targetMask = 1 << LayerMask.NameToLayer("Unit");

    private Dictionary<Transform, UnitShell> _hitTable;

    public override string Name { get { return "Attack"; } }

    public AbilityBasicAttack(BaseMotor caster, BaseUnit unit) : base(caster, unit, Cooldown, Duration) {
        _hitTable = new Dictionary<Transform, UnitShell>();
    }

    protected override void DoActivate()
    {
        _unit.TriggerAnimation(UnitTriggerAnimation.Attack1);
        _hitTable.Clear();
    }

    protected override void AbilityUpdate()
    {
        Vector3 inFront = _caster.transform.position + _caster.transform.forward + Vector3.up;

        RaycastHit[] hits = Physics.BoxCastAll(inFront, HalfBox, _caster.transform.forward, _caster.transform.rotation, MaxDistance, targetMask);
        foreach (var hit in hits)
        {
            Debug.DrawLine(_caster.transform.position + Vector3.up, hit.collider.transform.position + Vector3.up, Color.yellow);

            if (!_hitTable.ContainsKey(hit.transform)) {
                UnitShell unit = hit.transform.gameObject.GetComponentInChildren<UnitShell>();
                if (unit != null)
                {
                    DoHitOnTarget(unit);
                    _hitTable.Add(hit.transform, unit);
                }
                else {
                    Debug.LogError("Collider on \"" + hit.transform.gameObject.name + "\" in \"Unit\" layer does not have a BaseUnit component!");
                }
            }
        }
    }

    private void DoHitOnTarget(UnitShell target) {
        if (target.ChildUnit.GetTeam == Team.Enemies)
        {
            target.DealDamage(100);
        }
    }
}
