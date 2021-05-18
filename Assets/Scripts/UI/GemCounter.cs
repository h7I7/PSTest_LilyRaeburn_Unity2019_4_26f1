////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: GemCounter.cs
// Description: Displays the amount of gems collected and moves gems in world space to the position of the UI object on the screen
// Date Created: 13/05/2021
// Last Edit: 13/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GemCounter : MonoBehaviour
{
    #region Variables
    [SerializeField] private Camera m_camera = null;

    private TextMeshProUGUI m_counter = null;
    private int m_gems = 0;
    public int Gems
    { 
        get { return m_gems; }
    }
    #endregion Variables

    #region Functions
    private void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        m_counter = GetComponent<TextMeshProUGUI>();
    }

    // Animate the gem from its current position to the gem counters position
    public void CollectGem(Transform a_gem)
    {
        StartCoroutine(CollectGemIE(a_gem));
    }

    private IEnumerator CollectGemIE(Transform a_gem)
    {
        // Parent the gem to the camera to make things easier
        a_gem.transform.SetParent(m_camera.transform);

        // The final position of the gem in world space
        float distance = Vector3.Distance(a_gem.position, m_camera.transform.position);
        Ray r = m_camera.ScreenPointToRay(m_counter.transform.position);
        Vector3 target = r.direction * distance;

        // Move the gem
        float maxVelocity = 0f;
        while (Vector3.Distance(a_gem.position, target + m_camera.transform.position) > 0.1f)
        {
            maxVelocity += Time.deltaTime;
            a_gem.position = Vector3.MoveTowards(a_gem.position, target + m_camera.transform.position, maxVelocity);

            yield return null;
        }

        // Destroy the gem object and add 1 to the counter
        Destroy(a_gem.gameObject);
        AddGem();
    }

    private void AddGem()
    {
        ++m_gems;
        m_counter.text = m_gems.ToString();
    }
    #endregion Functions
}
