using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator m_animator;
    public GameObject m_mainMenuObject;
    public GameObject m_pauseMenu;

    bool m_paused = false;

    public void Play()
    {
        Debug.Log("Play");
        m_animator.enabled = true;

        //SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !m_mainMenuObject.active)
        {
            m_paused = !m_paused;

            if (m_paused)
            {
                Time.timeScale = 0.0f;
                m_pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1.0f;
                m_pauseMenu.SetActive(false);
            }

        }

    }

    public void ResumeGame()
    {
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        m_paused = !m_paused;
    }
}
