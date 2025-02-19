using UnityEngine;

public class Jump : MonoBehaviour
{
    public float jumpForce;
    public float firstJumpForce;
    public float secondJumpForce;
    public float thirdJumpForce;
    private GravityBody gb;
    private PlayerMovement pm;
    private Rigidbody rb;

    //0.5
    public float jumpComboMaxTimer;
    private float jumpComboTimer;
    public float  consecutiveJumps;

    public KeyCode jumpKey = KeyCode.Space;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        gb = GetComponent<GravityBody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if(consecutiveJumps > 0 && pm.isGrounded && jumpComboTimer > 0)
        {
            jumpComboTimer -=Time.deltaTime;
        }
        else if(jumpComboTimer <= 0)
        {
            consecutiveJumps = 0;
            jumpComboTimer = jumpComboMaxTimer;
        }

        // Jump logic
        if (Input.GetKeyDown(jumpKey) && pm.isGrounded)
        {
            
            consecutiveJumps++;
            jumpComboTimer = jumpComboMaxTimer;
            
            if(consecutiveJumps == 1)
            {
                jumpForce = firstJumpForce;
            }
            else if (consecutiveJumps == 2)
            {
                jumpForce = secondJumpForce;
            }
            else
            {
                jumpForce = thirdJumpForce;
                consecutiveJumps = 0;
            }

            rb.AddForce(-gb.GravityDirection * jumpForce, ForceMode.Impulse);
        }
    }
}
