using UnityEngine;

[DefaultExecutionOrder(100)] 
public class LODDisabler : MonoBehaviour
{
    void Start()
    {
        // Find all LODGroup components in the scene
        LODGroup[] allLODGroups = FindObjectsOfType<LODGroup>();
        
        // Disable each LOD group
        foreach (LODGroup lodGroup in allLODGroups)
        {
            if (lodGroup.tag == "ignore")
                continue;

            lodGroup.enabled = false;
        }

        Debug.Log($"Disabled {allLODGroups.Length} LOD Groups");
    }
}