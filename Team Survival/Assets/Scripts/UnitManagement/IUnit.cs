using UnityEngine;

public interface IUnit {
    string Name { get; }

    float MoveSpeed { get; }

    Team GetTeam { get; }

    GameObject gameObject { get; }
}
