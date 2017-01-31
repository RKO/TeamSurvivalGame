using UnityEngine;
using System.Collections.Generic;

public class AbilityBasicAttack : BaseAbility
{
    private const float Damage = 5f;
    private const float Cooldown = 0.1f;
    private const float MaxDistance = 0.5f;
    private Vector3 HalfBox = new Vector3(1f, 1f, 0.5f);
    private LayerMask targetMask = 1 << LayerMask.NameToLayer("Unit");

    protected Dictionary<Transform, UnitShell> _hitTable;
    private float _animationDuration;
    protected float _hitDelay;
    protected float _hitDelayTimer;
    protected bool _done;

    public override string Name { get { return "Attack"; } }

    public AbilityBasicAttack(IMotor caster, UnitShell unit, float animationDuration, float hitDelay) : base(caster, unit, Cooldown, animationDuration) {
        _hitDelay = hitDelay;
        _hitTable = new Dictionary<Transform, UnitShell>();
    }

    protected override void DoActivate()
    {
        _unit.TriggerAnimation(UnitTriggerAnimation.Attack1);
        _hitTable.Clear();
        _hitDelayTimer = _hitDelay;
        _done = false;
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

        _done = true;
    }

    protected void HitCheck() {
        Transform transform = _unit.transform;
        Vector3 inFront = transform.position + transform.forward + Vector3.up;

        ExtDebug.DrawBox(inFront, HalfBox, transform.rotation, Color.red);

        RaycastHit[] hits = Physics.BoxCastAll(inFront, HalfBox, transform.forward, transform.rotation, MaxDistance, targetMask);

        foreach (var hit in hits)
        {
            Debug.DrawLine(transform.position + Vector3.up, hit.collider.transform.position + Vector3.up, Color.yellow);

            if (!_hitTable.ContainsKey(hit.transform))
            {
                UnitShell unit = hit.transform.gameObject.GetComponentInChildren<UnitShell>();
                if (unit != null)
                {
                    DoHitOnTarget(unit, hit.point);
                    _hitTable.Add(hit.transform, unit);
                }
                else {
                    Debug.LogError("Collider on \"" + hit.transform.gameObject.name + "\" in \"Unit\" layer does not have a BaseUnit component!");
                }
            }
        }
    }

    private void DoHitOnTarget(UnitShell target, Vector3 impact) {
        if (target.ChildUnit.GetTeam != _unit.ChildUnit.GetTeam)
        {
            if (impact == Vector3.zero)
                impact = target.transform.position + Vector3.up;

            GameManager.Instance.EffectManager.TriggerEffect(Effect.EffectId.MeleeHit, impact);
            target.DealDamage(Damage);
        }
    }
}
