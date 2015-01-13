using UnityEngine;
using UnityEditor;
using System.Collections;

public class TerrainGeneratorWindow : EditorWindow
{
	Terrain m_terrain = null;
	AnimationCurve m_roundnessCurve = new AnimationCurve();
	int m_detail = 8;

	float m_smoothCenter = 0.5f;
	float m_smoothGrade = 0.5f;
	float m_smoothThreshold = 0.1f;
	int m_smoothIterations = 2;

	[MenuItem("Window/TerrainGenerator")]
	static void Init()
	{
		GetWindow<TerrainGeneratorWindow>();
	}

	void OnGUI()
	{
		m_terrain = (Terrain)EditorGUILayout.ObjectField("Terrain",m_terrain, typeof(Terrain));

		if (m_terrain != null) {
			m_roundnessCurve = EditorGUILayout.CurveField("Roundness", m_roundnessCurve);
			m_detail = EditorGUILayout.IntField("Detail", m_detail);
			float mapSize = Mathf.Pow(2, m_detail) + 1;
			EditorGUILayout.LabelField("HeightMap size: " + mapSize + "x" + mapSize);

			if (GUILayout.Button("Generate")) {
				DSANoise noise = new DSANoise(m_detail);
				noise.SetRoughnessFunction((average, max, size) => {
					return Mathf.Pow(2, -0.5f);
				});
				noise.Generate();
				m_terrain.terrainData.SetHeights(0, 0, noise.GetNoiseMap());
			}

			m_smoothCenter = EditorGUILayout.FloatField("Smooth center", m_smoothCenter);
			m_smoothGrade = EditorGUILayout.FloatField("Smooth grade", m_smoothGrade);
			m_smoothThreshold = EditorGUILayout.FloatField("Smooth threshold", m_smoothThreshold);
			m_smoothIterations = EditorGUILayout.IntField("Smooth iterations", m_smoothIterations);

			if (GUILayout.Button("Smooth")) {
				float[,] heightMap = m_terrain.terrainData.GetHeights(0, 0, m_terrain.terrainData.heightmapWidth, m_terrain.terrainData.heightmapHeight);
				for (int i = 0; i < m_smoothIterations; i++) {
					heightMap = SmoothHeightMap(heightMap);
				}
				m_terrain.terrainData.SetHeights(0,0,heightMap);
			}
		}
	}

	float[,] SmoothHeightMap(float[,] heightMap) {

		for (int i = 0; i < heightMap.GetLength(0); i++) {
			for (int j = 0; j < heightMap.GetLength(1); j++) {
				float diffToCenter = m_smoothCenter - heightMap[i, j];
				if ( Mathf.Abs(diffToCenter) < m_smoothThreshold) {
					heightMap[i, j] = heightMap[i, j] + Mathf.Sign(diffToCenter) * (m_smoothThreshold - Mathf.Abs(diffToCenter)) * m_smoothGrade;
				}
			}
		}

		return heightMap;
	}
}
