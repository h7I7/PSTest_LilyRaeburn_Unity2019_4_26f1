////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: PlayerController.cs
// Description: 
// Date Created: 11/05/2021
// Last Edit: 11/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private bool m_run = false;
    [SerializeField] private float m_playerSpeed = 5.0f;
    [SerializeField] private float m_jumpStrength = 1.0f;
    [SerializeField] private float m_gravity = 0.2f;

    [SerializeField] private Vector3 m_moveDir = Vector3.zero;

    private CharacterController m_cc;
    #endregion // Variables

    #region Functions
    private void Awake()
    {
        m_cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ProcessInput();
        UpdateShaders();
    }

    private void ProcessInput()
    {
        if (!m_run)
            return;

        Vector3 playerForward = transform.TransformDirection(Vector3.forward);

        bool jump = false;
        if (m_cc.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_moveDir.y = m_jumpStrength;
            }
        }
        else
        {
            // Apply gravity
            m_moveDir.y -= m_gravity * Time.deltaTime;
        }

        m_cc.Move(m_moveDir);
    }

    private void UpdateShaders()
    {
        Shader.SetGlobalVector("_PlayerPosition", transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
    #endregion // Functions
}
