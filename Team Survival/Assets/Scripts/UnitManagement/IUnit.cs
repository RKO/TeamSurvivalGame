using UnityEngine;

public interface IUnit {
    float MoveSpeed { get; }
    Team GetTeam { get; }
    GameObject gameObject { get; }
}
