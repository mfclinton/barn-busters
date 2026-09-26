using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(IMovePosition), typeof(RagdollController))]
public class AgentAnimStateControler : MonoBehaviour
{
    [Header("Anim Config")]
    [Tooltip("Used to reposition character when unragdolling.")]
    public float minSpeedToWalk, minSpeedToRun;

    Animator anim;
    IMovePosition movePosition;
    RagdollController ragdollController;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        movePosition = GetComponent<IMovePosition>();
        ragdollController = GetComponent<RagdollController>();
    }

    public void Update()
    {
        float speed = movePosition.GetCurrentSpeed();
        anim.SetFloat("speed", speed);
        anim.SetBool("ragdolled", ragdollController.isActive);
    }
}
