using UnityEngine;
using UnityEditor;
using System.Collections;

public class TerrainGeneratorWindow : EditorWindow
{
	Terrain m_terrain = null;
	int m_detail = 8;
	float m_roughness = 0.7f;

	Texture2D m_texLow;
	Texture2D m_texMid;
	Texture2D m_texHigh;

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
		m_terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", m_terrain, typeof(Terrain));

		if (m_terrain != null) {
			//
			// Generation
			//


			m_detail = EditorGUILayout.IntField("Detail", m_detail);
			float mapSize = Mathf.Pow(2, m_detail) + 1;
			EditorGUILayout.LabelField("HeightMap size: " + mapSize + "x" + mapSize);
			m_roughness = EditorGUILayout.FloatField("Roughness", m_roughness);

			// Default roughness function 2^(-H) for H := m_roughness
			// does not depend on average, max or size i.e. the callback is constant
			float roughnessResult = Mathf.Pow(2, -m_roughness);
			if (GUILayout.Button("Generate")) {
				DSANoise noise = new DSANoise(m_detail);
				noise.SetRoughnessFunction((average, max, size) => {
					return roughnessResult;
				});
				noise.Generate();
				m_terrain.terrainData.SetHeights(0, 0, noise.GetNoiseMap());
			}

			//
			// Texturing
			//

			m_texLow = (Texture2D)EditorGUILayout.ObjectField("TexLow", m_texLow, typeof(Texture2D));
			m_texMid = (Texture2D)EditorGUILayout.ObjectField("TexMid", m_texMid, typeof(Texture2D));
			m_texHigh = (Texture2D)EditorGUILayout.ObjectField("TexHigh", m_texHigh, typeof(Texture2D));
			if (GUILayout.Button("Texture")) {
				TextureTerrain();
			}
		}
	}

	void TextureTerrain()
	{
		var textures = new SplatPrototype[3];
		// low texture
		textures[0] = new SplatPrototype();
		textures[0].texture = m_texLow;
		// mid texture
		textures[1] = new SplatPrototype();
		textures[1].texture = m_texMid;
		// high texture
		textures[2] = new SplatPrototype();
		textures[2].texture = m_texHigh;

		var td = m_terrain.terrainData;
		td.alphamapResolution = td.heightmapResolution;
		var map = new float[td.alphamapWidth,td.alphamapHeight,3];

		float max = float.MinValue;
		float min = float.MaxValue;
		for (int x = 0; x < td.alphamapWidth; x++) {
			for (int y = 0; y < td.alphamapHeight; y++) {
				float xR = x / (float)td.alphamapWidth;
				float yR = y / (float)td.alphamapHeight;

				var angle = td.GetSteepness(xR, yR);
				var frac = angle / 90.0f;

				max = Mathf.Max(frac, max);
				min = Mathf.Min(frac, min);

				map[x, y, 0] = (frac < 0.3f) ? 1.0f : 0.0f;
				map[x, y, 1] = (frac >= 0.3f && frac < 0.7f) ? 1.0f : 0.0f;
				map[x, y, 2] = (frac >= 0.7f) ? 1.0f : 0.0f;

			}
		}
		Debug.Log("Max angle fraction: " + max + " min angle fraction: " + min);

		td.splatPrototypes = textures;
		td.SetAlphamaps(0, 0, map);
	}
}
