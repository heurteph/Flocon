using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* External references */

    private GameObject inputsManager;

    /* Settings */

    [SerializeField]
    [Range(0.01f,10f)]
    [Tooltip("Speed")]
    private float speed;

    private void Awake()
    {

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
        //Debug.Log("Read move : " + movement);
        Vector2 input = new Vector2(movement, 0);

        transform.Translate(input * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player collided with " + collision.otherCollider.name);
    }
}
