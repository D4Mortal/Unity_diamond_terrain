using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunOrbit : MonoBehaviour
{
    public Transform target;
    public Material terrain;
    public float radius = 100.0f;
    public float angularVelocity = 40.0f;
    
    // Use this for initialization
    void Start()
    {           
    }

    // function taken from lab 2 to create orbit around the target
    void Orbit()
    {
        // set a constant radius of orbit
        transform.position = target.position + (transform.position - target.position).normalized * radius;
        // make the object rotate around the target
        transform.RotateAround(target.position, Vector3.right, angularVelocity * Time.deltaTime);
        
    }

    void Update()
    {
        Orbit();
        // set the point light position to be the same as the sun to create the lighting effect of sun rise and fall
        terrain.SetVector("_PointLightPosition", this.transform.position);

    }
}
