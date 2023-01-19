using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Overheating))]
public class PlayerMovement : MonoBehaviour {

	[SerializeField]
	private float engineForce;
	[SerializeField]
	private float rotationForce;
	[SerializeField]
	private float maxVelocity;
	[SerializeField]
	private float rotationStabilityThreshold;
	[SerializeField]
	private float rotationStabilitySpeed;

	private Rigidbody rig;

	private Vector3Int rotationYPR;
	public bool MovingUpwards { get; private set; }

	private Overheating overheatingRef;

	private void Start() {
		this.rig = this.GetComponent<Rigidbody>();
		this.overheatingRef = this.GetComponent<Overheating>();
		this.MovingUpwards = false;
		this.rotationYPR = Vector3Int.zero;
	}

	private void Update() {
		this.HandleInput();
	}

	private void FixedUpdate() {
		this.Move();
	}

	private void Move() {
		const float BOOST_FACTOR = 10f;
		if (this.MovingUpwards)
			this.ApplyUpwardsMovement(BOOST_FACTOR);
		this.ApplyRotations(BOOST_FACTOR);

		//Clamping velocity's magnitude
		this.rig.velocity = Vector3.ClampMagnitude(this.rig.velocity, this.maxVelocity);
	}

	private void HandleInput() {
		//TODO: Replace this with a general input call
		this.MovingUpwards = Input.GetKey(KeyCode.Space);
		bool usedRotation = this.UpdateRotationInput();
		if (!usedRotation) {
			this.rotationYPR = Vector3Int.zero;
		}
	}

	private bool UpdateRotationInput() {
		bool pressed = false;
		//Move the Yaw (Y - axis to the leftmost position)
		if (Input.GetKey(KeyCode.Q)) {
			this.rotationYPR.y--;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.E)) {
			this.rotationYPR.y++;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.W)) {
			this.rotationYPR.x++;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.S)) {
			this.rotationYPR.x--;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.A)) {
			this.rotationYPR.z++;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.D)) {
			this.rotationYPR.z--;
			pressed = true;
		}
		//Clamp the vector to their min and max values to not go over and apply some weird forces (TODO: maybe normalize this, but let's wait)
		this.rotationYPR.Clamp(new Vector3Int(-1, -1, -1), Vector3Int.one);
		return pressed;
	}

	private void ApplyUpwardsMovement(float boostFactor) {
		//Apply a force to the relative upwards vector
		Vector3 forceToApply = this.transform.up * engineForce * boostFactor * Time.fixedDeltaTime;
		this.overheatingRef.IncreaseHeat();
		if (this.overheatingRef.IsOverheated) return;
		this.rig.AddForce(forceToApply, ForceMode.Impulse);
		
	}

	private void ApplyRotations(float boostFactor) {
		//Correct previous rotations that might've been caused by colliding into something
		Vector3 currAngVelocity = this.rig.angularVelocity;
		if (currAngVelocity.magnitude > this.rotationStabilityThreshold) {
			Debug.Log($"Chocamos alv: {currAngVelocity.magnitude}");
		}
		else if (this.rotationYPR != Vector3Int.zero) {
			this.rig.angularVelocity = Vector3.zero;
		}
		//Get the current difference to zero and keep lerping if necessary
		//Apply the proper rotation
		Vector3 rotDir = (Vector3)this.rotationYPR * rotationForce * boostFactor * Time.fixedDeltaTime;
		this.rig.transform.Rotate(rotDir);
	}
}
