using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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


    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("Walking min speed")]
    private float xMinSpeed = 0f;

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
    [Range(0.5f, 2f)]
    [Tooltip("Walk -> Idle animation transition duration when character is at full speed")]
    private float maxWalkToIdleAnimationTransitionDuration = 1;

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

    private Vector2 currentForward;

    private FACING facing;

    private Vector2 groundNormal;
    private Vector2 groundTangent;

    public delegate void EndLevelHandler();
    public event EndLevelHandler EndLevelEvent;

    /* Animations */

    private int walkStateId;
    private int walkTransitionToIdleId;
    private float transitionTimer;

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
        facing = FACING.RIGHT;
        transitionTimer = 0;
        currentForward = playerModel.transform.forward;

        // TO DO : Set target rotation to the maxDegToTheGround if player spawn on a slope > maxDegToTheGround
        targetRotation = playerModel.transform.rotation;
        //targetRotation = Quaternion.LookRotation(Quaternion.Euler(0,0,-maxDegToTheGround) * Vector2.right, Vector2.up);
    }

    // Start is called before the first frame update
    void Start()
    {
        inputsManager = GameObject.FindGameObjectWithTag("InputsManager");
        Debug.Assert(inputsManager != null, "Missing inputs manager");

        GetStateMachineIds("marche", "idle", out walkStateId, out walkTransitionToIdleId);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Fall();
        if (facing != FACING.TRANSITION)
        {
            BalancePosture();
        }
        if(!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGameOver())
        {
            Walk();
        }
        else
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
            if(xInput < 0 && xSpeed == 0)
            {
                // turn around
                facing = FACING.TRANSITION;
                currentForward = playerModel.transform.forward; // save this for coroutine
                StartCoroutine(TurnToLeft());
            }
            else if (xInput > 0)
            {
                // moving forward
                if (xSpeed == 0) xSpeed = xMinSpeed;
                xSpeed = Mathf.Min(xSpeed + xInput * xAcceleration * Time.deltaTime, xMaxSpeed);

                /*
                // The transition duration of walk->idle is the mirror of time elapsed on idle->walk
                transitionTimer = Mathf.Min(transitionTimer + Time.deltaTime, maxWalkToIdleAnimationTransitionDuration);
                // change transition duration with threshold not to burn the CPU
                if ((int)(transitionTimer * 1000) % (maxWalkToIdleAnimationTransitionDuration * 1000 / 4) == 0)
                {
                    ChangeTransitionDuration(walkStateId, walkTransitionToIdleId, transitionTimer);
                }
                */
            }
            else // xInput == 0
            if (xSpeed > 0)
            {
                // stopping
                transitionTimer = 0;
                xSpeed = Mathf.Max(xSpeed - xDeceleration * Time.deltaTime /** xSpeed*/, 0);
            }
        }
        else if (facing == FACING.LEFT)
        {
            if(xInput > 0 && xSpeed == 0)
            {
                // turn around
                facing = FACING.TRANSITION;
                currentForward = playerModel.transform.forward; // save this for coroutine
                StartCoroutine(TurnToRight());
            }
            if (xInput < 0)
            {
                // moving forward
                if (xSpeed == 0) xSpeed = -xMinSpeed;
                xSpeed = Mathf.Max(xSpeed + xInput * xAcceleration * Time.deltaTime, -xMaxSpeed);

                /*
                // The transition duration of walk->idle is the mirror of time elapsed on idle->walk
                transitionTimer = Mathf.Min(transitionTimer + Time.deltaTime, maxWalkToIdleAnimationTransitionDuration);
                // change transition duration with threshold not to burn the CPU
                if ((int)(transitionTimer * 1000) % (maxWalkToIdleAnimationTransitionDuration * 1000 / 4) == 0)
                {
                    ChangeTransitionDuration(walkStateId, walkTransitionToIdleId, transitionTimer);
                }
                */
            }
            else // xInput == 0
            if (xSpeed < 0)
            {
                // stopping
                transitionTimer = 0;
                xSpeed = Mathf.Min(xSpeed + xDeceleration * Time.deltaTime /** -xSpeed*/, 0);
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
                /* Handle animations */

                if((xInput > 0 && facing == FACING.RIGHT) || (xInput < 0 && facing == FACING.LEFT))
                {
                    playerAnimator.SetBool("IsWalking", true);
                }
                else if(facing != FACING.TRANSITION)
                {
                    playerAnimator.SetBool("IsWalking", false);
                }

                /* Handle speed */

                groundNormal = hit.normal;
                groundTangent = -Vector2.Perpendicular(groundNormal); // 90 degrees clockwise
                float angle = Vector3.Angle(Vector2.right, groundTangent);
                velocity = groundTangent.normalized * xSpeed; // Take the sign into account
                Debug.Log("Angle of the slope : " + angle);

                /* Handle tilting */
                // TO DO : See if the rotation should not happen to the parent itself
                if (angle < maxDegToTheGround)
                {
                    if (facing == FACING.RIGHT)
                    {
                        Debug.Log("Looking at target right : " + targetRotation);
                        targetRotation = Quaternion.LookRotation(groundTangent, Vector2.up);
                    }
                    else if (facing == FACING.LEFT)
                    {
                        Debug.Log("Looking at target left : " + targetRotation);
                        targetRotation = Quaternion.LookRotation(-groundTangent, Vector2.up);
                    }
                }
                else
                {
                    Debug.Log("Too slopy, target rotation stays the same " + targetRotation);
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
            playerModel.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(currentForward, -currentForward, timer / timeToTurn), Vector2.Perpendicular(-currentForward));
            //Quaternion.RotateTowards(playerModel.transform.rotation, Vector2.left, 
            
            // update time
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        facing = FACING.RIGHT;
        targetRotation = Quaternion.Euler(-targetRotation.eulerAngles); // Inverse the tilting

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
            playerModel.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(currentForward, -currentForward, timer / timeToTurn), Vector2.Perpendicular(currentForward));
            //Quaternion.RotateTowards(playerModel.transform.rotation, Vector2.left, 

            // update time
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        facing = FACING.LEFT;
        targetRotation = Quaternion.Euler(-targetRotation.eulerAngles);  // Inverse the tilting

        playerAnimator.SetBool("IsTurning", false);
    }

    private void GetStateMachineIds(string stateFrom, string stateTo, out int stateId, out int transitionId)
    {

        // Get a reference to the Animator Controller:
        AnimatorController ac = playerAnimator.runtimeAnimatorController as AnimatorController;
        //This part is IMPORTANT ^^

        // Number of layers:
        int layerCount = ac.layers.Length;
        Debug.Log(string.Format("Layer Count: {0}", layerCount));

        // Names of each layer:
        for (int layer = 0; layer < layerCount; layer++)
        {
            Debug.Log(string.Format("Layer {0}: {1}", layer, ac.layers[layer].name));
        }

        // States on layer 0:
        AnimatorStateMachine sm = ac.layers[0].stateMachine;
        ChildAnimatorState[] states = sm.states;
        stateId = 0;
        foreach (ChildAnimatorState s in states)
        {
            Debug.Log(string.Format("State: {0}", s.state.name));
            if (s.state.name == stateFrom)
                break;
            stateId++;
        }

        transitionId = 0;
        foreach (AnimatorStateTransition t in states[stateId].state.transitions)
        {
            Debug.Log(string.Format("Transition to : {0}", t.destinationState.name));
            if (t.destinationState.name == stateTo)
                break;
            transitionId++;
        }
    }

    private void ChangeTransitionDuration(int stateId, int transitionId, float duration)
    {
        AnimatorController ac = playerAnimator.runtimeAnimatorController as AnimatorController;
        ac.layers[0].stateMachine.states[stateId].state.transitions[transitionId].duration = duration;
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
