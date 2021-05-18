////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: HighscoreCounter.cs
// Description: Displays the highscore
// Date Created: 16/05/2021
// Last Edit: 16/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HighscoreCounter : MonoBehaviour
{
    #region Variables
    private TextMeshProUGUI m_counter = null;
    #endregion Variables

    #region Functions
    private void Awake()
    {
        m_counter = GetComponent<TextMeshProUGUI>();
        // Set the highscore text to the stored highscore in GameManager
        if (GameManager.m_GameManager != null)
            m_counter.text = GameManager.m_GameManager.GetHighscore().ToString();
    }
    #endregion Functions
}
