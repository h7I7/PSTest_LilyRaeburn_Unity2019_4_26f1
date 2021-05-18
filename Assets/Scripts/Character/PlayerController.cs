////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: PlayerController.cs
// Description: Manages user input, character physics, and collision interactions
// Date Created: 11/05/2021
// Last Edit: 16/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Variables
    private enum PlayerStates
    {
        menu,
        idle,
        running,
        hit
    }

    [SerializeField] private PlayerStates m_playerState = PlayerStates.idle;

    [Space(10f)]
    [Header("Movement")]
    [SerializeField] private float m_playerRunSpeed = 5.0f;
    [SerializeField] private float m_playerMoveSpeed = 5.0f;
    [SerializeField] private float m_jumpStrength = 1.0f;
    [SerializeField] private float m_swipeThreshold = 0.05f;
    [SerializeField] private float m_gravity = 45f;
    // These variables apply when the player takes damage
    [SerializeField] private float m_hitForceMultiplier = 5f;
    [SerializeField] private float m_hitGravityMultiplier = 0.5f;

    [Space(10f)]
    [Header("Animation")]
    [SerializeField] private float m_flashFrequency = 0.25f;
    [SerializeField] private float m_flashes = 3;

    [SerializeField] private Animator m_animator = null;

    [Space(10f)]
    [Header("Counters")]
    [SerializeField] private GemCounter m_gemCounter = null;
    [SerializeField] private LifeCounter m_lifeCounter = null;
    [SerializeField] private MultiplierCounter m_multiplierCounter = null;

    [Space(10f)]
    [Header("Rendering")]
    [SerializeField] private Camera m_camera = null;
    [SerializeField] private Renderer m_renderer = null;

    private CharacterController m_cc = null;
    private Vector3 m_moveDir = Vector3.zero; // Applied to the character every frame
    private Vector3 m_mousePositionPrevious = Vector3.zero;
    #endregion Variables

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
            case PlayerStates.menu: break;
            case PlayerStates.idle:
                {
                    if (Input.GetMouseButtonDown(0))
                        m_playerState = PlayerStates.running;
                    break;
                }

            case PlayerStates.running:
                {
                    Vector3 playerForward = transform.TransformDirection(Vector3.forward);
                    Vector3 playerRight = transform.TransformDirection(Vector3.right);

                    Vector3 mousePos = Input.mousePosition;
                    Vector3 playerInScreenSpace = m_camera.WorldToScreenPoint(transform.position);
                    Vector3 playerToMouse = mousePos - playerInScreenSpace;

                    // Move the player based on the distance from the player to the mouse on the x axis in screen space
                    // I intended to add turns to the level, however since the world is straight some of these calculations might be redundant
                    Vector3 moveDir = (playerForward * m_playerRunSpeed * m_multiplierCounter.Multiplier) + ((playerRight * playerToMouse.x * m_playerMoveSpeed) / m_camera.pixelWidth);

                    m_moveDir.x = moveDir.x;
                    m_moveDir.z = moveDir.z;

                    if (m_cc.isGrounded)
                    {
                        // Checking for a swipe up
                        float mousePosDifference = Mathf.Max(mousePos.y - m_mousePositionPrevious.y, 0f) / m_camera.pixelHeight;
                        if (mousePosDifference > m_swipeThreshold)
                        {
                            m_animator.Play("jump-up");
                            m_moveDir.y = m_jumpStrength;
                        }
                    }
                    else
                    {
                        m_moveDir.y -= m_gravity * Time.deltaTime;
                    }

                    m_cc.Move(m_moveDir * Time.deltaTime);

                    m_mousePositionPrevious = mousePos;
                    break;
                }
            case PlayerStates.hit:
                {
                    // Do not process input when player is in the hit state
                    break;
                }
        }
    }

    private void UpdateShaders()
    {
        // Setting variables used in the WorldCurve shader
        switch(m_playerState)
        {
            case PlayerStates.menu:
                {
                    // When we are in the menu we do not want any adjustments to the vertex positions of the world geometry
                    Shader.SetGlobalVector("_PlayerPosition", new Vector3(0, 0, 0));
                    break;
                }
            default:
                {
                    // Any other time we want to set the play position in the shader so that the world curves as the player moves
                    Shader.SetGlobalVector("_PlayerPosition", transform.position);
                    break;
                }
        }
    }

    private void ProcessAnimation()
    {
        switch(m_playerState)
        {
            case PlayerStates.menu: break;
            case PlayerStates.idle:
                {
                    // Play idle animation
                    AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
                    if (!info.IsName("idle"))
                        m_animator.Play("idle");
                    break;
                }
            case PlayerStates.running:
                {
                    // When grounded play the jump-down animation (which leads to the run animation) if jump-down and run are not currently being played
                    AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
                    if (m_cc.isGrounded && !info.IsName("run") && !info.IsName("jump-down"))
                    {
                        m_animator.Play("jump-down");
                    }
                    break;
                }
            case PlayerStates.hit:
                {
                    // When hit do not change animation
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Gem":
                {
                    // When colliding with a gem have the gem counter collect it and disable the gems collider
                    m_gemCounter.CollectGem(other.transform);
                    other.enabled = false;
                    break;
                }
            case "Obstacle":
                {
                    // When colliding with an obstacle have the player take damage
                    // If the player is already in the hit animation disable the collider so the player doesn't take too much damage
                    if (m_playerState == PlayerStates.running)
                        PlayerHit();
                    else if (m_playerState == PlayerStates.hit)
                        foreach(Collider c in other.GetComponents<Collider>())
                            c.enabled = false;
                    break;
                }
            default: break;
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

        m_lifeCounter.RemoveLife();

        // Knock the player back to give the damage some impact
        Vector3 moveDir = transform.TransformDirection(Vector3.back) * m_hitForceMultiplier;
        moveDir.y += m_jumpStrength;

        // Manages the player animations and position whilst in the hit state
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

        // If the player has more lives keep running, otherwise go back to the menu
        if (m_lifeCounter.Lives > 0)
            m_playerState = PlayerStates.running;
        else
        {
            if (GameManager.m_GameManager != null)
            {
                GameManager.m_GameManager.LoadScene(1, new FadeInfo(1f, 0f), false);
                GameManager.m_GameManager.SetHighscore(m_gemCounter.Gems);
            }
        }
    }
    #endregion Functions
}
