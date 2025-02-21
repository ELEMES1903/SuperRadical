using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class SkatingMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float friction = 2f;
    public float airControlFactor = 0.5f;
    public float rotationSpeed = 100f;
    public float groundCheckDistance = 1.1f;
    public float slopeBlendSpeed = 5f;
    public LayerMask groundMask;
    
    private float rotationInput;
    private Vector3 moveInput;
    private GravityBody gravityBody;
    private Vector3 groundNormal;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
    }

    void Update()
    {
        // Get movement input
        rotationInput = Input.GetAxisRaw("Horizontal"); // A/D keys rotate
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(0f, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        CheckGround();
        AlignWithSurface();
        RotatePlayer();
        ApplyMovement();
        ApplyFriction();
    }

    void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, groundCheckDistance, groundMask))
        {
            groundNormal = hit.normal;
            isGrounded = true;
        }
        else
        {
            groundNormal = -gravityBody.GravityDirection; // Default to gravity when airborne
            isGrounded = false;
        }
    }

    void AlignWithSurface()
    {
        Vector3 targetUp = isGrounded ? groundNormal : -gravityBody.GravityDirection;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * slopeBlendSpeed));
    }

    void RotatePlayer()
    {
        if (rotationInput != 0)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.fixedDeltaTime;
            transform.Rotate(Vector3.up, rotationAmount, Space.Self);
        }
    }

    void ApplyMovement()
    {
        if (moveInput.magnitude > 0)
        {
            float controlFactor = isGrounded ? 1f : airControlFactor;
            Vector3 moveDirection = transform.forward * moveInput.z;
            Vector3 targetVelocity = moveDirection * maxSpeed * controlFactor;
            
            Vector3 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration * controlFactor * Time.fixedDeltaTime);
            
            rb.linearVelocity += velocityChange;
        }
    }

    void ApplyFriction()
    {
        if (isGrounded && moveInput.magnitude == 0 && rb.linearVelocity.magnitude > 0.1f)
        {
            Vector3 frictionForce = -rb.linearVelocity.normalized * friction * Time.fixedDeltaTime;
            if (frictionForce.magnitude > rb.linearVelocity.magnitude)
                rb.linearVelocity = Vector3.zero;
            else
                rb.linearVelocity += frictionForce;
        }
    }
}
