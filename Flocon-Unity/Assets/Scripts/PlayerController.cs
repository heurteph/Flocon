using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* External references */

    private GameObject inputsManager;

    /* Internal references */

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if(characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inputsManager = GameObject.FindGameObjectWithTag("InputsManager");
        Debug.Assert(inputsManager != null, "Missing inputs manager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Walk();
    }

    private void Walk()
    {
        float movement = inputsManager.GetComponent<InputsManager>().ReadWalk();
        Debug.Log("Read move : " + movement);
        Vector2 speed = new Vector2(movement, 0);

        GetComponent<CharacterController>().Move(speed);
    }
}
