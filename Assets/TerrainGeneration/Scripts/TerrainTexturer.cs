using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class TerrainTexturer : MonoBehaviour {

	public Terrain Terrain { get; private set; }

	public Texture2D TexLow;
	public Texture2D TexMid;
	public Texture2D TexHigh;

	// Use this for initialization
	void Start()
	{
		Terrain = GetComponent<Terrain>();
	}


	public void TextureTerrain()
	{
		if(!Terrain)
			Terrain = GetComponent<Terrain>();

		var textures = new SplatPrototype[3];
		// low texture
		textures[0] = new SplatPrototype();
		textures[0].texture = TexLow;
		// mid texture
		textures[1] = new SplatPrototype();
		textures[1].texture = TexMid;
		// high texture
		textures[2] = new SplatPrototype();
		textures[2].texture = TexHigh;

		var td = Terrain.terrainData;
		td.alphamapResolution = td.heightmapResolution;
		var map = new float[td.alphamapWidth, td.alphamapHeight, 3];

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
