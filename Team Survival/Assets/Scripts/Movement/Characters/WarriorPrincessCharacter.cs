
using UnityEngine;

[RequireComponent(typeof(AnimationSync))]
[RequireComponent(typeof(BaseMotor))]
[RequireComponent(typeof(AbilityList))]
public class WarriorPrincessCharacter : BaseUnit
{
    public override Team GetTeam
    {
        get { return Team.Players; }
    }

    private string _name = "Player";

    public override string Name { get { return _name; } }
}
