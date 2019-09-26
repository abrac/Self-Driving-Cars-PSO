using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform target;
	public float distance, height, forward, rate;
	
	void Start () {
	
	}
	
	void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, target.position - target.forward * distance + Vector3.up * height, rate);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.forward * forward), rate);
	}
}
