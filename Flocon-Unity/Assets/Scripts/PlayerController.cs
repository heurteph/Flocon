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
    [Tooltip("Player speed")]
    private float speed;

    public delegate void EndLevelHandler();
    public event EndLevelHandler EndLevelEvent;

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

    public void InitializePosition(Vector2 origin)
    {
        transform.position = origin;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FinishPosition"))
        {
            EndLevelEvent();
        }
    }
}
