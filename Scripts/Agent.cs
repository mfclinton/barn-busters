using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMovePosition))]
public class Agent : MonoBehaviour
{
    public AgentRoundInfo agentRoundInfo { get; set; }

    public float height { get; private set; }
    private IMovePosition movePosition;
    public RagdollController rc { get; set; }

    // Actions
    public delegate void CollisionEventHandler(Collision collision);
    public event CollisionEventHandler OnCollision;

    public delegate void ReachedPositionEventHandler();
    public event ReachedPositionEventHandler OnReachedMovePosition;

    private void Awake()
    {
        height = GetComponent<Collider>().bounds.size.y;
        movePosition = GetComponent<IMovePosition>();
        rc = GetComponent<RagdollController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Collision: {collision.gameObject.name}, Speed: {Mathf.Abs(collision.relativeVelocity.magnitude)}");
        OnCollision?.Invoke(collision);
    }

    public void Teleport(Vector3 position, bool resetPath)
    {
        movePosition.Teleport(position, resetPath);
    }

    public void SetAgentRoundInfo(AgentRoundInfo agentRoundInfo)
    {
        this.agentRoundInfo = agentRoundInfo;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // Debug.Log($"{gameObject.name}: Moving to {targetPosition}");
        movePosition.SetMovePosition(targetPosition, () => OnReachedMovePosition?.Invoke());
    }
}
