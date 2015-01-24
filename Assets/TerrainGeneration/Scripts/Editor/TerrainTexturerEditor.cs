using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainTexturer))]
public class TerrainTexturerEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		TerrainTexturer texturer = (TerrainTexturer)target;

		DrawDefaultInspector();
		if (GUILayout.Button("Generate")) {
			texturer.TextureTerrain();
		}

	}
}
