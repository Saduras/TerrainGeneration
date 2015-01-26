using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UnityRandomGenerator : IRandomGenerator
{
	public float Range(float from, float to)
	{
		// Multiply by 1000f for extra digits while using unity's
		// int Random.Range(int, int)
		return (float)Random.Range(from * 1000f, to * 1000f) / 1000f;
	}
}
