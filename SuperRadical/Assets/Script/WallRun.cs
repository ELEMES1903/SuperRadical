using Unity.VisualScripting;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wallrunning")]

    public LayerMask wallLayer;
    public LayerMask groundLayer;

    public float wallRunSpeed;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput; 

    [Header("Detection")]
    public float wallCheckDistance;
    public float wallCheckAngle = 30f; 
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Reference")]

    private MarioGalaxyMovement pm;
    private Rigidbody rb;
    public bool isWallRunning;
    private GravityBody gravityBody;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MarioGalaxyMovement>();
        gravityBody = GetComponent<GravityBody>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
        AboveGround();
    }

    private void FixedUpdate()
    {
        if(isWallRunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        Vector3 origin = transform.position;        

        // Generate directions for the "pyramid" shape
        Vector3 rightUp = Quaternion.AngleAxis(wallCheckAngle, transform.forward) * transform.right;
        Vector3 rightDown = Quaternion.AngleAxis(-wallCheckAngle, transform.forward) * transform.right;
        Vector3 rightLeft = Quaternion.AngleAxis(-wallCheckAngle, transform.up) * transform.right;
        Vector3 rightRight = Quaternion.AngleAxis(wallCheckAngle, transform.up) * transform.right;

        Vector3 leftUp = Quaternion.AngleAxis(wallCheckAngle, transform.forward) * -transform.right;
        Vector3 leftDown = Quaternion.AngleAxis(-wallCheckAngle, transform.forward) * -transform.right;
        Vector3 leftLeft = Quaternion.AngleAxis(-wallCheckAngle, transform.up) * -transform.right;
        Vector3 leftRight = Quaternion.AngleAxis(wallCheckAngle, transform.up) * -transform.right;

        // Perform raycasts (any hit means we're near a wall)
        wallRight = Physics.Raycast(origin, rightUp, out rightWallHit, wallCheckDistance, wallLayer) ||
                    Physics.Raycast(origin, rightDown, out rightWallHit, wallCheckDistance, wallLayer) ||
                    Physics.Raycast(origin, rightLeft, out rightWallHit, wallCheckDistance, wallLayer) ||
                    Physics.Raycast(origin, rightRight, out rightWallHit, wallCheckDistance, wallLayer);

        wallLeft = Physics.Raycast(origin, leftUp, out leftWallHit, wallCheckDistance, wallLayer) ||
                Physics.Raycast(origin, leftDown, out leftWallHit, wallCheckDistance, wallLayer) ||
                Physics.Raycast(origin, leftLeft, out leftWallHit, wallCheckDistance, wallLayer) ||
                Physics.Raycast(origin, leftRight, out leftWallHit, wallCheckDistance, wallLayer);

        // Debug rays to visualize in Scene view
        Debug.DrawRay(origin, rightUp * wallCheckDistance, Color.blue);
        Debug.DrawRay(origin, rightDown * wallCheckDistance, Color.red);
        Debug.DrawRay(origin, rightLeft * wallCheckDistance, Color.yellow);
        Debug.DrawRay(origin, rightRight * wallCheckDistance, Color.black);
        
        Debug.DrawRay(origin, leftUp * wallCheckDistance, Color.blue);
        Debug.DrawRay(origin, leftDown * wallCheckDistance, Color.red);
        Debug.DrawRay(origin, leftLeft * wallCheckDistance, Color.yellow);
        Debug.DrawRay(origin, leftRight * wallCheckDistance, Color.black);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundLayer);
    }
    public void StateMachine()
    {

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        bool isAboveGround = !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);

        //State 1 - Wallrunning
        if((wallLeft || wallRight) && !pm.isGrounded && !exitingWall && AboveGround())
        {
            if (!isWallRunning)
            {
                StartWallRun();
                Debug.Log("wall running");
            }

            //wallrun timer
            if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if(wallRunTimer <= 0 && isWallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
            
            //walljump
            if(Input.GetKeyDown(jumpKey))
            {
                WallJump();
                Debug.Log("wall jumping");
            }
        }
        //State 2 - Exiting
        else if (exitingWall)
        {
            if(isWallRunning)
            {
                StopWallRun();
            }
            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if(exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        //State 3 - None
        else
        {
            if(isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        pm.canMove = false;
        wallRunTimer = maxWallRunTime;
    }

    private void WallRunningMovement()
    {
        Vector3 gravityUp = -gravityBody.GravityDirection.normalized; // Custom gravity's up direction
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);  
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, gravityUp).normalized; // Use gravity up

        if(Vector3.Dot(transform.forward, wallForward) < Vector3.Dot(transform.forward, -wallForward))
        {
            wallForward = -wallForward;
        }

        // Align player orientation with custom gravity
        Quaternion targetRotation = Quaternion.LookRotation(wallForward, gravityUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        rb.linearVelocity = wallForward * wallRunSpeed;

        // Push player towards wall
        rb.AddForce(-wallNormal * 100, ForceMode.Force);
    }


    private void StopWallRun()
    {
        isWallRunning = false;
        pm.canMove = true; 
    }

    private void WallJump()
    {
        // enter exiting wall state
        exitingWall = true; 
        exitWallTimer = exitWallTime;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 upForce = transform.up * wallJumpUpForce;
        Vector3 sideForce = wallNormal * wallJumpSideForce;
        //reset y velocity and add foce
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);  
        rb.AddForce(upForce, ForceMode.Impulse);
        rb.AddForce(sideForce, ForceMode.Impulse);
    }
}
