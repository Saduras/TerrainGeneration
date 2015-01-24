using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour {

	public Terrain Terrain { get; private set; }
	public int Detail = 8;
	public float Roughness = 0.7f;

	// Use this for initialization
	void Start () 
	{
		Init();
	}

	public void Init()
	{
		Terrain = GetComponent<Terrain>();
	}

	public void Generate()
	{
		if(!Terrain)
			Terrain = GetComponent<Terrain>();

		// Default roughness function 2^(-H) for H := m_roughness
		// does not depend on average, max or size i.e. the callback is constant
		float roughnessResult = Mathf.Pow(2, -Roughness);
		int resolution = (int)Mathf.Pow(2, Detail) + 1;
		DSANoise noise = new DSANoise(Detail);
		noise.SetControlValues(
			Random.Range(0f, 1f),
			Random.Range(0f, 1f),
			Random.Range(0f, 1f),
			Random.Range(0f, 1f)
			);
		noise.SetRoughnessFunction((average, max, size) => {
			return roughnessResult;
		});
		noise.Generate();
		Terrain.terrainData.heightmapResolution = resolution;
		Terrain.terrainData.size = new Vector3(2000, 600, 2000);
		Terrain.terrainData.SetHeights(0, 0, noise.Map);
	}
}
