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
    bool m_hasControl; // determines whether or not the player has control over the character. This is for when prolonged animations need to be performed like climbing and pushing/pulling
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

        m_hasControl = true;

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
        if (m_hasControl)
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
	}

    // Code to detect grounded helped made with this forum post: https://answers.unity.com/questions/196381/how-do-i-check-if-my-rigidbody-player-is-grounded.html
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, m_distToGround + 0.1f);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Climb" && Input.GetKey(key_jump))
        {
            print("Triggered climb");
            StartCoroutine(Climb());
        }
    }

    IEnumerator Climb()
    {
        m_hasControl = false;
        com_rigidbody.useGravity = false;

        // Code helped created with this forum post: https://answers.unity.com/questions/21909/rounding-rotation-to-nearest-90-degrees.html
        // Round to nearest 90 degrees
        Vector3 vec = transform.eulerAngles;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        transform.eulerAngles = vec;

        // Move player
        float climb_height = 1.5f;
        float ascent_rate = 0.5f;
        float sec = 0.1f;

        Vector3 startPos = transform.position;
        Vector3 moveTo = startPos;
        startPos.x += climb_height * transform.forward.x;
        startPos.z += climb_height * transform.forward.z;

        Instantiate(GameObject.CreatePrimitive(2))

        while (transform.position.y < startPos.y + climb_height)
        {
            yield return new WaitForSeconds(sec);
            transform.Translate(new Vector3(0.0f, ascent_rate * Time.deltaTime, 0.0f));
        }

        while (Vector3.Distance(transform.position, moveTo) > 1.0f)
        {
            yield return new WaitForSeconds(sec);
            transform.position += transform.forward * ascent_rate;
        }

        m_hasControl = true;
        com_rigidbody.useGravity = true;
        yield return null;
    }
}
