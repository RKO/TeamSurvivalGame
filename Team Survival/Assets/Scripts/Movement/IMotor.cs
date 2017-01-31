
using UnityEngine;

public interface IMotor {
    //TODO This should not be available on the client.
    Transform Body { get; }
    Transform Head { get; }

    Vector3 MoveDirection { get; }

    void Initialize(float moveSpeed);

    bool CalculateIsGrounded();

    void SetMoveDestination(Vector3 destination);

    void SetMoveDirection(Vector3 dir);

    void SetRotateDestination(Vector3 dir);

    void AddForce(Vector3 force);

    void Stop();
}
