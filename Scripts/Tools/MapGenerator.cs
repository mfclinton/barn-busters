using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using Pathfinding;

[System.Serializable]
public class TilePrefab
{
    public GameObject prefab;
    public float p;

    public TilePrefab(GameObject prefab, float p)
    {
        this.prefab = prefab;
        this.p = p;
    }

    public TilePrefab Clone()
    {
        return new TilePrefab(prefab, p);
    }
}

public class MapGenerator : MonoBehaviour
{
    [Header("Tile Map Settings")]
    [SerializeField]
    TilePrefab[] tilePrefabs;
    [SerializeField]
    Vector3 origin;
    [SerializeField]
    Vector2Int size;
    [SerializeField]
    float spacing;
    [SerializeField]
    Transform parentTransform;

    [Header("Cinemachine Settings")]
    [SerializeField]
    CinemachineVirtualCamera dollyCam;
    [SerializeField]
    Vector3 offsetFromCenterStart;

    [Header("Pathfinding Agents Settings")]
    [SerializeField]
    AstarPath pathfinder;
    [SerializeField]
    Vector3 offsetFromCenter;

    [Header("ohhh")]
    public GameObject[] gos;
    public Transform parent;



    public void GenerateMap()
    {
        GenerateTileMap();
        UpdateCinemachineDolly();
        UpdatePathfinding();
    }

    public void UpdateCinemachineDolly()
    {
        CinemachineTrackedDolly dolly = dollyCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        CinemachineSmoothPath smoothPath = ((CinemachineSmoothPath)dolly.m_Path);
        int numWaypoints = smoothPath.m_Waypoints.Length;
        
        float length = spacing * size[1];
        Vector3 centerStart = origin + new Vector3(1f,0f,0f) * spacing * size.x / 2f;
        centerStart += offsetFromCenterStart;

        for (int i = 0; i < numWaypoints; i++)
        {
            float posFrac = (float)i / (numWaypoints - 1);
            smoothPath.m_Waypoints[i].position = centerStart + new Vector3(0f, 0f, 1f) * length * posFrac;
        }

        dolly.m_PathPosition = 0f;
    }

    Vector3 GetCenterPos()
    {
        return origin + new Vector3(1f, 0f, 0f) * spacing * size[0] / 2f + new Vector3(0f, 0f, 1f) * spacing * size[1] / 2f;
    }

    public void UpdatePathfinding()
    {
        GridGraph gg = (GridGraph) pathfinder.graphs[0];
        gg.width = Mathf.RoundToInt(size[0] * spacing / gg.nodeSize); // * gg.nodeSize
        gg.depth = Mathf.RoundToInt(size[1] * spacing / gg.nodeSize);
        gg.center = GetCenterPos() + offsetFromCenter;
    }

    void PlaceTile(TilePrefab tile, int x, int z)
    {
        Vector3 position = origin + new Vector3(x, 0f, z) * spacing;
        GameObject obj = Instantiate(tile.prefab, position, Quaternion.identity);
        if (parentTransform != null)
            obj.transform.parent = parentTransform;
    }

    bool ProbsAreValid()
    {
        return tilePrefabs.Sum((tile) => tile.p) == 1f;
    }

    public void GenerateTileMap()
    {
        // Generate Tile Map
        if (!ProbsAreValid())
        {
            Debug.LogError("Map Generator Probabilities Are Not Normalized!");
            return;
        }

        if (parentTransform != null)
            while (parentTransform.childCount != 0)
                DestroyImmediate(parentTransform.GetChild(0).gameObject);

        // Modifies and Sorts TilePrefabs
        TilePrefab[] tiles = tilePrefabs.OrderBy((tile) => tile.p).Select((tile) => tile.Clone()).ToArray();

        float cummTotal = 0f;
        foreach (TilePrefab tile in tiles)
        {
            cummTotal += tile.p;
            tile.p = cummTotal;
        }

        for (int x = 0; x < size[0]; x++)
        {
            for (int z = 0; z < size[1]; z++)
            {
                float rng = Random.value;
                TilePrefab tile = tiles.First((tile) => rng <= tile.p);
                PlaceTile(tile, x, z);
            }
        }
    }

    public void FixPrefab()
    {
        List<GameObject> newChildren = new List<GameObject>();
        foreach (Transform child in parent)
        {
            child.name = child.name.Replace("(Clone)", "");
            GameObject prefab = gos.FirstOrDefault((GameObject go) => go.name == child.name);
            print($"{child.name}");
            GameObject newObj = Instantiate(prefab, child.position, child.rotation);
            newChildren.Add(newObj);
        }

        while (parent.childCount != 0)
            DestroyImmediate(parent.GetChild(0).gameObject);

        foreach (GameObject newGo in newChildren)
            newGo.transform.parent = parent;
    }
}
