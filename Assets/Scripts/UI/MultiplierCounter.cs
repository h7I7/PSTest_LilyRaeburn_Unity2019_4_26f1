////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: MultiplierCounter.cs
// Description: 
// Date Created: 17/05/2021
// Last Edit: 17/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MultiplierCounter : MonoBehaviour
{
    #region Variables
    [SerializeField] private GemCounter m_gemCounter;
    [SerializeField] private Transform m_playerTransform;
    [SerializeField] private float m_multiplierAdjust;

    private TextMeshProUGUI m_counter;

    private float m_multiplier = 1f;
    public float Multiplier
    {
        get { return m_multiplier; }
    }
    #endregion Varaibles

    #region Functions
    private void Awake()
    {
        m_counter = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (m_gemCounter == null || m_playerTransform == null)
            return;

        m_multiplier += (m_gemCounter.Gems * m_multiplierAdjust) / (Mathf.Deg2Rad * Vector3.Distance(Vector3.zero, m_playerTransform.position)) * Time.deltaTime;
        m_counter.text = "X" + m_multiplier.ToString("F2");
    }
    #endregion Functions
}
