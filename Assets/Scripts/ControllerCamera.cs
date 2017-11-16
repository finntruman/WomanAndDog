using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCamera : MonoBehaviour {

    public GameObject m_player;

    public float m_xPos;
    public float m_yPos;
    public float m_zPos;

    public float m_xDifference;
    public float m_moveThresh = 2.0f;
    public float speed = 4.95f;

    void Start()
    {
        m_xPos = 0.0f;
        m_yPos = 5.5f;
        m_zPos = -13.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 target = new Vector3(m_player.transform.position.x, m_yPos, m_zPos);

        // Camera dead zone
        m_xDifference = m_player.transform.position.x - transform.position.x; // Check what the x difference is betwene player and camera
        if (Mathf.Abs(m_xDifference) >= m_moveThresh) // if player is too far to either side...
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime); // follow them
        }
    }
}
