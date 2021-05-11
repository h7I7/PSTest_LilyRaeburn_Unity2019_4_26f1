//////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: GameManager.cs
// Description: Manages game variables, functions, and states.
// Date Created:    11/05/2021
// Last Edit:       11/05/2021
// Comments: Singleton class
//////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables

    // Handles for singleton manager objects
    private static GameManager m_gameManager = null;
    public static GameManager m_GameManager
    {
        get { return m_gameManager; }
    }

    private static FadeManager m_fadeManager = null;
    public static FadeManager m_FadeManager
    {
        get { return m_fadeManager; }
    }

    #endregion // Variables

    #region Functions

    private void Awake()
    {
        Initialise();
    }

    /// <summary>
    /// Setting handles to manager objects
    /// </summary>
    private void Initialise()
    {
        // Checking and setting the GameManager
        if (m_gameManager != null)
            Destroy(gameObject);
        m_gameManager = this;
        DontDestroyOnLoad(gameObject);

        // Setting other manager object handles
        m_fadeManager = GetComponent<FadeManager>();
    }

    private void Start()
    {
        LoadScene(1, new FadeInfo(1f, 0f), false);
    }

    /// <summary>
    /// Loading scenes packed into one function
    /// </summary>
    /// <param name="a_scene">The scene build index to load</param>
    /// <param name="a_fadeInfo">Fader info if we need fading</param>
    /// <param name="a_additive">Do we want to load additively?</param>
    public void LoadScene(int a_scene, FadeInfo a_fadeInfo, bool a_additive = true)
    {
        StartCoroutine(LoadSceneIE(a_scene, a_fadeInfo, a_additive));
    }

    /// <summary>
    /// The IEnumerator for loading scenes
    /// </summary>
    /// <param name="a_scene">The scene build index to load</param>
    /// <param name="a_fadeInfo">Fader info if we need fading</param>
    /// <param name="a_additive">Do we want to load additively?</param>
    private IEnumerator LoadSceneIE(int a_scene, FadeInfo a_fadeInfo, bool a_additive)
    {
        // Start pre-loading scene
        LoadSceneMode mode;
        if (a_additive)
            mode = LoadSceneMode.Additive;
        else
            mode = LoadSceneMode.Single;

        AsyncOperation async = SceneManager.LoadSceneAsync(a_scene, mode);
        async.allowSceneActivation = false;

        // Fading the screen out
        if (a_fadeInfo != null)
        {
            // Waiting for other fades to finish first
            while (m_fadeManager.Fading)
                yield return null;

            m_fadeManager.Fade(a_fadeInfo.alpha1, a_fadeInfo.speed, a_fadeInfo.blockRaycast);

            // Wait for the fade before loading
            while (m_fadeManager.Fading)
                yield return null;
        }

        // Scene loading
        while (async.progress < 0.9f)
            yield return null;
        async.allowSceneActivation = true;

        yield return null;

        // Fading the screen in
        if (a_fadeInfo != null)
        {
            while (m_fadeManager.Fading)
                yield return null;

            m_fadeManager.Fade(a_fadeInfo.alpha2, a_fadeInfo.speed, a_fadeInfo.blockRaycast);

            while (m_fadeManager.Fading)
                yield return null;
        }
    }

    #endregion // Functions
}
