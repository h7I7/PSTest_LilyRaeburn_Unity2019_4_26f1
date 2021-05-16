////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: PlayerController.cs
// Description: 
// Date Created: 11/05/2021
// Last Edit: 16/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    private enum PlayerStates
    {
        idle,
        running,
        hit
    }
    [SerializeField] private PlayerStates m_playerState = PlayerStates.idle;
    [SerializeField] private float m_playerRunSpeed = 5.0f;
    [SerializeField] private float m_playerMoveSpeed = 5.0f;
    [SerializeField] private float m_jumpStrength = 1.0f;
    [SerializeField] private float m_jumpThreshold = 0.05f;
    [SerializeField] private float m_gravity = 0.2f;
    [SerializeField] private float m_hitForceMultiplier = 5f;
    [SerializeField] private float m_hitGravityMultiplier = 0.5f;
    [SerializeField] private float m_flashFrequency = 0.25f;
    [SerializeField] private float m_flashes = 3;

    [SerializeField] private Vector3 m_moveDir = Vector3.zero;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Camera m_camera;

    [SerializeField] private GemCounter m_gemCounter;

    [SerializeField] private Renderer m_renderer;

    private CharacterController m_cc;
    private Vector3 m_mousePositionPrevious = Vector3.zero;
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
        ProcessAnimation();
    }

    private void ProcessInput()
    {
        switch(m_playerState)
        {
            case PlayerStates.idle:
                {
                    if (Input.GetMouseButtonDown(0))
                        m_playerState = PlayerStates.running;
                    return;
                }

            case PlayerStates.running:
                {
                    Vector3 playerForward = transform.TransformDirection(Vector3.forward);
                    Vector3 playerRight = transform.TransformDirection(Vector3.right);

                    Vector3 mousePos = Input.mousePosition;
                    Vector3 playerInScreenSpace = m_camera.WorldToScreenPoint(transform.position);
                    Vector3 playerToMouse = mousePos - playerInScreenSpace;

                    Vector3 moveDir = (playerForward * m_playerRunSpeed) + ((playerRight * playerToMouse.x * m_playerMoveSpeed) / m_camera.pixelWidth);

                    m_moveDir.x = moveDir.x;
                    m_moveDir.z = moveDir.z;

                    if (m_cc.isGrounded)
                    {
                        float mousePosDifference = Mathf.Max(mousePos.y - m_mousePositionPrevious.y, 0f) / m_camera.pixelHeight;
                        if (mousePosDifference > m_jumpThreshold)
                        {
                            m_animator.Play("jump-up");
                            m_moveDir.y = m_jumpStrength;
                        }
                    }
                    else
                    {
                        m_moveDir.y -= m_gravity;
                    }

                    m_cc.Move(m_moveDir * Time.deltaTime);

                    m_mousePositionPrevious = mousePos;

                    return;
                }
            case PlayerStates.hit:
                {
                    return;
                }
        }
    }

    private void UpdateShaders()
    {
        Shader.SetGlobalVector("_PlayerPosition", transform.position);
    }

    private void ProcessAnimation()
    {
        switch(m_playerState)
        {
            case PlayerStates.idle:
                {
                    AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
                    if (!info.IsName("idle"))
                        m_animator.Play("idle");
                    return;
                }
            case PlayerStates.running:
                {
                    AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
                    if (m_cc.isGrounded && !info.IsName("run") && !info.IsName("jump-down"))
                    {
                        m_animator.Play("jump-down");
                    }
                    return;
                }
            case PlayerStates.hit:
                {
                    return;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Gem":
                {
                    m_gemCounter.CollectGem(other.transform);
                    other.enabled = false;
                    return;
                }
            case "Obstacle":
                {
                    if (m_playerState == PlayerStates.running)
                        PlayerHit();
                    else if (m_playerState == PlayerStates.hit)
                        foreach(Collider c in other.GetComponents<Collider>())
                            c.enabled = false;
                    return;
                }
            default: return;
        }
    }

    private void PlayerHit()
    {
        StartCoroutine(PlayerHitIE());
    }

    private IEnumerator PlayerHitIE()
    {
        m_playerState = PlayerStates.hit;

        m_animator.Play("float-hit");

        Vector3 moveDir = transform.TransformDirection(Vector3.back) * m_hitForceMultiplier;
        moveDir.y += m_jumpStrength;

        float time = Time.time;
        int flashCount = 0;
        while (flashCount < m_flashes * 2)
        {
            // Movement
            m_cc.Move(moveDir * Time.deltaTime);

            moveDir.x *= 0.99f;
            moveDir.z *= 0.99f;
            if (m_cc.isGrounded)
                moveDir.y = 0f;
            else
                moveDir.y -= m_gravity * m_hitGravityMultiplier;

            // Animation
            float newTime = Time.time - time;
            if (newTime > m_flashFrequency)
            {
                time = Time.time;
                m_renderer.enabled = !m_renderer.enabled;
                ++flashCount;
            }
            
            // Wait 1 frame
            yield return 0;
        }

        m_playerState = PlayerStates.running;
    }
    #endregion // Functions
}
