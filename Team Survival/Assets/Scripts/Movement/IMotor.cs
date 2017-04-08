﻿
using UnityEngine;

public interface IMotor {
    Vector3 MoveDirection { get; }

    void Initialize(Transform parent, float moveSpeed);

    bool CalculateIsGrounded();

    void SetMoveDestination(Vector3 destination);

    void SetMoveDirection(Vector3 dir);

    void SetRotateDestination(Vector3 dir);

    void AddForce(Vector3 force);

    void LateUpdate();

    void Update();

    void Stop();
}
