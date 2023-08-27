using System.Collections;
using System.Collections.Generic;
using Auxiliars;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttractableBody : MonoBehaviour, IAttractable
{
	public Rigidbody Rig { get; private set; }

	public Vector3 Position => this.transform.position;

	public float Mass => mass;

	[SerializeField]
	private float mass;

    public void Start()
    {
		this.Rig = GetComponent<Rigidbody>();
    }

    public Vector3 CalculateAcceleration(Transform destinationBody, float bodyMass)
	{
		//Calculate the distance between the two bodies
		float sqrDst = SpartanMath.DistanceSqr(this.transform.position, destinationBody.position);
        Vector3 forceDir = (destinationBody.position - this.Position).normalized;
        return forceDir * GravityModifier.GRAVITATIONAL_CONSTANT * bodyMass / sqrDst;
    }

	public void AddForce(Vector3 force)
	{
		this.Rig.velocity += force;
	}

}
