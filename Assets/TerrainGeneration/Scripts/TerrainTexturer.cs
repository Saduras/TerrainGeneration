using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(Terrain))]
public class TerrainTexturer : MonoBehaviour {

	[Serializable]
	public class TTextureData
	{
		public Texture2D Tex;
		public Texture2D NormalMap;
		public Vector2 TileOffSet;
		public Vector2 TileSize;
		public float HeightBorder;
	}
	
	
	public Terrain Terrain { get; private set; }
	public bool UseFractalNoise = true;
	public float NoiseFactor = 0.5f;
	public float NoiseRoughness = 0.7f;
	public List<TTextureData> HeightTexturData = new List<TTextureData>();

	// Use this for initialization
	void Start()
	{
		Init();
	}

	public void Init()
	{
		Terrain = GetComponent<Terrain>();
	}


	public void TextureTerrain()
	{
		if(!Terrain)
			Terrain = GetComponent<Terrain>();

		var td = Terrain.terrainData;
		td.alphamapResolution = td.heightmapResolution;
		var map = new float[td.alphamapWidth, td.alphamapHeight, HeightTexturData.Count];

		var noise = new float[td.alphamapWidth, td.alphamapHeight];
		if (UseFractalNoise) {
			var noiseGen = new DSANoise((int) Mathf.Log(td.heightmapResolution - 1, 2));
			noiseGen.SetRoughnessFunction((average, max, size) => { return NoiseRoughness; });
			noiseGen.Generate();
			noise = noiseGen.Map;
		}

		for (int x = 0; x < td.alphamapWidth; x++) {
			for (int y = 0; y < td.alphamapHeight; y++) {
				// For unknown reasons the alpha map has flipped coordinates
				// That's why we access the height of (y,x) but write it into (x,y)
				var height = td.GetHeight(y, x) / td.heightmapScale.y;

				if (UseFractalNoise) {
					height += (0.5f - noise[x, y]) * NoiseFactor;
				}

				map[x, y, 0] = (height < HeightTexturData.First().HeightBorder) ? 1.0f : 0.0f;
				for (int k = 1; k < HeightTexturData.Count - 1; k++) {
					map[x, y, k] = (height >= HeightTexturData[k - 1].HeightBorder
						&& height < HeightTexturData[k].HeightBorder) 
							? 1.0f : 0.0f;
				}
				if (HeightTexturData.Count > 1)
					map[x, y, HeightTexturData.Count - 1] = (height >= HeightTexturData[HeightTexturData.Count - 2].HeightBorder) ? 1.0f : 0.0f;

			}
		}

		td.splatPrototypes = ToSplatArray(HeightTexturData);
		td.SetAlphamaps(0, 0, map);
	}

	private SplatPrototype[] ToSplatArray(List<TTextureData> texData)
	{
		var result = new SplatPrototype[texData.Count];
		for (int i = 0; i < texData.Count; i++) {
			result[i] = new SplatPrototype();
			result[i].texture = texData[i].Tex;
			result[i].normalMap = texData[i].NormalMap;
			result[i].tileOffset = texData[i].TileOffSet;
			if (texData[i].TileSize == Vector2.zero) {
				texData[i].TileSize = new Vector2(texData[i].Tex.width, texData[i].Tex.height);
			}
			result[i].tileSize = texData[i].TileSize;
			
		}
		return result;
	}
}
