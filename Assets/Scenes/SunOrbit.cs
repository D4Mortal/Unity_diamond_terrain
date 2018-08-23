using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Note: Code is taken from the CubeOrbit.cs file made and adjusted in Lab 2 where we spun the cube around
// another target. In this instance, this orbit script is used to make the sun rotate around a given point
// to mimic and the rise and fall of the sun over the course of a day
public class SunOrbit : MonoBehaviour
{
    // declare the target which the sun will orbit around (here, a Cube gameobject with the mesh renderer
    // turned off). Also declare the distance at which the sun will rotate around it, and its speed.
    public Transform target;
    public float orbitDistance = 5000.0f;
    public float orbitDegreesPerSec = 10.0f;

    // Use this for initialization
    // Nothing happens on running
    void Start()
    {

    }

    // function to make the target orbit around another
    void Orbit()
    {
        if (target != null)
        {
            // Keep us at orbitDistance from target
            // change the position so that its always a set distance from the targets position
            transform.position = target.position + (transform.position - target.position).normalized * orbitDistance;

            // public void RotateAround(Vector3 point, Vector3 axis, float angle);
            // in this case, using Vector3.right because we want it to rotate into the mesh and below it to mimic
            // night and day.
            transform.RotateAround(target.position, Vector3.right, orbitDegreesPerSec * Time.deltaTime);
        }
    }

    // Call from LateUpdate if you want to be sure your
    // target is done with it's move.
    void LateUpdate()
    {

        Orbit();

    }
}
