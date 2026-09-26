using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath))]
public class MovePositionAStarPathfinding : MonoBehaviour, IMovePosition
{
    private AIPath aiPath;

    private void Awake()
    {
        aiPath = GetComponent<AIPath>();
    }

    public void SetMovePosition(Vector3 movePosition, Action onReachedMovePosition)
    {
        aiPath.destination = movePosition;
    }

    public void Teleport(Vector3 position, bool resetPath)
    {
        aiPath.Teleport(position, true);
        if(resetPath)
            aiPath.destination = aiPath.position;
    }

    public float GetCurrentSpeed()
    {
        return aiPath.velocity.magnitude;
    }
}
