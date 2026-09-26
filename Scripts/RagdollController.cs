using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class RagdollController : MonoBehaviour
{
    [Header("Ragdoll Helper")]
    [Tooltip("Used to reposition character when unragdolling.")]
    public RagdollAnchor ragdollAnchor;

    [Header("Ragdoll Config")]
    public float absVelocityToRagdoll;
    public float timeToRecover;
    public float timeGracePeriod;
    public float maxDistToSnap;
    public AudioClip ragdolledNoise;

    [Header("Hit Effects")]
    [Tooltip("Array of particle systems to randomly choose from when hit")]
    public ParticleSystem[] hitEffects;
    [Tooltip("How long before the hit effect is destroyed/returned to pool")]
    public float hitEffectDuration = 1f;
    [Tooltip("Minimum collision force required to spawn hit effect")]
    public float hitEffectForceThreshold = 5f;

    private Animator anim;
    private AIPath aiPath;
    private Rigidbody rb;
    private Collider coll;
    private Agent agent;
    private AudioSource audioSource;

    private Collider[] childrenColls;
    private Rigidbody[] childrenRbs;

    // State
    public bool isActive { get; private set; }
    private float lastDisable;

    void Awake()
    {
        anim = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        agent = GetComponent<Agent>();
        ragdollAnchor.SetRagdollController(this, agent);
        audioSource = GetComponent<AudioSource>();

        childrenColls = GetComponentsInChildren<Collider>();
        childrenRbs = GetComponentsInChildren<Rigidbody>();
        
        RagdollActive(false);

        if(agent != null)
            agent.OnCollision += Ragdoll;
        if (anim != null)
            anim.keepAnimatorStateOnDisable = true;
    }

    public void ResetRagdoll()
    {
        foreach (Rigidbody childRb in childrenRbs)
            childRb.velocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        RagdollActive(false);
    }

    public void RagdollActive(bool active)
    {
        if (isActive == true && active == false)
            MoveToAnchor();
        if (active == false)
            lastDisable = Time.time;

        foreach (Collider c in childrenColls)
        {
            if(!c.isTrigger)
                c.enabled = active;
        }
        foreach (Rigidbody r in childrenRbs)
        {
            r.detectCollisions = active;
            r.isKinematic = !active;
        }

        if(anim != null)
            anim.enabled = !active;
        if (aiPath != null)
            aiPath.canMove = !active;
        rb.detectCollisions = !active;
        rb.isKinematic = active;
        coll.enabled = !active;

        isActive = active;
    }

    public IEnumerator DelayedHandleCollision(Collision collision, bool active, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        RagdollActive(active);
        if(active)
            foreach (Rigidbody r in childrenRbs) r.velocity = collision.relativeVelocity;
    }


    void MoveToAnchor()
    {
        if(ragdollAnchor != null)
        {
            float halfHeight = coll.bounds.size.y / 2f;
            if (agent != null)
                halfHeight = agent.height / 2f;

            Vector3 newPosition = ragdollAnchor.transform.position;

            Vector3? closestGridPoint = PathingHelpers.GetClosestWalkableGridPoint(newPosition);
            if(closestGridPoint.HasValue && Vector3.Distance(ragdollAnchor.transform.position, closestGridPoint.Value) <= maxDistToSnap)
                newPosition = closestGridPoint.Value + new Vector3(0f, halfHeight, 0f);

            transform.position = newPosition;
        }
    }


    bool CollisionTriggersRagdoll(Collision collision)
    {
        return !isActive && absVelocityToRagdoll <= Mathf.Abs(collision.relativeVelocity.magnitude);
    }

    void Ragdoll(Collision collision)
    {
        if (lastDisable + timeGracePeriod < Time.time && CollisionTriggersRagdoll(collision))
        {
            isActive = true;
            audioSource.PlayOneShot(ragdolledNoise);

            float collisionForce = collision.relativeVelocity.magnitude;
            if (hitEffects != null && hitEffects.Length > 0 && collisionForce >= hitEffectForceThreshold)
            {
                SpawnHitEffect(collision);
            }

            StartCoroutine(DelayedHandleCollision(collision, true, 0.01f));
            StartCoroutine(DelayedHandleCollision(collision, false, timeToRecover));
        }
    }

    private void SpawnHitEffect(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        ParticleSystem prefab = hitEffects[Random.Range(0, hitEffects.Length)];
        
        ParticleSystem effect = ObjectPool.Instance.GetParticleSystem(prefab);
        if (effect != null)
        {
            effect.transform.position = contact.point;
            effect.transform.rotation = Quaternion.LookRotation(contact.normal);
            effect.Play();

            // Use the particle system's duration instead of a fixed time
            float duration = effect.main.duration + effect.main.startLifetime.constantMax;
            StartCoroutine(ReturnToPoolAfterDuration(effect, duration));
        }
    }

    private IEnumerator ReturnToPoolAfterDuration(ParticleSystem effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (effect != null)
        {
            effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            ObjectPool.Instance.ReturnParticleSystem(effect);
        }
    }
}
