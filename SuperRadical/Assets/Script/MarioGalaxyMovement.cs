using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarioGalaxyMovement : MonoBehaviour
{
    [Header ("Movement")]

    public float moveSpeed = 8f;
    public float walkSpeed;
    public float sprintSpeed;

    public float turnSpeed = 1500f;
    public float jumpForce = 10f;
    public float deceleration = 10f;
    public float maxVelocity = 8f;

    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Vector3 direction;
    private Vector3 currentVelocity;
    

    [Header ("References")]
    private Rigidbody rb;
    private GravityBody gravityBody;
    private RailGrind railGrind;
    private WallRun wallRun;
    private DriftKart driftKart;
    public MovementState state;

    [Header ("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftAlt;

    public bool isGrounded;
    public bool canMove = true;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        gravityBody = transform.GetComponent<GravityBody>();
        railGrind = transform.GetComponent<RailGrind>();
        wallRun = transform.GetComponent<WallRun>();
        driftKart = transform.GetComponent<DriftKart>();
    }

    void Update()
    {
        // Get input direction
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Jump logic
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.AddForce(-gravityBody.GravityDirection * jumpForce, ForceMode.Impulse);
        }

        StateHandler();
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    void FixedUpdate()
    {
        // Smooth deceleration
        if (direction.magnitude > 0.1f)
        {
            // Accelerate in the input direction
            Vector3 targetVelocity = transform.forward * direction.z * moveSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Gradually decelerate when no input
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Clamp velocity
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxVelocity);

        if(canMove)
        {
            // Apply movement
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        }

        // Smooth rotation
        if (direction.magnitude > 0.1f)
        {
            Quaternion rightDirection = Quaternion.Euler(0f, direction.x * (turnSpeed * Time.fixedDeltaTime), 0f);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, rb.rotation * rightDirection, Time.fixedDeltaTime * 3f);
            rb.MoveRotation(newRotation);
        } 
    
    }
    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        grinding,
        air,
        drifting
    }
    private void StateHandler()
    {
        //Mode - Wallrunning
        if(wallRun.isWallRunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallRun.wallRunSpeed;
            return;
        }
        //Mode - RailGrinding
        if(railGrind.isGrinding)
        {
            state = MovementState.grinding;
            return;
        }

        //Mode - Drifting
        if(driftKart.isDrifting)
        {
            state = MovementState.drifting;
            return;
        }

        //Mode - Sprinting
        if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        //Mode - walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        //Mode - air
        else
        {
            state = MovementState.air;

        }
    }
}
