using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnchor : MonoBehaviour
{
    RagdollController rc;
    public Agent agent { get; set; }
    public void SetRagdollController(RagdollController rc, Agent a)
    {
        this.rc = rc;
        this.agent = a;
    }
}
