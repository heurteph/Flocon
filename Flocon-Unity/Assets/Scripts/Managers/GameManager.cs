using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private GameObject inputsManager;
    private GameObject menuManager;

    private Vector2 start;

    private BoxCollider2D end;

    private bool isGameOver;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Awake
            inputsManager = GameObject.FindGameObjectWithTag("InputsManager");
            Debug.Assert(inputsManager != null, "Missing inputs manager");
            inputsManager.GetComponent<InputsManager>().RegisterPause(PauseGame);

            menuManager = GameObject.FindGameObjectWithTag("MenuManager");
            Debug.Assert(menuManager != null, "Missing menu manager");
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PauseGame(InputAction.CallbackContext ctx)
    {
        if (!menuManager.GetComponent<MenuManager>().IsGamePaused())
        {
            menuManager.GetComponent<MenuManager>().PauseGame();
        }
        else
        {
            menuManager.GetComponent<MenuManager>().ResumeGame();
        }
    }
}
