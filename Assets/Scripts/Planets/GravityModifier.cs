using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityModifier : MonoBehaviour {

	[SerializeField]
	private float radius;
	[SerializeField]
	private float pullingForce;

	//private const int GRAVITY_AFFECTED_LAYER = 0b00000100;
	private int gravityAffectedLayer;

	private void Start() {
		this.gravityAffectedLayer = LayerMask.NameToLayer("GravityAffected");
	}

	private void FixedUpdate() {
		//Do a sphere cast to the specified radius
		Collider[] hits = Physics.OverlapSphere(transform.position, radius);
		Debug.Log($"Currently affecting {hits.Length} Objects");
		foreach (Collider hit in hits) {
			//Each one of the hits MUST contain a rigidbody, otherwise, ignore it, but log a warning
			Transform currTransform = hit.transform;
			Rigidbody currRig = currTransform.GetComponent<Rigidbody>();
			if (currRig == null) {
				Debug.LogWarning($"The object ${currTransform.name} does not have a rigidbody attached to it, and therefore can't be affected by gravity!");
				continue;
			}
			//Here a rigidbody is guaranteed :D
			//Get the direction vector
			Vector3 pullDirection = (currRig.position - transform.position).normalized;
			Vector3 moddedVelocity = currRig.velocity - pullDirection;
			currRig.velocity = moddedVelocity;
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius);
	}


}
