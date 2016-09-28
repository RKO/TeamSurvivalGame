using UnityEngine;

public class AbilityBasicAttack : BaseAbility
{
    private const float Cooldown = 1f;
    private const float MaxDistance = 0.5f;
    private Vector3 HalfBox = new Vector3(1f, 1f, 0.5f);
    private LayerMask targetMask = 1 << LayerMask.NameToLayer("Unit");

    public override string Name { get { return "Attack"; } }

    public AbilityBasicAttack(BaseMotor caster, AnimationSync animSync) : base(caster, animSync, Cooldown) { }

    protected override void DoActivate()
    {
        _animSync.RpcTriggerAnimation(UnitTriggerAnimation.Attack1);

        Vector3 inFront = _caster.transform.position + _caster.transform.forward + Vector3.up;

        RaycastHit[] hits = Physics.BoxCastAll(inFront, HalfBox, _caster.transform.forward, _caster.transform.rotation, MaxDistance, targetMask);
        foreach (var hit in hits)
        {
            Debug.DrawLine(_caster.transform.position + Vector3.up, hit.collider.transform.position + Vector3.up, Color.yellow);


            UnitShell unit = hit.transform.gameObject.GetComponentInChildren<UnitShell>();
            if (unit != null)
            {
                DoHitOnTarget(unit);
            }
            else {
                Debug.LogError("Collider on \"" + hit.transform.gameObject.name+"\" in \"Unit\" layer does not have a BaseUnit component!");
            }
        }
    }

    private void DoHitOnTarget(UnitShell target) {
        if (target.ChildUnit.GetTeam == Team.Enemies)
        {
            Debug.Log("Hit: " + target.ChildUnit.Name);
            target.DealDamage(100);
        }

    }
}
