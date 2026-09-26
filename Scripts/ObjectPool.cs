using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    // Use prefab instance ID as key
    private Dictionary<int, Queue<ParticleSystem>> particlePools = new Dictionary<int, Queue<ParticleSystem>>();
    private Dictionary<int, ParticleSystem> prefabReferences = new Dictionary<int, ParticleSystem>();
    
    // Optionally pre-warm pools
    [System.Serializable]
    public class PoolPreload
    {
        public ParticleSystem prefab;
        public int count = 10;
    }
    public PoolPreload[] preloadPools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PreloadPools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PreloadPools()
    {
        if (preloadPools == null) return;
        
        foreach (var pool in preloadPools)
        {
            if (pool.prefab == null) continue;
            
            // Pre-instantiate particles
            for (int i = 0; i < pool.count; i++)
            {
                CreateNewParticle(pool.prefab);
            }
        }
    }

    private ParticleSystem CreateNewParticle(ParticleSystem prefab)
    {
        int prefabId = prefab.GetInstanceID();
        
        // Initialize pool if needed
        if (!particlePools.ContainsKey(prefabId))
        {
            particlePools[prefabId] = new Queue<ParticleSystem>();
            prefabReferences[prefabId] = prefab;
        }

        ParticleSystem newParticle = Instantiate(prefab, transform);
        newParticle.gameObject.SetActive(false);
        particlePools[prefabId].Enqueue(newParticle);
        return newParticle;
    }

    public ParticleSystem GetParticleSystem(ParticleSystem prefab)
    {
        if (prefab == null) return null;

        int prefabId = prefab.GetInstanceID();
        ParticleSystem particle;

        // Create new pool if needed
        if (!particlePools.ContainsKey(prefabId))
        {
            particle = CreateNewParticle(prefab);
        }
        else
        {
            Queue<ParticleSystem> pool = particlePools[prefabId];
            
            // Create new particle if pool is empty
            if (pool.Count == 0)
            {
                particle = CreateNewParticle(prefab);
            }
            
            particle = pool.Dequeue();
            
            // Validate particle still exists
            if (particle == null)
            {
                particle = CreateNewParticle(prefab);
            }
        }

        particle.gameObject.SetActive(true);
        return particle;
    }

    public void ReturnParticleSystem(ParticleSystem particle)
    {
        if (particle == null) return;

        // Find the original prefab ID
        foreach (var kvp in prefabReferences)
        {
            if (particle.name.StartsWith(kvp.Value.name))
            {
                particle.gameObject.SetActive(false);
                particle.transform.SetParent(transform);
                particlePools[kvp.Key].Enqueue(particle);
                return;
            }
        }

        // If we can't find the pool, destroy it
        Destroy(particle.gameObject);
    }
} 