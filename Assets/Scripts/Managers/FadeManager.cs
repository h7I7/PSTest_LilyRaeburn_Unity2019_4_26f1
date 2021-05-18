////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: FadeManager.cs
// Description: Manages game fading
// Date Created: 11/05/2021
// Last Edit: 11/05/2021
// Comments: Singleton class
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A nullable structure that makes passing fader info into scene loading functions easier
public class FadeInfo
{
    public float alpha1;
    public float alpha2;
    public float speed;
    public bool blockRaycast;

    public FadeInfo(float a_alpha1, float a_alpha2, float a_speed = 1f, bool a_blockRaycast = true)
    {
        alpha1 = a_alpha1;
        alpha2 = a_alpha2;
        speed = a_speed;
        blockRaycast = a_blockRaycast;
    }
}

[RequireComponent(typeof(GameManager))]
public class FadeManager : MonoBehaviour
{
    #region Variables
    // The overlay image for fading
    [SerializeField] private Image m_imageFader = null;

    private IEnumerator m_currentRoutine = null;

    private bool m_fading = false;
    public bool Fading
    {
        get { return m_fading; }
    }
    #endregion Variables

    #region Functions
    // Fades the overlay fading image, blocks raycasting by default
    public bool Fade(float a_targetAlpha, float a_speed = 1f, bool a_blockRaycast = true)
    {
        // Return false if another fade is already running
        if (m_currentRoutine != null)
            return false;

        m_fading = true;
        m_currentRoutine = FadeIE(a_targetAlpha, a_speed, a_blockRaycast);
        StartCoroutine(m_currentRoutine);
        return true;
    }

    private IEnumerator FadeIE(float a_targetAlpha, float a_speed, bool a_blockRaycast = true)
    {
        if (a_blockRaycast)
            m_imageFader.raycastTarget = true;

        float alphaPerSecond = (a_targetAlpha - m_imageFader.color.a) / a_speed;

        // Order alphas in ascending order so that the time passed can be used to adjust the fade image alpha regardless of whether the image is fading in or out
        float startAlpha = m_imageFader.color.a;
        float minClamp = startAlpha;
        float maxClamp = a_targetAlpha;

        if (minClamp > maxClamp)
        {
            float temp = minClamp;
            minClamp = maxClamp;
            maxClamp = temp;
        }

        // Fade the image over time
        while (m_imageFader.color.a != a_targetAlpha)
        {
            Color temp = m_imageFader.color;
            temp.a = Mathf.Clamp((alphaPerSecond * Time.deltaTime) + temp.a, minClamp, maxClamp);
            m_imageFader.color = temp;
            yield return null;
        }

        m_imageFader.raycastTarget = false;
        m_fading = false;
        m_currentRoutine = null;
    }
    #endregion Functions
}
