using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer : MonoBehaviour {

    ////////////////////////////
    /// PRIVATE VARIABLES
    ////////////////////////////

    // COMPONENTS
    private Rigidbody   com_rigidbody;

    //
    private bool        m_jumping;
    private bool        m_grabbed;
    private GameObject  m_grabbedObject;
    private float       m_distToGround;
    public float        m_stamina;

    // CONTROLS
    bool m_hasControl; // determines whether or not the player has control over the character. This is for when prolonged animations need to be performed like climbing and pushing/pulling
    // KEY CODES
    private KeyCode key_left;
    private KeyCode key_right;
    private KeyCode key_up;
    private KeyCode key_down;
    private KeyCode key_jump;
    private int key_mb_right;

    // GAMEPAD
    private float m_deadZone;

    ////////////////////////////
    /// PUBLIC VARIABLES
    ////////////////////////////
    //public Camera com_camera;
    public float m_speed;
    public float m_jumpForce;
    public float rotatespeed = 0.1f;
    public bool m_whistled;

    // OTHER
    public SphereCollider com_sphereColl;
    public GameObject m_farPlane;
    public static ControllerPlayer g_player = null;

    // Use this for initialization
    void Start ()
    {
        if (!g_player)
        {
            g_player = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        m_grabbed   = false;
        m_stamina   = 1000.0f;

        com_rigidbody   = GetComponent<Rigidbody>();
        com_sphereColl = GetComponent<SphereCollider>();
        m_jumping       = false;
        m_distToGround  = GetComponent<Collider>().bounds.extents.y;

        m_hasControl = true;

        key_left    = KeyCode.A;
        key_right   = KeyCode.D;
        key_up      = KeyCode.W;
        key_down    = KeyCode.S;
        key_jump    = KeyCode.Space;
        key_mb_right = 1;

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
            if (Input.GetMouseButtonDown(key_mb_right))
            {
                if (m_grabbed)
                {
                    m_grabbedObject.transform.parent = null;
                    m_grabbed = false;
                    m_speed = 5.0f;
                }
                else
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, transform.forward, out hit, 1.5f);
                    GameObject obj = hit.collider.gameObject;
                    if (obj.tag == "Grab")
                    {
                        obj.transform.parent = this.transform;
                        m_grabbedObject = obj;
                        m_grabbed = true;
                        m_speed = 3.0f;
                    }
                }
            }

            // Stick input
            xMovement = Input.GetAxisRaw("Horizontal");
            zMovement = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(xMovement) < m_deadZone) xMovement = 0.0f;
            if (Mathf.Abs(zMovement) < m_deadZone) zMovement = 0.0f;

            float staminaLoss = (Mathf.Abs(xMovement) + Mathf.Abs(zMovement)) * 0.1f;

            // Jump
            if ((Input.GetKeyDown(key_jump) /*|| Input.GetButtonDown(btn_jump)*/) && IsGrounded() && !m_grabbed)
            {
                com_rigidbody.AddForce(0, m_jumpForce, 0);
                staminaLoss += 1.5f;
            }

            // Code helped created from this forum post: https://answers.unity.com/questions/803365/make-the-player-face-his-movement-direction.html

            Vector3 movement = new Vector3(xMovement, 0.0f, zMovement);

            if (m_grabbed)
            {
                //if (transform.rotation.y % 90 != 0) transform.eulerAngles = new Vector3(transform.rotation.x, Mathf.Round(transform.rotation.y / 90) * 90, transform.rotation.z);
            }
            // If the movement vector is zero, set rotation to itself, else slerp towards the direction of movement
            else transform.rotation = (movement == Vector3.zero) ? transform.rotation : Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
            transform.Translate(movement * m_speed * Time.deltaTime, Space.World);

            m_stamina -= staminaLoss * Time.deltaTime;
        }

        m_farPlane.transform.position = new Vector3(transform.position.x, 0.0f, 32.0f);
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
            StartCoroutine(Climb(trigger));
        }
    }

    IEnumerator Climb(Collider trigger)
    {
        m_hasControl = false;
        com_rigidbody.useGravity = false;
        com_rigidbody.isKinematic = true;

        // Code helped created with this forum post: https://answers.unity.com/questions/21909/rounding-rotation-to-nearest-90-degrees.html
        // Round to nearest 90 degrees
        Vector3 vec = transform.eulerAngles;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        transform.eulerAngles = vec;

        // Move player
        //float climb_height = 1.5f;
        float climb_height = (trigger.transform.position.y + 1.00f) - transform.position.y;
        float ascent_rate = 4.0f;
        float sec = 0.2f;

        Vector3 startPos = transform.position;
        Vector3 moveTo = startPos;
        //moveTo.x += climb_height * transform.forward.x;
        //moveTo.z += climb_height * transform.forward.z;
        moveTo += transform.forward * 1.5f;

        //GameObject prim = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        //prim.transform.position = moveTo;

        com_rigidbody.isKinematic = false;

        while (transform.position.y < startPos.y + climb_height)
        {
            //transform.Translate(new Vector3(0.0f, ascent_rate * Time.deltaTime, 0.0f));
            com_rigidbody.AddForce(transform.up * 20.0f);
            yield return new WaitForSeconds(sec);
        }

        while (Mathf.Abs(moveTo.x - transform.position.x) > 0.2f)
        {
            //transform.position += transform.forward * ascent_rate * Time.deltaTime;
            //transform.position.Set(transform.position.x, startPos.y + climb_height, transform.position.z);
            com_rigidbody.AddForce(-transform.up * 20.0f);
            com_rigidbody.AddForce(transform.forward * 30.0f);
            yield return new WaitForSeconds(sec);
        }

        m_hasControl = true;
        com_rigidbody.useGravity = true;
        com_rigidbody.isKinematic = false;
        yield return null;
    }
}
