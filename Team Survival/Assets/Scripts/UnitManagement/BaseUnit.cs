using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public float UnitMoveSpeed;
    public float MoveSpeed { get { return UnitMoveSpeed; } }

    public Team UnitTeam;
    //TODO Rename!
    public Team GetTeam { get { return UnitTeam; } }

    public string UnitName;
    public string Name { get { return UnitName; } }

    public int MaxHealth;
}
