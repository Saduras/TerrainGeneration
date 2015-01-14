using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour {

	public float Speed = 1f;

	float m_currentAngle;

	// Update is called once per frame
	void Update () 
	{
		m_currentAngle += Speed * 360f * Time.deltaTime;
		transform.localRotation = Quaternion.AngleAxis(m_currentAngle, Vector3.up);
	}
}
