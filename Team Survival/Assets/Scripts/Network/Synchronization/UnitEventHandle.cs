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
}
