using System.Collections;
using System.Collections.Generic;
using Auxiliars;
using UnityEngine;

public class GravityModifier : MonoBehaviour {

	public const float GRAVITATIONAL_CONSTANT = 0.0001f;

    [SerializeField]
	private float radius;
	[SerializeField]
	private float pullingForce;
	[SerializeField]
	private float mass;

	//private const int GRAVITY_AFFECTED_LAYER = 0b00000100;
	private int gravityAffectedLayer;

	private void Start() {
		this.gravityAffectedLayer = LayerMask.NameToLayer("GravityAffected");
	}

	private void FixedUpdate() {
		//Do a sphere cast to the specified radius
		Collider[] hits = Physics.OverlapSphere(transform.position, radius);
		foreach (Collider hit in hits) {
			//Each one of the hits MUST contain a rigidbody, otherwise, ignore it, but log a warning
			Transform currTransform = hit.transform;
			if (currTransform == this.transform) continue;
			AttractableBody currBody = currTransform.GetComponent<AttractableBody>();
			if (currBody == null) {
				Debug.LogWarning($"The object {currTransform.name} does not have an attractable body attached to it, and therefore can't be affected by gravity!");
				continue;
			}
			//Here a rigidbody is guaranteed :D
			//Get the direction vector
			//Vector3 pullDirection = (currRig.position - transform.position).normalized;
			Vector3 pullDirection = currBody.CalculateAcceleration(this.transform, mass);
			currBody.AddForce(pullDirection);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius);
	}


}
