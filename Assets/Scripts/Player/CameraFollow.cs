using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour {

	[SerializeField]
	private Transform target;
	
	[SerializeField]
	private float mouseSpeed;

	//This is the difference between the two positions (pivot - cameraPos)
	[SerializeField]
	private Vector3 initialOffset;

	[SerializeField]
	private float maxLookupAngle;

	private Camera camRef;
	private Vector2 mouseInput;
	private Transform pivot;
	private Vector2 cameraRotation;

	private void Start() {
		Cursor.lockState = CursorLockMode.Locked;
		this.camRef = GetComponent<Camera>();
		this.pivot = this.transform.parent;
		this.initialOffset = target.position - pivot.position;
		this.mouseInput = Vector2.zero;
		this.cameraRotation = Vector2.zero;
	}

	private void Update() {
		this.FollowTarget();
		this.HandleInput();
		this.RotateAround();
	}

	private void HandleInput() {
		float mouseX = Input.GetAxisRaw("Mouse X");
		float mouseY = Input.GetAxisRaw("Mouse Y");
		this.mouseInput.Set(mouseX, mouseY);
	}



	private void RotateAround() {
		const float ROTATION_LIMIT_X = 0.3f;
		Vector3 rotationVector = new Vector3(this.mouseInput.y, this.mouseInput.x, 0f);
		this.pivot.Rotate(rotationVector * this.mouseSpeed * Time.deltaTime);
		Vector3 correctedEuler = this.pivot.eulerAngles;
		correctedEuler.z = 0f;
		Quaternion correctedQuat = Quaternion.Euler(correctedEuler);
		correctedQuat.x = Mathf.Clamp(correctedQuat.x, -ROTATION_LIMIT_X, ROTATION_LIMIT_X);
		correctedQuat.z = Mathf.Clamp(correctedQuat.z, -ROTATION_LIMIT_X, ROTATION_LIMIT_X);
		this.pivot.rotation = correctedQuat;
		Debug.Log($"Current rotation: {this.pivot.rotation}");
	}

	private void FollowTarget() {
		//Keep the camera at a fixed distance from the pivot
		//If the pivot is out of range, try to get closer, whilst maintaining the original position
		this.pivot.position = target.position - initialOffset;
	}

}
