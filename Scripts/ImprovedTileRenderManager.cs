using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;
using EasyBuildSystem.Features.Runtime.Buildings.Part;
using UnityEngine;

public class ImprovedTileRenderManager : MonoBehaviour
{
    public Transform map;
    public Vector2Int size = new Vector2Int(10, 75);
    public float spacing = 5f;
    public Transform originTransform;
    public string childNameToDeactivate = "Other";

    GameObject[,] mappedObjects;
    float xOrigin, yOrigin;

    public void UpdatedMeshes()
    {
        // Maps out the objects
        mappedObjects = new GameObject[size.x, size.y];
        foreach(Transform child in map)
        {
            Vector2Int indexes = PosToIndex(child.position);
            mappedObjects[indexes.x, indexes.y] = child.gameObject;
        }

        // Disables the Children
        int width = mappedObjects.GetLength(0);
        int height = mappedObjects.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (NeighboringPosOccupied(x, y) && mappedObjects[x, y] != null)
                {
                    Transform tileRoot = mappedObjects[x, y].transform;
                    SetTileState(tileRoot, false);
                }
            }
        }
    }

    Vector2Int PosToIndex(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x - xOrigin) / spacing);
        int z = Mathf.RoundToInt((position.z - yOrigin) / spacing);

        return new Vector2Int(x, z);
    }

    bool NeighboringPosOccupied(int x, int y)
    {
        return CheckPosOccupied(x, y + 1)
            && CheckPosOccupied(x + 1, y)
            && CheckPosOccupied(x, y - 1)
            && CheckPosOccupied(x - 1, y);
    }

    void SetTileState(Transform tileRoot, bool isObserved)
    {
        Transform child = tileRoot.Find(childNameToDeactivate);
        if (child == null)
            Debug.LogError("CHILD SHOULD NOT BE NULL");

        child.gameObject.SetActive(isObserved);
    }

    bool CheckPosOccupied(int x, int y)
    {
        if(x < 0 || mappedObjects.GetLength(0) <= x || y < 0 || mappedObjects.GetLength(1) <= y || mappedObjects[x,y] == null)
        {
            return false;
        }

        return true;
    }

    void UpdateDeletedArea(Vector3 position)
    {
        Vector2Int indexes = PosToIndex(position);

        if(CheckPosOccupied(indexes.x, indexes.y + 1))
        {
            Transform tileRoot = mappedObjects[indexes.x, indexes.y + 1].transform;
            SetTileState(tileRoot, true);
        }

        if (CheckPosOccupied(indexes.x + 1, indexes.y))
        {
            Transform tileRoot = mappedObjects[indexes.x + 1, indexes.y].transform;
            SetTileState(tileRoot, true);
        }

        if (CheckPosOccupied(indexes.x, indexes.y - 1))
        {
            Transform tileRoot = mappedObjects[indexes.x, indexes.y - 1].transform;
            SetTileState(tileRoot, true);
        }

        if (CheckPosOccupied(indexes.x - 1, indexes.y))
        {
            Transform tileRoot = mappedObjects[indexes.x - 1, indexes.y].transform;
            SetTileState(tileRoot, true);
        }

        mappedObjects[indexes.x, indexes.y] = null;
    }

    private void Start()
    {
        xOrigin = originTransform.position.x;
        yOrigin = originTransform.position.z;

        UpdatedMeshes();
        BuildingManager.Instance.OnDestroyingBuildingPartEvent.AddListener((BuildingPart part) => {
            if (part.State == BuildingPart.StateType.DESTROY && part.gameObject.name.StartsWith("FloorTile"))
                UpdateDeletedArea(part.transform.position);
        });
    }
}
