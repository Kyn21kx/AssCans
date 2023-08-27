using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{

    private Rigidbody rig;

    private void Start()
    {
        this.rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
    }

}
