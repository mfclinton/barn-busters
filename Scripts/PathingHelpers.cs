using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public static class PathingHelpers
{
    public static Vector3? GetClosestWalkableGridPoint(Vector3 pos)
    {
        AstarPath path = AstarPath.active;
        if (path != null)
        {
            NNInfo info = path.GetNearest(pos);
            if (info.node.Walkable)
                return info.position;
        }

        return null;
    }

    public static Vector3 SamplePointFromBox(BoxCollider collider)
    {
        Bounds bounds = collider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
