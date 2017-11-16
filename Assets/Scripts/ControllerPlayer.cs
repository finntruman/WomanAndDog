using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer : MonoBehaviour {

    ////////////////////////////
    /// PRIVATE VARIABLES
    ////////////////////////////

    // COMPONENTS
    private Rigidbody   com_rigidbody;
    private bool        m_jumping;
    private float       m_distToGround;

    // CONTROLS
    public int m_controlsUsed;
    // KEY CODES
    private KeyCode key_left;
    private KeyCode key_right;
    private KeyCode key_up;
    private KeyCode key_down;
    private KeyCode key_jump;

    // GAMEPAD
    private float m_deadZone;

    ////////////////////////////
    /// PUBLIC VARIABLES
    ////////////////////////////
    //public Camera com_camera;
    public float m_speed;
    public float m_jumpForce;
    public float rotatespeed = 0.1f;

	// Use this for initialization
	void Start ()
    {
        com_rigidbody   = GetComponent<Rigidbody>();
        m_jumping       = false;
        m_distToGround  = GetComponent<Collider>().bounds.extents.y;

        key_left    = KeyCode.A;
        key_right   = KeyCode.D;
        key_up      = KeyCode.W;
        key_down    = KeyCode.S;
        key_jump    = KeyCode.Space;

        m_deadZone  = 0.25f;

        m_speed     = 5.0f;
        m_jumpForce = 250.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float xMovement = 0.0f;
        float zMovement = 0.0f;

        if (Input.GetKey(key_left))
        {
            xMovement -= 1;
        }
        if (Input.GetKey(key_right))
        {
            xMovement += 1;
        }
        if (Input.GetKey(key_down))
        {
            zMovement -= 1;
        }
        if (Input.GetKey(key_up))
        {
            zMovement += 1;
        }

        // Stick input
        xMovement = Input.GetAxisRaw("Horizontal");
        zMovement = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(xMovement) < m_deadZone) xMovement = 0.0f;
        if (Mathf.Abs(zMovement) < m_deadZone) zMovement = 0.0f;

        // Jump
        if ((Input.GetKeyDown(key_jump) /*|| Input.GetButtonDown(btn_jump)*/) && IsGrounded())
        {
            com_rigidbody.AddForce(0, m_jumpForce, 0);
        }

        // Code helped created from this forum post: https://answers.unity.com/questions/803365/make-the-player-face-his-movement-direction.html

        Vector3 movement = new Vector3(xMovement, 0.0f, zMovement);

        // If the movement vector is zero, set rotation to itself, else slerp towards the direction of movement
        transform.rotation = (movement == Vector3.zero) ? transform.rotation : Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        transform.Translate(movement * m_speed * Time.deltaTime, Space.World);
	}

    // Code to detect grounded helped made with this forum post: https://answers.unity.com/questions/196381/how-do-i-check-if-my-rigidbody-player-is-grounded.html
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, m_distToGround + 0.1f);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Climb" && !IsGrounded())
        {
            print("Triggered climb");
        }
    }
}
