using UnityEngine;
using System.Collections;
 
public class cameraMovement : MonoBehaviour {
 	
     //make sure the floor is kinematic
	
	public float moveSpd = 7000.0f; //regular speed
    public float sensitivity = 0.1f; //How sensitive it with mouse
    public Rigidbody rb;
    private Vector3 prevPosition;

    void Start () {
        prevPosition = Input.mousePosition;
        
        rb = this.gameObject.AddComponent<Rigidbody>();
        SphereCollider collider = this.gameObject.AddComponent<SphereCollider>();
        rb.mass = 10;
        rb.drag = 5;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = false;
        collider.radius = 0.5f;
    }

    void FixedUpdate () {
        Vector3 move = Vector3.zero;
        if(Input.mousePosition.x <= 0) {
            move += Vector3.left;
        }
        if(Input.mousePosition.x >= Screen.width) {
            move += Vector3.right;
        }
        if(Input.mousePosition.y <= 0) {
            move += Vector3.down*0.5f;
        }
        if(Input.mousePosition.y >= Screen.height) {
            move += Vector3.up*0.5f;
        }
        //TODO - weird movement when camera looking straingt up/down and we want to go further up/down because up/down vector inverts when we flipfrom angling up to down
        Vector3 mouseMovement = Input.mousePosition - prevPosition ;
        Debug.Log(mouseMovement);
        prevPosition = Input.mousePosition;
        Vector3 cameraMovement = ((mouseMovement+5*move) * sensitivity);
        //line below received great help from https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996
        this.transform.eulerAngles = new Vector3(transform.eulerAngles.x - cameraMovement.y , transform.eulerAngles.y + cameraMovement.x , 0);
    
        //consider using these instead??
        //this.transform.Rotate(Vector3);
        //this.transform.LookAt(Vector3);
        
        this.rb.AddForce(GetBaseInput()*Time.deltaTime*moveSpd*rb.mass);
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