using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    public delegate void OnAgentTriggerHandler(Agent a);
    public event OnAgentTriggerHandler OnAgentTrigger;

    private void OnTriggerEnter(Collider other)
    {
        RagdollAnchor ra = other.GetComponent<RagdollAnchor>();
        Agent a = other.GetComponent<Agent>();

        if (ra != null)
            a = ra.agent;

        if(a != null)
            OnAgentTrigger?.Invoke(a);
    }
}
