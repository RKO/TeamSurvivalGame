using UnityEngine;

public class UnitDecorator : MonoBehaviour
{
    [SerializeField]
    private GameObject HealthBarPrefab;

    [SerializeField]
    private GameObject TeamIndicatorPrefab;

    public void Initialize(UnitManager unitManager)
    {
        unitManager.OnUnitAdded += DecorateNewUnit;
    }

    private void DecorateNewUnit(UnitShell unit)
    {
        if (!unit.isClient)
            return;

        AddHealthBar(unit);
        AddTeamIndicator(unit);
    }

    private void AddHealthBar(UnitShell unit)
    {
        var obj = Instantiate(HealthBarPrefab, unit.transform, false) as GameObject;
        obj.transform.localPosition = unit.HeadTransform.localPosition + Vector3.up * 0.5f;
    }

    private void AddTeamIndicator(UnitShell unit)
    {
        Instantiate(TeamIndicatorPrefab, unit.transform, false);
    }
}
