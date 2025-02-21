using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    public float groundDrag;

    [Header ("References")]
    private Rigidbody rb;
    private GravityBody gravityBody;
    private RailGrind railGrind;
    private WallRun wallRun;
    private Drift drift;
    public MovementState state;

    [Header ("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftAlt;

    public bool isGrounded;
    public bool canMove = true;

    float horizontalInput;
    float verticalInput;

    private Vector3 groundNormal = Vector3.up;

    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        gravityBody = transform.GetComponent<GravityBody>();
        railGrind = transform.GetComponent<RailGrind>();
        wallRun = transform.GetComponent<WallRun>();
        drift = transform.GetComponent<Drift>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        // Get input direction
        //direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // Get input direction
        //verticalInput = Input.GetAxisRaw("Vertical");
        //horizontalInput = Input.GetAxisRaw("Horizontal");

        // Get input direction
        direction = new Vector3(0f, 0f, Input.GetAxisRaw("Vertical")).normalized;

        SlopeAlign();
        StateHandler();
    }

    void SlopeAlign()
    {
        // Get ground normal
        RaycastHit hit;
        if (Physics.Raycast(groundCheck.position, -transform.up, out hit, groundCheckRadius * 2f, groundLayer))
        {
            groundNormal = hit.normal;
            if(hit.collider.CompareTag("SlopedGround"))
            {
                gravityBody.useOverrideGravity = true;
            }
            else{
                gravityBody.useOverrideGravity = false;
            }
        }
        
        gravityBody.SetGravityDirection(Vector3.Slerp(gravityBody.GravityDirection, -groundNormal, Time.deltaTime * 5f), isGrounded);
    }

    void FixedUpdate()
    {
        // Smooth deceleration
        /*if (direction.magnitude > 0.1f)
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
        }*/

        /*if(canMove)
        {
            // Apply forward movement force
            Vector3 targetVelocity = transform.forward * verticalInput * moveSpeed;
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, targetVelocity, moveSpeed * Time.fixedDeltaTime);
        }

        // Smooth rotation
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            Quaternion rightDirection = Quaternion.Euler(0f, horizontalInput * (turnSpeed * Time.fixedDeltaTime), 0f);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, rb.rotation * rightDirection, Time.fixedDeltaTime * 3f);
            rb.MoveRotation(newRotation);
        }*/

        if (canMove)
        {
            MovePlayer();
            RotatePlayer();
        }

    }

    private void MovePlayer()
    {
        if (direction.magnitude > 0.1f)
        {
            // Calculate movement force
            Vector3 movementForce = transform.forward * direction.z * moveSpeed;

            // Apply force
            rb.AddForce(movementForce, ForceMode.Acceleration);
        }
        else
        {
            // Deceleration when no input is given
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Clamp velocity to max speed
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0; // Ignore vertical movement
        if (flatVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = flatVelocity.normalized * maxVelocity + Vector3.up * rb.linearVelocity.y;
        }
    }

    private void RotatePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, horizontalInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    public enum MovementState
    {
        walking,
        sprinting,
        wallrunning,
        grinding,
        air,
        drifting,
        sliding
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
        if(drift.isDrifting)
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