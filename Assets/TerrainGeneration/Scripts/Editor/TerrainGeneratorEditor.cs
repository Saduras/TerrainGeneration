using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		TerrainGenerator generator = (TerrainGenerator)target;

		DrawDefaultInspector();
		if (GUILayout.Button("Generate")) {
			generator.Generate();
		}

	}
}
