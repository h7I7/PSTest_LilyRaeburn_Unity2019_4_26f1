////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: StartGame.cs
// Description: Start game button
// Date Created: 11/05/2021
// Last Edit: 11/05/2021
// Comments: 
////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.EventSystems;

public class StartGame : MonoBehaviour, IPointerDownHandler
{
    // When the button is clicked load the level1 scene
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.m_GameManager)
        {
            GameManager.m_GameManager.LoadScene(2, new FadeInfo(1f, 0f), false);
        }
    }
}
