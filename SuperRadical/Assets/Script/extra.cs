using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class extra : MonoBehaviour
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

    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header ("References")]
    private Rigidbody rb;
    private GravityBody gravityBody;
    private RailGrind railGrind;
    private WallRun wallRun;
    private Drift drift;

    [Header ("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftAlt;

    public bool isGrounded;
    public bool canMove = true;

    float horizontalInput;
    float verticalInput;

    void Start()
    {

    }
/*
    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if(isGrounded)
        {
            rb.linearDamping = groundDrag;
        } else {
            rb.linearDamping = groundDrag;
        }

        SpeedControl();
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }

    void FixedUpdate()
    {

        float turnInput = Input.GetAxisRaw("Horizontal"); // -1 for A, 1 for D, 0 otherwise

        if (turnInput != 0)
        {
            float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
        MovePlayer();
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); 
            return angle < maxSlopeAngle && angle != 0; 
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(currentVelocity, slopeHit.normal).normalized;
    }

    private void MovePlayer()
    {
        float forwardInput = Mathf.Max(0, verticalInput); // Ignore negative input (S key)
        Vector3 moveDirection = transform.forward * forwardInput;
        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
    }
    
    
    
    
    
    
    
    
    
    // Get ground normal
            RaycastHit hit;
            if (Physics.Raycast(groundCheck.position, -transform.up, out hit, groundCheckRadius * 2f, groundLayer))
            {
                groundNormal = hit.normal;
            }
        

        gravityBody.SetGravityDirection(Vector3.Slerp(gravityBody.GravityDirection, -groundNormal, Time.deltaTime * 5f), isGrounded);
    
    
    private Vector3 overrideGravityDirection = Vector3.zero;
    private bool overrideGravity = false;

    public void SetGravityDirection(Vector3 newDirection, bool overrideGravity)
    {
        this.overrideGravity = overrideGravity;
        overrideGravityDirection = newDirection.normalized;
    }

    public Vector3 GravityDirection
    {
        get
        {
            if (overrideGravity) return overrideGravityDirection;
            if (_gravityAreas.Count == 0) return Vector3.zero;
            _gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            return _gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    */

}
