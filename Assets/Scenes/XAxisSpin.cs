using UnityEngine;
using System.Collections;

// Code was taken from Tutorial 2 (where we made the cube spin)
// script to make the object spin (in this case, the sun)
public class XAxisSpin : MonoBehaviour
{

    public float spinSpeed;

    // Update is called once per frame
    void Update()
    {
        // this addresses the object which the script is attached to
        this.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * spinSpeed, new Vector3(1.0f, 1.0f, 0.0f));
    }
}
