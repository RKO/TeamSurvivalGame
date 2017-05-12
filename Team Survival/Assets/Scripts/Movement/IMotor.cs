
using UnityEngine;

public interface IMotor {
    Vector3 MoveDirection { get; }

    void Initialize(Transform parent, float moveSpeed);

    bool CalculateIsGrounded();

    void SetMoveSpeed(float speed);

    void SetMoveDirection(Vector3 dir);

    void SetRotateDestination(Vector3 dir);

    void SetRotateDestination(Quaternion dir);

    void AddForce(Vector3 force);

    void LateUpdate();

    void Update();

    void Stop();
}
