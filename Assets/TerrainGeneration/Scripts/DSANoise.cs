using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DSANoise {

	public delegate float CalculateRoughness(float average, float max, float size);

	public float[,] Map { get { return m_map; } }

	CalculateRoughness m_roughness;
	float[,] m_map;
	int m_max;

	public DSANoise(int detail) {
		m_max = (int) Mathf.Pow(2, detail);

		Reset();

		m_roughness = (average, max, size) => {
			return Mathf.Pow(2, -1.0f);
		};
	}

	public void SetControlValues(float a, float b, float c, float d)
	{
		m_map[0,0] = a;
		m_map[m_max,0] = b;
		m_map[0,m_max] = c;
		m_map[m_max,m_max] = d;
	}

	public void SetControlValue(int x, int y, float value)
	{
		m_map[x, y] = value;
	}

	public void SetRoughnessFunction(CalculateRoughness roughness) 
	{
		m_roughness = roughness;
	}

	public void Reset()
	{
		// Initialize the height map with -1
		// -1 indicates that the field was not set
		m_map = new float[m_max + 1, m_max + 1];
		for (int x = 0; x < m_map.GetLength(0); x++) {
			for (int y = 0; y < m_map.GetLength(1); y++) {
				m_map[x, y] = -1f;
			}
		}

		// set default control values
		SetControlValues(0.5f, 0.5f, 0.5f, 0.5f);
	}

	public void Generate() {
		int size = m_max;
		int x, y, half;
		float average, scale;

		while (size > 1) {
			half = size / 2;
			scale = size / (float)m_max;

			// recursion cancel condition
			if (half < 1) return;

			// iterate over square centers for current size
			for (x = half; x < m_max; x += size) {
				for (y = half; y < m_max; y += size) {
					// Skip field if it is already set
					if (m_map[x, y] >= 0f)
						continue;

					average = SquareAverage(x, y, half);
					float roundness = GetRoundness(average, size);
					float random = Random.Range(-scale * roundness, scale * roundness);
					m_map[x, y] = Mathf.Clamp(average + random, 0, 1);
				}
			}

			// iterate over diamond center for current size
			for (x = 0; x <= m_max; x += half) {
				for (y = (x + half) % size; y <= m_max; y += size) {
					// Skip field if it is already set
					if (m_map[x, y] >= 0f)
						continue;

					average = DiamondAverage(x, y, half);
					float roundness = GetRoundness(average, size);
					float random = Random.Range(-scale * roundness, scale * roundness);
					m_map[x, y] = Mathf.Clamp(average + random, 0, 1);
				}
			}

			size = half;
		}
	}
	

	/**
	 * Calculate the average of vertices of a diamond 
	 * with center in (x,y) and size (size)
	 */
	float SquareAverage(int x, int y, int size) {
		// square vertices in clockwise order
		int[][] vertices = new int[][]{
			new int[]{x - size, y - size},
			new int[]{x + size, y - size},
			new int[]{x + size, y + size},
			new int[]{x - size, y + size}
		};

		// check if vertex is on the map
		List<float> values = new List<float>();
		for (int i = 0; i != 4; i++)
		{
			if (IsInRange(vertices[i][0], 0, m_max)
				&& IsInRange(vertices[i][1], 0, m_max)) {
				values.Add(m_map[vertices[i][0], vertices[i][1]]);
			}
		}
		
		return values.Average();
	}

	/**
	* Calculate the average of vertices of a square
	* with center in (x,y) and size (size)
	*/
	float DiamondAverage(int x, int y, int size) {
		// diamond vertices in clockwise order
		int[][] vertices = new int[][]{
			new int[]{x, y - size},
			new int[]{x + size, y},
			new int[]{x, y + size},
			new int[]{x - size, y}
		};

		// check if vertex is on the map
		List<float> values = new List<float>();
		for (int i = 0; i != 4; i++)
		{
			if (IsInRange(vertices[i][0], 0, m_max)
				&& IsInRange(vertices[i][1], 0, m_max)) {
				values.Add(m_map[vertices[i][0],vertices[i][1]]);
			}
		}

		return values.Average();
	}

	float GetRoundness(float average, float size)
	{
		return Mathf.Clamp(m_roughness(average, m_max, size), 0, 1);
	}

	/**
	 * Check whether the value is in the closed interval [min, max] or not.
	 */
	bool IsInRange(int value, int min, int max) {
		return (value >= min) && (value <= max);
	}
}
