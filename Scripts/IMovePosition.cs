using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovePosition
{
    void SetMovePosition(Vector3 movePosition, Action onReachedMovePosition);
    float GetCurrentSpeed();
    void Teleport(Vector3 position, bool resetPath);
}
