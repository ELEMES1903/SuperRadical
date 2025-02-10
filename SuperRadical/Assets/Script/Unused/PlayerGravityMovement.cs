using UnityEngine;

[RequireComponent(typeof(GravityBody))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerGravityMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float maxVelocity = 5f;
    public Transform characterModel;
    public float rotationSpeed = 180f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private float groundCheckRadius = 0.6f;
    public float jumpForce = 10f;

    private Rigidbody _rigidbody;
    private GravityBody _gravityBody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _gravityBody = GetComponent<GravityBody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        HandleJump();
        HandleRotation();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ClampVelocity();
    }

    private void HandleJump()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _rigidbody.AddForce(-_gravityBody.GravityDirection * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMovement()
    {
        // Get movement input for forward/backward (W and S)
        float forwardInput = Input.GetAxis("Vertical"); // W/S or up/down arrow
        Vector3 gravityDirection = _gravityBody.GravityDirection;

        // Determine movement direction (using the player's transform forward direction)
        Vector3 moveDirection = Vector3.ProjectOnPlane(transform.forward, gravityDirection).normalized;

        // Apply movement force
        _rigidbody.AddForce(moveDirection * forwardInput * moveSpeed, ForceMode.Force);
    }

    private void ClampVelocity()
    {
        Vector3 gravityDirection = _gravityBody.GravityDirection;
        Vector3 velocity = _rigidbody.linearVelocity;
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(velocity, gravityDirection);

        if (lateralVelocity.magnitude > maxVelocity)
        {
            Vector3 clampedVelocity = lateralVelocity.normalized * maxVelocity;
            _rigidbody.linearVelocity = clampedVelocity + Vector3.Project(velocity, gravityDirection);
        }
    }

    private void HandleRotation()
    {
        float rotationInput = Input.GetAxis("Horizontal"); // A/D or left/right arrow
        transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime, Space.Self);
    }

    void OnDrawGizmos()
    {
        // Draw a sphere to visualize the ground check radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
