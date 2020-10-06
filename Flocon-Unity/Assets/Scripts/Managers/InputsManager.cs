using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputsManager : MonoBehaviour
{
    private static Inputs inputs;

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton
        if (inputs == null)
        {
            inputs = new Inputs();
        }
        else
        {
            Destroy(this);
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
        inputs.Player.Disable();
    }
}
