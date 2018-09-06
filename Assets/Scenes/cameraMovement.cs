using UnityEngine;
using System.Collections;
 
public class cameraMovement : MonoBehaviour {
 	
	
	float moveSpd = 3.0f; //regular speed
    float sensitivity = 0.1f; //How sensitive it with mouse
    private Vector3 prevPosition;
    public Rigidbody rb;

    void Start () {
        prevPosition = Input.mousePosition;
        Debug.Log("width: " + Screen.width);
        Debug.Log("height: " + Screen.height);
        rb = GetComponent<Rigidbody>();
        Debug.Log(this.transform.position);
    }

    void FixedUpdate () {
        //Debug.Log(this.transform.position);

        Vector3 move = Vector3.zero;
        if(Input.mousePosition.x <= 0) {
            move += Vector3.left;
        }
        if(Input.mousePosition.x >= Screen.width) {
            move += Vector3.right;
        }
        if(Input.mousePosition.y <= 0) {
            move += Vector3.down;
        }
        if(Input.mousePosition.y >= Screen.height) {
            move += Vector3.up;
        }
        //TODO - weird movement at excess up/down because up/down vector inverts when we flipfrom angling up to down
        Vector3 mouseMovement = Input.mousePosition - prevPosition + 30*move;
        
        prevPosition = Input.mousePosition;
        Vector3 cameraMovement = (mouseMovement * sensitivity);
        //line below received great help from https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996
        this.transform.eulerAngles = new Vector3(transform.eulerAngles.x - cameraMovement.y , transform.eulerAngles.y + cameraMovement.x , 0);
    
        //consider using these instead??
        //this.transform.Rotate(Vector3);
        //this.transform.LookAt(Vector3);
        
        this.rb.MovePosition(this.transform.position + GetBaseInput()*Time.deltaTime*moveSpd);
    }

    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = Vector3.zero;
        if (Input.GetKey (KeyCode.W)){
            p_Velocity += transform.forward;
        }
        if (Input.GetKey (KeyCode.S)){
            p_Velocity -= transform.forward;
        }
        if (Input.GetKey (KeyCode.A)){
            p_Velocity -= transform.right;
        }
        if (Input.GetKey (KeyCode.D)){
            p_Velocity += transform.right;
        }
        return p_Velocity;
    }
}