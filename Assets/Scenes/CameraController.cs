using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 500;
    public float rollSpeed = 40;
    public float yawSpeed = 40;
    public float pitchSpeed = 40;
	private float maxMoveSpeed = 800;

    // Use this for initialization
    void Start()
    {
		//Initial position set in inspector
    }

    // Update is called once per frame
    void Update()
    {
		//To avoid tunneling
		if (moveSpeed > maxMoveSpeed) {
			moveSpeed = maxMoveSpeed;
		}

		//Move left and right
		if (Input.GetAxis ("Horizontal") < 0) {
			transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
		} else if (Input.GetAxis ("Horizontal") > 0) {
			transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
		}

		//Move forward and backward
		if (Input.GetAxis ("Vertical") < 0) {
			transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
		} else if (Input.GetAxis ("Vertical") > 0) {
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
		}

		//Roll
		if (Input.GetKey ("q")) {
			transform.Rotate(Vector3.forward, rollSpeed * Time.deltaTime);
		} else if (Input.GetKey ("e")) {
			transform.Rotate(Vector3.forward, -rollSpeed * Time.deltaTime);
		}
			
		//Yaw
		if (Input.GetAxis("Mouse X") < 0) 
		{
			transform.Rotate(Vector3.up, -yawSpeed * Time.deltaTime);
		} else if (Input.GetAxis("Mouse X") > 0)
		{
			transform.Rotate(Vector3.up, yawSpeed * Time.deltaTime);
		}

		//Pitch
		if (Input.GetAxis("Mouse Y") < 0)
		{
			transform.Rotate(Vector3.right, pitchSpeed * Time.deltaTime);
		}
		else if (Input.GetAxis("Mouse Y") > 0)
		{
			transform.Rotate(Vector3.right, -pitchSpeed * Time.deltaTime);
		}

		//Check boundaries
		checkBoundaries();
	
    }

	void checkBoundaries() {
		//Get dimension of landscape
		GameObject landscape = GameObject.Find("NewMesh");
		float outerLimit = landscape.GetComponent <NewDiamondSquare> ().nSize / 2;

		//Make sure we stay within landscape
		if (transform.position.x > outerLimit) {
			//Put back inside boundary
			transform.position = new Vector3 (outerLimit, transform.position.y, transform.position.z);
		} else if (transform.position.x < -outerLimit) {
			transform.position = new Vector3 (-outerLimit, transform.position.y, transform.position.z);
		}

		if (transform.position.z > outerLimit) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, outerLimit);
		} else if (transform.position.z < -outerLimit) {
			transform.position = new Vector3 (transform.position.x, transform.position.y, -outerLimit);
		}
	}
		
}
