using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FACING { RIGHT, LEFT, TRANSITION}

public class PlayerController : MonoBehaviour
{
    /* External references */

    private GameObject inputsManager;

    private GameObject raycastOrigin;
    //private GameObject feetLevel;

    private GameObject playerModel;

    private Animator playerAnimator;

    /* Settings */

    [Space]
    [Header("Linear Physics")]

    [SerializeField]
    [Range(0.01f,10f)]
    [Tooltip("Walking max speed")]
    private float xMaxSpeed = 3;

    private float xSpeed;

    [SerializeField]
    [Range(0.01f, 10f)]
    [Tooltip("Walking acceleration")]
    private float xAcceleration = 1;

    [SerializeField]
    [Range(0.01f, 10f)]
    [Tooltip("Walking deceleration")]
    private float xDeceleration = 4;

    [SerializeField]
    [Range(0f,10f)]
    [Tooltip("The gravitational acceleration applied to the player")]
    private float gravityAcceleration = 0.5f;

    [Space]
    [Header("Rotations")]

    [SerializeField]
    [Range(10f, 100f)]
    [Tooltip("Speed to adapt posture to the ground")]
    private float tiltSpeed = 50;

    [SerializeField]
    [Range(0f, 90f)]
    [Tooltip("The maximum slope angle the character can adapt its posture to")]
    private float maxDegToTheGround = 45;

    [SerializeField]
    [Range(0f, 90f)]
    [Tooltip("Speed of the character spinning when the game is over")]
    private float spinSpeed = 20;

    [SerializeField]
    [Range(0.5f, 5f)]
    [Tooltip("Time to switch side in seconds")]
    private float timeToTurn = 0.5f;

    private float ySpeed;

    private int groundMask;

    private bool isGrounded;

    private Quaternion initialOrientation;

    private Quaternion targetRotation;

    private FACING facing;

    public delegate void EndLevelHandler();
    public event EndLevelHandler EndLevelEvent;

    private void Awake()
    {
        raycastOrigin = transform.GetChild(0).gameObject;
        Debug.Assert(raycastOrigin != null, "No origin point found for the raycast");

        //feetLevel = transform.GetChild(1).gameObject;
        //Debug.Assert(feetLevel != null, "No point found for the ground level");

        playerModel = transform.GetChild(1).gameObject;
        Debug.Assert(playerModel != null, "No 3D model found for the player");
        playerAnimator = playerModel.GetComponent<Animator>();
        Debug.Assert(playerModel != null, "No Animator found for the player");

        groundMask = LayerMask.GetMask("Ground");

        xSpeed = 0;
        ySpeed = 0;
        isGrounded = false;
        initialOrientation = playerModel.transform.rotation;
        targetRotation = playerModel.transform.rotation;
        facing = FACING.RIGHT;
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
        Fall();
        Walk();
        //BalancePosture();
        if(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGameOver())
        {
            VictoryRotation();
        }
    }

    private void Walk()
    {
        // Get inputs
        float xInput = inputsManager.GetComponent<InputsManager>().ReadWalk();

        if (facing == FACING.RIGHT)
        {
            if(xInput < 0)
            {
                // turn around
                facing = FACING.TRANSITION;
                StartCoroutine(TurnToLeft());
            }
            else if (xInput > 0)
            {
                xSpeed = Mathf.Min(xSpeed + xInput * xAcceleration * Time.deltaTime, xMaxSpeed);
            }
            else // xInput == 0
            if (xSpeed > 0)
            {
                xSpeed = Mathf.Max(xSpeed - xDeceleration * Time.deltaTime, 0);
            }
        }
        else if (facing == FACING.LEFT)
        {
            if(xInput > 0)
            {
                // turn around
                facing = FACING.TRANSITION;
                StartCoroutine(TurnToRight());
            }
            if (xInput < 0)
            {
                xSpeed = Mathf.Max(xSpeed + xInput * xAcceleration * Time.deltaTime, -xMaxSpeed);
            }
            else // xInput == 0
            if (xSpeed < 0)
            {
                xSpeed = Mathf.Min(xSpeed + xDeceleration * Time.deltaTime, 0);
            }
        }

        Vector2 velocity = new Vector2(xSpeed, 0);
        //Debug.Log("xSpeed : " + xSpeed);

        // Follow the terrain only if on the ground
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.transform.position, -Vector2.up, Mathf.Infinity, groundMask);
            if (hit.collider != null)
            {
                // Handle animation
                if((xInput > 0 && facing == FACING.RIGHT) || (xInput < 0 && facing == FACING.LEFT))
                {
                    playerAnimator.SetBool("IsWalking", true);
                }
                else if(facing != FACING.TRANSITION)
                {
                    playerAnimator.SetBool("IsWalking", false);
                }

                Vector3 groundNormal = hit.normal;
                Vector2 groundTangent = -Vector2.Perpendicular(groundNormal); // 90 degrees clockwise
                velocity = groundTangent.normalized * xSpeed; // Take the sign into account

                // Orientation of the model
                // TO DO : See if the rotation should not happen to the parent itself
                float angle = Vector3.Angle(Vector2.right, groundTangent);
                if(angle < maxDegToTheGround)
                {
                    targetRotation = Quaternion.LookRotation(groundTangent, Vector2.up);
                }
                else
                {
                    // 45 degrees orientation
                    // just keep the old target rotation, that will do
                }
            }
            else // in the void
            {
                playerAnimator.SetBool("IsWalking", false);
            }
        }
        else // in the air
        {
            //playerAnimator.SetBool("IsWalking", false);
        }

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void BalancePosture()
    {
        // Add up every rotations
        float step = tiltSpeed * Time.deltaTime;
        playerModel.transform.rotation = Quaternion.RotateTowards(playerModel.transform.rotation, targetRotation, step);
    }

    private void VictoryRotation()
    {
        playerModel.transform.Rotate(Vector2.up, spinSpeed);
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

    private IEnumerator TurnToRight()
    {
        // TO DO : Turn back animation ?
        playerAnimator.SetBool("IsTurning", true);

        float timer = 0;
        while(timer < timeToTurn)
        {
            // update 3d model
            playerModel.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(Vector2.left, Vector2.right, timer / timeToTurn), Vector2.up);
            //Quaternion.RotateTowards(playerModel.transform.rotation, Vector2.left, 
            
            // update time
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        facing = FACING.RIGHT;

        playerAnimator.SetBool("IsTurning", false);
    }

    private IEnumerator TurnToLeft()
    {
        // TO DO : Turn back animation ?
        playerAnimator.SetBool("IsTurning", true);

        float timer = 0;
        while (timer < timeToTurn)
        {
            // update 3d model
            playerModel.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(Vector2.right, Vector2.left, timer / timeToTurn), Vector2.up);
            //Quaternion.RotateTowards(playerModel.transform.rotation, Vector2.left, 

            // update time
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        facing = FACING.LEFT;

        playerAnimator.SetBool("IsTurning", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FinishPosition"))
        {
            playerAnimator.SetBool("IsFinish", true);
            EndLevelEvent();
        }
    }
}
