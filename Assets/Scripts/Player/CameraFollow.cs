using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {
	[SerializeField]
	private Transform target;
	[SerializeField]
	private float followSpeed;
	[SerializeField]
	private float mouseSpeed;
	//This is the difference between the two positions (target - cameraPos)
	[SerializeField]
	private Vector3 initialOffset;

	private Camera camRef;

	private void Start() { 
		this.camRef = GetComponent<Camera>();
		this.initialOffset = target.position - transform.position;
	}

	private void Update() {
		this.FollowTarget();
	}

	private void FollowTarget() {
		//Keep the camera at a fixed distance from the target
		//If the target is out of range, try to get closer, whilst maintaining the original position
		this.transform.position = target.position - initialOffset;
	}

}
