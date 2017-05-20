public class UnitEventHandle
{
    public delegate void OnTeamChangedDelegate(Team team);
    public OnTeamChangedDelegate OnTeamChanged;

    public delegate void OnKillDelegate();
    public OnKillDelegate OnKill;

    public delegate void OnLifeStateCahngedDelegate(LifeState newState);
    public OnLifeStateCahngedDelegate OnLifeStateChanged;

    public delegate void OnHealthChangedDelegate(float health);
    public OnHealthChangedDelegate OnHealthChanged;

    public void CallOnTeamChanged(Team team) {
        if (OnTeamChanged != null)
            OnTeamChanged(team);
    }

    public void CallOnKill()
    {
        if (OnKill != null)
            OnKill();
    }

    public void ClearAll() {
        OnTeamChanged = null;
        OnKill = null;
        OnLifeStateChanged = null;
        OnHealthChanged = null;
    }
}
