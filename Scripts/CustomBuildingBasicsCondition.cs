using UnityEngine;

using EasyBuildSystem.Features.Runtime.Buildings.Area;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;
using EasyBuildSystem.Features.Runtime.Buildings.Group;

namespace EasyBuildSystem.Features.Runtime.Buildings.Part.Conditions
{
    [BuildingCondition("Custom Building Conditions",
        "This helps guide obstacle placement\n\n" +
        "and more.")]
    public class CustomBuildingBasicsCondition : BuildingCondition
    {
        #region Fields
        [SerializeField] int maxElementsPerGroup = 5;
        public int MaxElementsPerGroup { get { return maxElementsPerGroup; } set { maxElementsPerGroup = value; } }

        [SerializeField] float minDistFromCluster = 5f;
        public float MinDistFromCluster { get { return minDistFromCluster; } set { minDistFromCluster = value; } }

        [SerializeField] int totalGrassDeletes = 14;
        public int TotalGrassDeletes { get { return totalGrassDeletes; } set { totalGrassDeletes = value; } }
        public static int grassDeletesUsed = 0;

        #endregion

        #region Internal Methods

        public override bool CheckPlacingCondition()
        {
            return true;
            // BuildingGroup bg = GetClosestBuildingGroup(transform.position);
            // if (bg == null || maxElementsPerGroup <= 0)
            //     return true;

            // var parts = bg.RegisteredBuildingPart;
            // if (parts.Count == 0 || parts.Count < maxElementsPerGroup)
            //     return true;

            // float minDist = -1f;
            // foreach(var p in parts)
            // {
            //     float dist = Vector3.Distance(transform.position, p.transform.position);
            //     if (minDist < 0f || dist < minDist)
            //         minDist = dist;
            // }

            // return MinDistFromCluster <= minDist;
        }

        public override bool CheckDestroyCondition()
        {
            if(!gameObject.name.StartsWith("FloorTile"))
                return true;
            
            if(totalGrassDeletes - grassDeletesUsed <= 0)
                return false;

            grassDeletesUsed++;
            return true;
        }

        public override bool CheckEditingCondition()
        {
            return false;
        }

        public BuildingGroup GetClosestBuildingGroup(Vector3 position)
        {
            BuildingManager bm = BuildingManager.Instance;
            BuildingGroup closest = null;
            float minDist = 0f;
            for (int i = 0; i < bm.RegisteredBuildingGroups.Count; i++)
            {
                if (bm.RegisteredBuildingGroups[i] != null)
                {
                    float dist = Vector3.Distance(position, bm.RegisteredBuildingGroups[i].transform.position);
                    if (closest == null || dist < minDist)
                    {
                        closest = bm.RegisteredBuildingGroups[i];
                        minDist = dist;
                    }
                }
            }

            return closest;
        }

        #endregion
    }
}