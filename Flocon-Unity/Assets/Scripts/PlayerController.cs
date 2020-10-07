using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* External references */

    private GameObject inputsManager;

    private GameObject raycastOrigin;
    //private GameObject feetLevel;

    /* Settings */

    [SerializeField]
    [Range(0.01f,10f)]
    [Tooltip("Player speed")]
    private float xSpeed = 5;

    [SerializeField]
    [Range(0f,10f)]
    [Tooltip("The gravitational acceleration applied to the player")]
    private float gravityAcceleration = 0.5f;

    private float ySpeed;

    private int groundMask;

    private bool isGrounded;

    public delegate void EndLevelHandler();
    public event EndLevelHandler EndLevelEvent;

    private void Awake()
    {
        raycastOrigin = transform.GetChild(0).gameObject;
        Debug.Assert(raycastOrigin != null, "No origin point found for the raycast");

        //feetLevel = transform.GetChild(1).gameObject;
        //Debug.Assert(feetLevel != null, "No point found for the ground level");

        groundMask = LayerMask.GetMask("Ground");

        ySpeed = 0;
        isGrounded = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputsManager = GameObject.FindGameObjectWithTag("InputsManager");
        Debug.Assert(inputsManager != null, "Missing inputs manager");
    }

    // Update is called once per frame
    void Update()
    {
        Fall();
        Walk();
    }

    private void Walk()
    {
        // Get inputs
        float xInput = inputsManager.GetComponent<InputsManager>().ReadWalk();
        Vector2 velocity = new Vector2(xInput, 0);

        // Follow the terrain only if on the ground
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.transform.position, -Vector2.up, Mathf.Infinity, groundMask);
            if (hit.collider != null)
            {
                Vector3 groundNormal = hit.normal;
                Vector2 groundTangent = -Vector2.Perpendicular(groundNormal); // 90 degrees clockwise
                velocity = groundTangent.normalized * xInput; // Take the sign into account
            }
        }

        transform.Translate(velocity * xSpeed * Time.deltaTime, Space.World);
    }

    private void Fall()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.transform.position, -Vector2.up, Mathf.Infinity, groundMask);
        if (hit.collider != null)
        {
            Debug.DrawRay(hit.point, Vector2.right, Color.yellow, 1, true);
            Debug.DrawRay(hit.point, Vector2.up, Color.yellow, 1, true);
            Debug.DrawLine(raycastOrigin.transform.position, hit.point, Color.red, 1, true);

            Vector2 groundLevel = hit.point;
            //Debug.Log("FeetLevel " + feetLevel.transform.position.y + " and GroundLevel " + groundLevel.y);

            if (transform.position.y - groundLevel.y > Mathf.Epsilon)
            {
                // Fall
                isGrounded = false;
                ySpeed += gravityAcceleration;
                transform.Translate(ySpeed * -Vector2.up * Time.deltaTime, Space.World);
            }
            else
            {
                // Reset position, reset fall speed
                isGrounded = true;
                transform.position = new Vector2(transform.position.x, groundLevel.y);
                //Debug.Log("Feet level is now at " + feetLevel.transform.position);
                ySpeed = 0;
            }
        }
        else
        {
            // Fall into the void
            isGrounded = false;
            ySpeed += gravityAcceleration;
            transform.Translate(ySpeed * -Vector2.up * Time.deltaTime, Space.World);
        }
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
