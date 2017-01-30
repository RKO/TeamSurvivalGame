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

    public AbilityBasicAttack(IMotor caster, UnitShell unit) : base(caster, unit, Cooldown, Duration) {
        _hitTable = new Dictionary<Transform, UnitShell>();
    }

    protected override void DoActivate()
    {
        _unit.TriggerAnimation(UnitTriggerAnimation.Attack1);
        _hitTable.Clear();
    }

    protected override void AbilityUpdate()
    {
        Transform transform = _unit.transform;
        Vector3 inFront = transform.position + transform.forward + Vector3.up;

        ExtDebug.DrawBox(inFront, HalfBox, transform.rotation, Color.red);

        RaycastHit[] hits = Physics.BoxCastAll(inFront, HalfBox, transform.forward, transform.rotation, MaxDistance, targetMask);
        
        foreach (var hit in hits)
        {
            Debug.DrawLine(transform.position + Vector3.up, hit.collider.transform.position + Vector3.up, Color.yellow);

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
        if (target.ChildUnit.GetTeam != _unit.ChildUnit.GetTeam)
        {
            GameManager.Instance.EffectManager.TriggerEffect(Effect.EffectId.MeleeHit, target.transform.position + Vector3.up);
            target.DealDamage(10);
        }
    }
}
