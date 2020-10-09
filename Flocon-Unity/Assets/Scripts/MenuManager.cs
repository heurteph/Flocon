using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager instance;

    public Animator m_animator;
    public GameObject m_mainMenuObject;
    public GameObject m_pauseMenu;

    FMOD.Studio.EventInstance instanceToStop;

    bool m_paused = false;

    private void Start()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Awake
            m_animator.enabled = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        instanceToStop = FMODUnity.RuntimeManager.CreateInstance("event:/Ambiance/Ambiance");
        instanceToStop.start();
        instanceToStop.release();
    }

    public void Play()
    {
        //Debug.Log("Play");
        m_animator.SetTrigger("Disappear");
        m_paused = false;

        // TO DO : Make sure the correct scene is loaded
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartAnimator()
    {
        m_animator.SetTrigger("Restart");
        instanceToStop.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Saisons", 0);
    }

    void Update()
    {

    }

    public bool IsGamePaused()
    {
        return m_paused;
    }

    public void PauseGame()
    {
        m_pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        m_paused = true;
    }

    public void ResumeGame()
    {
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        m_paused = false;
    }
}
