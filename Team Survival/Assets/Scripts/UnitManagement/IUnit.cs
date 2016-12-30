using UnityEngine;

public interface IUnit {
    string Name { get; }

    UnitShell Shell { get; }

    float MoveSpeed { get; }

    Team GetTeam { get; }

    GameObject gameObject { get; }

    Vector3 Position { get; }

}
