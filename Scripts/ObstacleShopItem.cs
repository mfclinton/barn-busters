using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyBuildSystem.Features.Runtime.Buildings.Part;
using EasyBuildSystem.Features.Runtime.Buildings.Placer;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ScriptableObjects/ObstacleShopItem", order = 1)]
public class ObstacleShopItem : ISelectable
{
    [SerializeField]
    BuildingPart buildingPart;

    public override void Select()
    {
        BuildingPlacer.Instance.ChangeBuildMode(BuildingPlacer.BuildMode.PLACE);
        BuildingPlacer.Instance.SelectBuildingPart(buildingPart);
    }

    public override void DeSelect()
    {
        BuildingPlacer.Instance.ChangeBuildMode(BuildingPlacer.BuildMode.NONE);
    }
}
