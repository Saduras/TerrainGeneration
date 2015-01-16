using UnityEngine;
using System.Collections;
using System;

public class MeteorFall : MonoBehaviour {

	public Vector3 StartOffset;
	public float FallTime;
	public GameObject Explosion;

	public void StartFall(Vector3 target, Action finishCallback)
	{
		gameObject.SetActive(true);
		StartCoroutine(FallRoutine(target, finishCallback));
	}

	IEnumerator FallRoutine(Vector3 target, Action finishCallback)
	{
		float time = 0.0f;
		Vector3 start = target + StartOffset;

		while (time < FallTime) {
			transform.localPosition = Vector3.Lerp(start, target, time / FallTime);

			yield return new WaitForEndOfFrame();

			time += Time.deltaTime;
		}

		gameObject.SetActive(false);
		Instantiate(Explosion, transform.position, transform.rotation);
		finishCallback();
	}
}
