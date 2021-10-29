using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ThoughtWorld.Terrain
{
    [CustomEditor(typeof(TerrainGeneratorOld))]
    public class TerrainGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainGeneratorOld script = (TerrainGeneratorOld)target;

            if (DrawDefaultInspector())
            {
                script.Initiate();
            }

            if (GUILayout.Button("New Seed"))
            {
                script.seed = Random.Range(0, 100);
                script.Initiate();
            }

            if (GUILayout.Button("Save Mesh as Asset"))
            {
                script.SaveMesh();
            }
        }
    }
}
