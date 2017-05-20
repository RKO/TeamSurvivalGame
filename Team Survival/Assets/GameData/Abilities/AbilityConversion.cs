using UnityEngine;

public class AbilityConversion : BaseAbility
{
    private Vector3 HalfBox = new Vector3(1f, 1f, 0.5f);
    private LayerMask targetMask;

    [SerializeField]
    protected float MaxDistance = 0.5f;

    protected override void Initialize()
    {
        targetMask = 1 << LayerMask.NameToLayer("Unit");
    }

    protected override void DoActivate()
    {
        HitCheck();
    }

    protected void HitCheck()
    {
        Transform transform = _unit.transform;
        Vector3 inFront = transform.position + transform.forward + Vector3.up;

        ExtDebug.DrawBox(inFront, HalfBox, transform.rotation, Color.red);

        RaycastHit[] hits = Physics.BoxCastAll(inFront, HalfBox, transform.forward, transform.rotation, MaxDistance, targetMask);

        foreach (var hit in hits)
        {
            Debug.DrawLine(transform.position + Vector3.up, hit.collider.transform.position + Vector3.up, Color.yellow);

            UnitShell unit = hit.transform.gameObject.GetComponentInChildren<UnitShell>();
            if (unit != null)
            {
                DoHitOnTarget(unit, hit.point);
            }
            else {
                Debug.LogError("Collider on \"" + hit.transform.gameObject.name + "\" in \"Unit\" layer does not have a BaseUnit component!");
            }
        }
    }

    private void DoHitOnTarget(UnitShell target, Vector3 impact)
    {
        if (target.CurrentTeam != _unit.CurrentTeam)
        {
            if (impact == Vector3.zero)
                impact = target.transform.position + Vector3.up;

            GameManager.Instance.EffectManager.TriggerEffect(Effect.EffectId.Conversion, impact);
            target.SetNewTeam(_unit.CurrentTeam);
        }
    }
}
