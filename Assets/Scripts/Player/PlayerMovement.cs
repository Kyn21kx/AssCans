using System.Collections;
using System.Collections.Generic;
using Auxiliars;
using UnityEngine;

[RequireComponent(typeof(Overheating))]
public class PlayerMovement : MonoBehaviour {

    const float BOOST_FACTOR = 10f;

    #region Variables
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

	private Vector3Int rotationYPR;

	private Rigidbody rig;
	private Overheating overheatingRef;
	#endregion

	public bool MovingUpwards { get; private set; }
	public bool Decelerating { get; private set; }

	private void Start() {
		this.rig = this.GetComponent<Rigidbody>();
		this.overheatingRef = this.GetComponent<Overheating>();
		this.MovingUpwards = false;
		this.Decelerating = false;
		this.rotationYPR = Vector3Int.zero;
	}

	private void Update() {
		this.HandleInput();
	}

	private void FixedUpdate() {
		this.Move();
	}

	private void Move() {
		if (this.MovingUpwards)
			this.ApplyUpwardsMovement();
		if (this.Decelerating)
			this.DecelerateMovement();

		this.ApplyRotations();

		//Clamping velocity's magnitude
		this.rig.velocity = Vector3.ClampMagnitude(this.rig.velocity, this.maxVelocity);
	}

	private void HandleInput() {
		//TODO: Replace this with a general input call
		this.MovingUpwards = Input.GetKey(KeyCode.Space);
		this.Decelerating = Input.GetKey(KeyCode.LeftShift);

		bool usedRotation = this.UpdateRotationInput();
		if (!usedRotation) {
			this.rotationYPR = Vector3Int.zero;
		}
	}

	private bool UpdateRotationInput() {
		bool movementPressed = PressedMovementKeysAndUpdate();
		//Clamp the vector to their min and max values to not go over and apply some weird forces (TODO: maybe normalize this, but let's wait)
		this.rotationYPR.Clamp(Vector3IntExtensions.MinusOne, Vector3Int.one);
		return movementPressed;
	}

	private bool PressedMovementKeysAndUpdate()
	{
		bool pressed = false;
		//Move the Yaw (Y - axis to the leftmost position)
		if (Input.GetKey(KeyCode.Q))
		{
			this.rotationYPR.y--;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.E))
		{
			this.rotationYPR.y++;
			pressed = true;
		}
		//Move the other rotations
		if (Input.GetKey(KeyCode.W))
		{
			this.rotationYPR.x++;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.S))
		{
			this.rotationYPR.x--;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.rotationYPR.z++;
			pressed = true;
		}
		if (Input.GetKey(KeyCode.D))
		{
			this.rotationYPR.z--;
			pressed = true;
		}
		return pressed;
	}

	private void DecelerateMovement()
	{
		if (this.overheatingRef.IsOverheated) return;
		//Get the current movement vector
		Vector3 currentVelocity = this.rig.velocity;
		//Check if it's not zero
		if (SpartanMath.ArrivedAt(currentVelocity, Vector2.zero))
		{
			//Completely stop
			this.rig.velocity = Vector2.zero;
			return;
		}
		//Apply the negative vector of that
		this.rig.AddForce((-currentVelocity) * this.rotationStabilitySpeed * Time.fixedDeltaTime * BOOST_FACTOR, ForceMode.Impulse);
		this.overheatingRef.IncreaseHeat();
	}

	private void ApplyUpwardsMovement() {
		//Apply a force to the relative upwards vector
		Vector3 forceToApply = this.transform.up * engineForce * BOOST_FACTOR * Time.fixedDeltaTime;
		this.overheatingRef.IncreaseHeat();
		if (this.overheatingRef.IsOverheated) return;
		this.rig.AddForce(forceToApply, ForceMode.Impulse);
		
	}

	private void ApplyRotations() {
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
		Vector3 rotDir = (Vector3)this.rotationYPR * rotationForce * BOOST_FACTOR * Time.fixedDeltaTime;
		this.rig.transform.Rotate(rotDir);
	}
}
