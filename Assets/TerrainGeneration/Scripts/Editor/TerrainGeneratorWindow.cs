using UnityEngine;
using UnityEditor;
using System.Collections;

public class TerrainGeneratorWindow : EditorWindow
{
	TerrainGenerator m_generator;
	TerrainTexturer m_texturer;

	[MenuItem("Window/TerrainGenerator")]
	static void Init()
	{
		GetWindow<TerrainGeneratorWindow>();
	}

	void OnGUI()
	{
		m_generator = (TerrainGenerator)EditorGUILayout.ObjectField("Generator", m_generator, typeof(TerrainGenerator), true);

		if (m_generator != null) {
			m_generator.Detail = EditorGUILayout.IntField("Detail", m_generator.Detail);
			float mapSize = Mathf.Pow(2, m_generator.Detail) + 1;
			EditorGUILayout.LabelField("HeightMap size: " + mapSize + "x" + mapSize);
			m_generator.Roughness = EditorGUILayout.FloatField("Roughness", m_generator.Roughness);

			if (GUILayout.Button("Generate")) {
				m_generator.Generate();
			}
		}

		//
		// Texturing
		//
		//m_texturer = (TerrainTexturer)EditorGUILayout.ObjectField("Texturer", m_texturer, typeof(TerrainTexturer), true);

		//if(m_texturer != null)
		//{
		//	m_texturer.TexLow = (Texture2D)EditorGUILayout.ObjectField("TexLow", m_texturer.TexLow, typeof(Texture2D), false);
		//	m_texturer.TexMid = (Texture2D)EditorGUILayout.ObjectField("TexMid", m_texturer.TexMid, typeof(Texture2D), false);
		//	m_texturer.TexHigh = (Texture2D)EditorGUILayout.ObjectField("TexHigh", m_texturer.TexHigh, typeof(Texture2D), false);
		//	if (GUILayout.Button("Texture")) {
		//		m_texturer.TextureTerrain();
		//	}
		//}
	}
}
