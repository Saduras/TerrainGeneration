using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Reflection;

[ExecuteInEditMode]
public class TerrainControlPoint : MonoBehaviour 
{
	public Vector2 HeightmapPoint { get; private set; }

	float FixedPosX;
	float FixedPosZ;
	float MinY;
	float MaxY;

	bool initialized = false;

	void Start () {
		// This script is only for editor use
		// Destroy if is still present while playing
		if (Application.isPlaying) {
			Debug.Log(this.gameObject + " has a TerrainControlPoint attached, which is for editor use only! It will be destroyed now!");
			Destroy(this.gameObject);
		}

		Texture2D tex = EditorGUIUtility.IconContent("sv_label_2").image as Texture2D;
		Type editorGUIUtilityType = typeof(EditorGUIUtility);
		BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
		object[] args = new object[] { this.gameObject, tex };
		editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
	}

	public void Init(Vector2 heightmapPt, float x, float z, float minY, float maxY)
	{
		HeightmapPoint = heightmapPt;
		FixedPosX = x;
		FixedPosZ = z;
		MinY = minY;
		MaxY = maxY;

		initialized = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!initialized || !Application.isEditor)
			return;

		var currentPos = transform.position;

		currentPos.x = FixedPosX;
		currentPos.z = FixedPosZ;
		currentPos.y = Mathf.Clamp(currentPos.y, MinY, MaxY);

		transform.position = currentPos;
	}
}
