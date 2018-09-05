using UnityEngine;
using System.Collections;
 
public class cameraMovement : MonoBehaviour {
 	
	
	float moveSpd = 20.0f; //regular speed
    float sensitivity = 0.5f; //How sensitive it with mouse
    private Vector2 prevPosition = new Vector2(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    //private float totalRun= 1.0f;
     
    void Update () {
        mouseMovement = Input.mousePosition - prevPosition;
        prevPosition = Input.mousePosition;
    }
     
    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += new Vector3(0, 0 , 1);
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}