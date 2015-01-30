using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

	TerrainControlPoint[,] ControlPoints;

	DSANoise m_noise;

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
		Terrain.terrainData.heightmapResolution = resolution;
		Terrain.terrainData.size = new Vector3(2000, 600, 2000);

		m_noise = new DSANoise(Detail);
		m_noise.GenerationCompleted += OnGenerationCompleted;
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

		Debug.Log("Start height map generation!");
		if (UsingCoroutine) {
			StopAllCoroutines();
			StartCoroutine(m_noise.GenerateRoutine());
		} else {
			m_noise.Generate();
			
		}
	}

#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		if (ControlPoints == null || ControlPoints.GetLength(0) < 2)
			return;

		Vector3 tl, tr, bl, br;
		for (int i = 1; i < ControlPoints.GetLength(0); i++) {
			for (int j = 1; j < ControlPoints.GetLength(1); j++) {
				tl = ControlPoints[i-1, j-1].transform.position;
				tr = ControlPoints[i  , j-1].transform.position;
				bl = ControlPoints[i-1, j  ].transform.position;
				br = ControlPoints[i  , j  ].transform.position;

				Debug.DrawLine(tl, tr);
				Debug.DrawLine(tr, br);
				Debug.DrawLine(br, bl);
				Debug.DrawLine(bl, tl);
			}
		}
	}

	public void StartEditControls()
	{
		int iterations = 1;

		int max = Terrain.terrainData.heightmapResolution - 1;
		int x, y, i, j;

		int controlMatrixSize = (int)Mathf.Pow(2, iterations) + 1;
		var controlCoordiantes = new Vector2[controlMatrixSize, controlMatrixSize];
		int step = max / (controlMatrixSize - 1);
		i = j = 0;
		for (x = 0; x <= max; x += step) {
			j = 0;
			for (y = 0; y <= max; y += step) {
				controlCoordiantes[i, j] = new Vector2(x, y);
				j++;
			}
			i++;
		}

		var td = Terrain.terrainData;
		var tPos = Terrain.transform.position;
		ControlPoints = new TerrainControlPoint[controlMatrixSize, controlMatrixSize];

		for (i = 0; i < controlMatrixSize; i++) {
			for (j = 0; j < controlMatrixSize; j++) {

				var go = new GameObject("CtrlPt");
				go.transform.SetParent(Terrain.transform);
				var ctrlPt = go.AddComponent<TerrainControlPoint>();

				Vector3 worldPos = ControlPointToWorld(controlCoordiantes[i, j]);
				// Get height bounds
				float minY = tPos.y;
				float maxY = minY + td.size.y;

				ctrlPt.Init(controlCoordiantes[i, j], worldPos.x, worldPos.z, minY, maxY);
				go.transform.position = worldPos;

				ControlPoints[i, j] = ctrlPt;
			}
		}
	}

	public void StopEditControls()
	{
		ControlPoints = null;
	}

	Vector3 ControlPointToWorld(Vector2 point)
	{
		var td = Terrain.terrainData;
		int max = td.heightmapResolution - 1;

		Vector3 worldPos = new Vector3();
		worldPos.x = (point.x / (float)max) * td.size.x;
		worldPos.y = td.GetHeight((int)point.x, (int)point.y);
		worldPos.z = (point.y / (float)max) * td.size.z;

		return worldPos;
	}
#endif

	void OnGenerationCompleted()
	{
		Debug.Log("Generation completed!");
		Terrain.terrainData.SetHeights(0, 0, m_noise.Map);
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
