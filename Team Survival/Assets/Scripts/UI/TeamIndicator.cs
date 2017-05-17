using UnityEngine;

public class TeamIndicator : MonoBehaviour {
    private UnitShell _owner;
    private Projector _projector;

    private void Start()
    {
        _owner = GetComponentInParent<UnitShell>();
        _owner.EventHandle.OnTeamChanged += UpdateColor;
        _owner.EventHandle.OnLifeStateChanged += UpdateVisibility;

        _projector = GetComponentInChildren<Projector>();
        //Wow, really unity? This is needed so they don't all share the same material instance.
        _projector.material = new Material(_projector.material);

        UpdateColor(_owner.CurrentTeam);
        UpdateVisibility(_owner.AliveState);
    }

    private void UpdateColor(Team team)
    {
        if (team == Team.Players)
            _projector.material.color = Color.green;
        else if(team == Team.Enemies)
            _projector.material.color = Color.red;
        else
            _projector.material.color = Color.yellow;
    }

    private void UpdateVisibility(LifeState newState)
    {
        _projector.enabled = (newState == LifeState.Alive);
    }
}
