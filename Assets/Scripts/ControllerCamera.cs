using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCamera : MonoBehaviour {

    public GameObject m_player;

    public float m_xPos;
    public float m_yPos;
    public float m_zPos;
    public float m_xRot;

    public float m_xDifference;
    public float m_yDifference;
    public float m_xMoveThresh = 2.0f;
    public float m_yMoveThresh = 2.0f;
    public float speed = 4.95f;

    void Start()
    {
        m_xPos = 0.0f;
        m_yPos = 5.1f;
        m_zPos = -13.0f;

        m_xRot = 15.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        //Vector3 xTarget = new Vector3(m_player.transform.position.x, m_yPos, m_zPos);
        //Vector3 yTarget = new Vector3(m_xPos, m_player.transform.position.y, m_zPos);

        //// Camera dead zone
        //m_xDifference = m_player.transform.position.x - transform.position.x; // Check what the x difference is betwene player and camera
        //m_yDifference = m_player.transform.position.y - transform.position.y + 4.0f;
        //if (Mathf.Abs(m_xDifference) >= m_xMoveThresh) // if player is too far to either side...
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, xTarget, speed * Time.deltaTime); // follow them
        //}
        //if (Mathf.Abs(m_yDifference) >= m_yMoveThresh)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, yTarget, speed * Time.deltaTime);
        //}

        m_xDifference = m_player.transform.position.x - transform.position.x; // Check what the x difference is betwene player and camera
        m_yDifference = m_player.transform.position.y - transform.position.y + 5.0f;

        float xTarget = (Mathf.Abs(m_xDifference) >= m_xMoveThresh) ? m_player.transform.position.x : transform.position.x;
        float yTarget = (Mathf.Abs(m_yDifference) >= m_yMoveThresh) ? m_player.transform.position.y + 4.0f : transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(xTarget, yTarget, transform.position.z), speed * Time.deltaTime);
    }
}
