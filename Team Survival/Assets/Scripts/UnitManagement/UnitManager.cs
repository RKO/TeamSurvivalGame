using System;
using System.Collections.Generic;
using System.Linq;

public class UnitManager {
    private Dictionary<Team, List<UnitShell>> _unitsByTeam;

    public UnitManager()
    {
        _unitsByTeam = new Dictionary<Team, List<UnitShell>>();
        foreach (var item in Enum.GetValues(typeof(Team)).Cast<Team>())
        {
            _unitsByTeam.Add(item, new List<UnitShell>());
        }
    }

    public void AddUnit(UnitShell unit)
    {
        _unitsByTeam[unit.ChildUnit.GetTeam].Add(unit);
    }

    public void RemoveUnit(UnitShell unit)
    {
        _unitsByTeam[unit.ChildUnit.GetTeam].Remove(unit);
    }

    public void RemovePlayer() {

    }

    public void KillUnit(UnitShell unit)
    {
        RemoveUnit(unit);
    }

    public List<UnitShell> GetUnits(Team team) {
        return _unitsByTeam[team];
    }

    public int GetUnitCount(Team team)
    {
        return _unitsByTeam[team].Count;
    }
}
