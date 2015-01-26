using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour {

	public Terrain Terrain { get; private set; }
	public int Detail = 8;
	public float Roughness = 0.7f;
	public bool UsingCoroutine = false;

	public Terrain NeighborLeft;
	public Terrain NeighborTop;
	public Terrain NeighborRight;
	public Terrain NeighborBottom;


	// Use this for initialization
	void Start () 
	{
		Init();
	}

	public void Init()
	{
		Terrain = GetComponent<Terrain>();
		Terrain.SetNeighbors(NeighborLeft, NeighborTop, NeighborRight, NeighborBottom);
	}

	public void Generate()
	{
		if(!Terrain)
			Terrain = GetComponent<Terrain>();

		// Default roughness function 2^(-H) for H := m_roughness
		// does not depend on average, max or size i.e. the callback is constant
		float roughnessResult = Mathf.Pow(2, -Roughness);
		int resolution = (int)Mathf.Pow(2, Detail) + 1;
		DSANoise m_noise = new DSANoise(Detail);
		m_noise.SetControlValues(
			Random.Range(0f, 1f),
			Random.Range(0f, 1f),
			Random.Range(0f, 1f),
			Random.Range(0f, 1f)
			);
		m_noise.SetRoughnessFunction((average, max, size) => {
			return roughnessResult;
		});

		GetControlsFromNeighbours(ref m_noise, resolution);

		if (UsingCoroutine) {
			StartCoroutine(NoiseGenerationRoutine(Terrain, m_noise, resolution));
		} else {
			m_noise.Generate();
			Terrain.terrainData.heightmapResolution = resolution;
			Terrain.terrainData.size = new Vector3(2000, 600, 2000);
			Terrain.terrainData.SetHeights(0, 0, m_noise.Map);
		}
	}

	IEnumerator NoiseGenerationRoutine(Terrain terrain, DSANoise noise, int resolution)
	{
		int size = resolution;

		while (size > 1) {
			noise.Step(size);
			size = size / 2;

			if (Time.deltaTime > 0.02f) {
				yield return new WaitForEndOfFrame();
			}
		}

		terrain.terrainData.heightmapResolution = resolution;
		terrain.terrainData.size = new Vector3(2000, 600, 2000);
		terrain.terrainData.SetHeights(0, 0, noise.Map);
	}


	void GetControlsFromNeighbours(ref DSANoise noise, int resolution)
	{
		if (NeighborLeft != null) {
			for (int j = 0; j < resolution; j++) {
				float height = NeighborLeft.terrainData.GetHeight(resolution - 1, j) / NeighborLeft.terrainData.heightmapScale.y;
				noise.SetControlValue(j, 0, height);
			}
		}

		if (NeighborTop != null) {
			for (int j = 0; j < resolution; j++) {
				float height = NeighborTop.terrainData.GetHeight(j, 0) / NeighborTop.terrainData.heightmapScale.y;
				noise.SetControlValue(resolution - 1, j, height);
			}
		}

		if (NeighborRight != null) {
			for (int j = 0; j < resolution; j++) {
				float height = NeighborRight.terrainData.GetHeight(0, j) / NeighborRight.terrainData.heightmapScale.y;
				noise.SetControlValue(j, resolution - 1, height);
			}
		}

		if (NeighborBottom != null) {
			for (int j = 0; j < resolution; j++) {
				float height = NeighborBottom.terrainData.GetHeight(j, resolution - 1) / NeighborBottom.terrainData.heightmapScale.y;
				noise.SetControlValue(0, j, height);
			}
		}

		Terrain.SetNeighbors(NeighborLeft, NeighborTop, NeighborRight, NeighborBottom);
	}

	
}
