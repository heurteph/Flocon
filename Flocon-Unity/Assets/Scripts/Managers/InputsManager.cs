using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
    private Inputs inputs;

    private static InputsManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Awake
            inputs = new Inputs();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TO DO : Remove
    }

    /* Register to inputs */

    public void RegisterPause(System.Action<InputAction.CallbackContext> func)
    {
        inputs.Player.Pause.performed += func;
    }

    public void UnregisterPause(System.Action<InputAction.CallbackContext> func)
    {
        inputs.Player.Pause.performed -= func;
    }

    /* Read inputs */

    public float ReadWalk()
    {
        return inputs.Player.Walk.ReadValue<float>();
    }

    private void OnEnable()
    {
        inputs.Player.Enable();
    }

    private void OnDisable()
    {
        Debug.Log("Inputs Manager disabled because of the pause");
        //inputs.Player.Disable();
    }
}
