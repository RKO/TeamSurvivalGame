﻿using System;
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
        _unitsByTeam[unit.CurrentTeam].Add(unit);
        if (OnUnitAdded != null)
            OnUnitAdded(unit);
    }

    public void ChangeUnitTeam(UnitShell unit, Team oldTeam)
    {
        _unitsByTeam[oldTeam].Remove(unit);
        _unitsByTeam[unit.CurrentTeam].Add(unit);
    }

    public void KillUnit(UnitShell unit)
    {
        RemoveUnit(unit);
    }

    public void RemoveUnit(UnitShell unit)
    {
        List<UnitShell> teamList = _unitsByTeam[unit.CurrentTeam];
        if (teamList.Contains(unit))
            teamList.Remove(unit);
    }

    public List<UnitShell> GetUnits(Team team) {
        return _unitsByTeam[team];
    }

    public int GetUnitCount(Team team)
    {
        return _unitsByTeam[team].Count;
    }

    public delegate void UnitAddedDelegate(UnitShell unit);
    public UnitAddedDelegate OnUnitAdded;
}
