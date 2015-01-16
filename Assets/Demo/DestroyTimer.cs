using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {

	public float LifeTime;

	private float m_startTime;

	void Start()
	{
		m_startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_startTime + LifeTime < Time.time)
			Destroy(this.gameObject);
	}
}
