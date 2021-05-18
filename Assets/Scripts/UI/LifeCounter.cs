////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: LifeCounter.cs
// Description: Displays the amount of lives
// Date Created: 16/05/2021
// Last Edit: 16/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LifeCounter : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_lives = 3;
    public int Lives
    { 
        get { return m_lives; }
    }

    private TextMeshProUGUI m_counter = null;
    private IEnumerator m_currentEnumerator = null; // Used for the colour animation
    #endregion Variables

    #region Functions
    private void Awake()
    {
        m_counter = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateCounter();
    }

    public void RemoveLife()
    {
        // Updating lives
        --m_lives;
        UpdateCounter();

        // Life counter colour animation
        if (m_currentEnumerator != null)
            StopCoroutine(m_currentEnumerator);

        m_currentEnumerator = TextColourTweenIE(Color.red, Color.white, 1.0f);
        StartCoroutine(m_currentEnumerator);
    }

    private void UpdateCounter()
    {
        m_counter.text = m_lives.ToString();
    }

    private IEnumerator TextColourTweenIE(Color a_start, Color a_finish, float a_speed)
    {
        m_counter.color = a_start;

        // Animate text colour over time
        float startTime = Time.time;
        while(m_counter.color != a_finish)
        {
            m_counter.color = Color.Lerp(a_start, a_finish, (Time.time - startTime) * a_speed);
            yield return 0;
        }
    }
    #endregion Functions
}
