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

/// <summary>
/// A nullable struct that makes passing fader info into scene loading functions easier
/// </summary>
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
    #endregion // Variables

    #region Functions
    /// <summary>
    /// Fades the overlay fading image, blocks raycasting by default
    /// </summary>
    /// <param name="a_alpha"></param>
    /// <param name="a_speed"></param>
    /// <param name="a_blockRaycast"></param>
    /// <returns></returns>
    public bool Fade(float a_alpha, float a_speed = 1f, bool a_blockRaycast = true)
    {
        // Return false if another fade is already running
        if (m_currentRoutine != null)
            return false;

        m_fading = true;
        m_currentRoutine = FadeIE(a_alpha, a_speed, a_blockRaycast);
        StartCoroutine(m_currentRoutine);
        return true;
    }

    /// <summary>
    /// Fades the overlay fading image, blocks raycasting by default
    /// </summary>
    /// <param name="a_alpha"></param>
    /// <param name="a_speed"></param>
    /// <param name="a_blockRaycast"></param>
    /// <returns></returns>
    private IEnumerator FadeIE(float a_alpha, float a_speed, bool a_blockRaycast = true)
    {
        if (a_blockRaycast)
            m_imageFader.raycastTarget = true;

        float alphaPerSecond = (a_alpha - m_imageFader.color.a) / a_speed;

        float startAlpha = m_imageFader.color.a;
        float minClamp = startAlpha;
        float maxClamp = a_alpha;

        if (minClamp > maxClamp)
        {
            float temp = minClamp;
            minClamp = maxClamp;
            maxClamp = temp;
        }

        while (m_imageFader.color.a != a_alpha)
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
    #endregion // Functions
}
