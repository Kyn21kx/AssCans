using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttractable
{
    Vector3 CalculateAcceleration(Transform destinationBody, float bodyMass);

    void AddForce(Vector3 force);
}
