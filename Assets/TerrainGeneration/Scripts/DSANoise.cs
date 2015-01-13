using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DSANoise {

	public delegate float CalculateRoughness(float average, float max, float size);

	CalculateRoughness m_roughness;
	int[,] _map;
	int _max;

	public DSANoise(int detail) {
		_max = (int) Mathf.Pow(2, detail);

		_map = new int[_max + 1, _max + 1];

		// set default control values
		int half = (_max + 1) / 2;
		SetControlValues(half, half, half, half);

		m_roughness = (average, max, size) => {
			return Mathf.Pow(2, -1.0f);
		};
	}

	public void SetControlValues(int a, int b, int c, int d) {
		_map[0,0] = a;
		_map[_max,0] = b;
		_map[0,_max] = c;
		_map[_max,_max] = d;
	}

	public void SetRoughnessFunction(CalculateRoughness roughness) 
	{
		m_roughness = roughness;
	}

	public void Generate() {
		Divide(_max);
	}

	public float[,] GetNoiseMap()
	{
		float[,] result = new float[_max + 1, _max + 1];

		for (int i = 0; i < _map.GetLength(0); i++) {
			for (int j = 0; j < _map.GetLength(1); j++) {
				result[i,j] = _map[i,j] / (float)_max;
			}
		}

		return result;
	}
	
	void Divide(int size) {
		int x, y;
		int half = size / 2;
		float average;
		int scale = size;

		// recursion cancel condition
		if (half < 1) return;

		// iterate over square centers for current size
		for (x = half; x < _max; x += size)
		{
			for (y = half; y < _max; y += size)
			{
				average = SquareAverage(x, y, half);
				float roundness = GetRoundness(average, size);
				int random = Mathf.FloorToInt(Random.Range(-scale * roundness, scale * roundness));
				_map[x,y] = Mathf.FloorToInt(Mathf.Clamp(average + random, 0, _max));
			}
		}

		// iterate over diamond center for current size
		for (x = 0; x <= _max; x += half)
		{
			for (y = (x + half) % size; y <= _max; y += size)
			{
				average = DiamondAverage(x, y, half);
				float roundness = GetRoundness(average, size);
				int random = Mathf.FloorToInt(Random.Range(-scale * roundness, scale * roundness));
				_map[x,y] = Mathf.FloorToInt(Mathf.Clamp(average + random, 0, _max));
			}
		}

		Divide(half);
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
			if (IsInRange(vertices[i][0], 0, _max)
				&& IsInRange(vertices[i][1], 0, _max)) {
				values.Add(_map[vertices[i][0], vertices[i][1]]);
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
			if (IsInRange(vertices[i][0], 0, _max)
				&& IsInRange(vertices[i][1], 0, _max)) {
				values.Add(_map[vertices[i][0],vertices[i][1]]);
			}
		}

		return values.Average();
	}

	float GetRoundness(float average, float size)
	{
		return Mathf.Clamp(m_roughness(average, _max, size), 0, 1);
	}

	/**
	 * Check whether the value is in the closed interval [min, max] or not.
	 */
	bool IsInRange(int value, int min, int max) {
		return (value >= min) && (value <= max);
	}
}
