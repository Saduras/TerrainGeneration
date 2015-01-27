using UnityEngine;
using System.Collections;

public class MeteorStrike : MonoBehaviour {

	public Terrain Terrain;
	public Vector3 CenterOffset;
	public float Radius;

	public MeteorFall Meteor;

	bool m_ready = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && m_ready) {
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			var hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				m_ready = false;
				Meteor.StartFall(hit.point, () => {
					Explode(hit.point + CenterOffset, Radius);
					m_ready = true;
				});
				
			}
		}
	}

	void Explode(Vector3 center, float radius)
	{

		Vector3 worldSize = Terrain.terrainData.size;
		Vector3 worldCenter = Terrain.transform.position;
		int size = Terrain.terrainData.heightmapResolution;

		float[,] heightMap = Terrain.terrainData.GetHeights(0, 0, size, size);

		Vector3 pos;
		float height;
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				height = Terrain.terrainData.GetHeight(x,y);
				// Transform heigtmap pixel to world coordiantes
				pos = PixelToWorld(height, x, y, size, worldSize, worldCenter);

				// check collision with sphere
				if ( (center - pos).magnitude < radius ) 
				{
					heightMap[y, x] = ProjectHeight(pos, center, radius) / Terrain.terrainData.size.y;
				}

			}
		}

		Terrain.terrainData.SetHeights(0, 0, heightMap);
	}

	Vector3 PixelToWorld(float height, int x, int y, int size, Vector3 worldSize, Vector3 worldCenter)
	{
		var result = new Vector3(worldCenter.x + x / (float)size * worldSize.x,
			worldCenter.y + height,
			worldCenter.z + y / (float)size * worldSize.z);
		return result;
	}
	

	// radius = (point - center).magnitude => solve for point.y
	float ProjectHeight(Vector3 point, Vector3 center, float radius)
	{
		var result = - Mathf.Sqrt(radius * radius - Mathf.Pow(point.x - center.x, 2) - Mathf.Pow(point.z - center.z,2)) + center.y;
		return result;
	}
}
