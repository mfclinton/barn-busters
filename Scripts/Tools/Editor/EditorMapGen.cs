using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using EasyBuildSystem;
using EasyBuildSystem.Features.Runtime.Buildings.Part.Conditions;

[CustomEditor(typeof(MapGenerator))]
public class EditorMapGen : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Generate Map"))
        {
            MapGenerator mapGen = (MapGenerator)target;
            mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate Tilemap"))
        {
            MapGenerator mapGen = (MapGenerator)target;
            mapGen.GenerateTileMap();
        }

        if (GUILayout.Button("Update Cinemachine"))
        {
            MapGenerator mapGen = (MapGenerator)target;
            mapGen.UpdateCinemachineDolly();
        }

        if (GUILayout.Button("Update Pathfinding"))
        {
            MapGenerator mapGen = (MapGenerator)target;
            mapGen.UpdatePathfinding();
        }

        if (GUILayout.Button("TEMP"))
        {
            var allGos = FindObjectsOfType(typeof(GameObject));
            var objs = allGos.Where((obj) => (obj.name == "GroundForGrass"));
            foreach(GameObject obj in objs)
            {
                var c = obj.AddComponent<BuildingCollisionSurface>();
                c.SurfaceIdentifier = "GroundForGrass";
            }
        }

        if (GUILayout.Button("Fix"))
        {
            MapGenerator mapGen = (MapGenerator)target;
            mapGen.FixPrefab();
        }
    }
}
