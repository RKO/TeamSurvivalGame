using System;
using System.Collections.Generic;
using System.Linq;

public class UnitManager {
    private Dictionary<Team, List<IUnit>> _unitsByTeam;

    public UnitManager()
    {
        _unitsByTeam = new Dictionary<Team, List<IUnit>>();
        foreach (var item in Enum.GetValues(typeof(Team)).Cast<Team>())
        {
            _unitsByTeam.Add(item, new List<IUnit>());
        }
    }

    public int GetUnitCount(Team team) {
        return _unitsByTeam[team].Count;
    }

    public void AddUnit(IUnit unit)
    {
        _unitsByTeam[unit.GetTeam].Add(unit);
    }

    public void RemoveUnit(IUnit unit)
    {
        _unitsByTeam[unit.GetTeam].Remove(unit);
    }

    public void KillUnit(IUnit unit)
    {
        RemoveUnit(unit);
    }
}
