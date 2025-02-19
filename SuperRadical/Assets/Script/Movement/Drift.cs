using UnityEngine;

public class Drift : MonoBehaviour
{
    [Header("Drift Settings")]
    [Tooltip("Speed at which the kart moves while drifting.")]
    public float driftSpeed = 10f;
    
    [Tooltip("Base drift rotation speed in degrees per second (how tight the drift is).")]
    public float driftStrength = 20f;
    
    [Tooltip("Key to initiate drifting.")]
    public KeyCode driftButton = KeyCode.LeftShift;
    
    [Tooltip("How quickly the kart rotates towards the continuously updated drift direction.")]
    public float rotationLerpSpeed = 5f;

    [Header("Drift Boost Settings")]
    [Tooltip("Current boost charge (accumulates while drifting).")]
    public float boostCharge = 0f;
    
    [Tooltip("Maximum boost charge the player can accumulate.")]
    public float maxBoostCharge = 20f;
    
    [Tooltip("Rate (per second) at which boost charge accumulates while drifting.")]
    public float boostChargeRate = 5f;

    [Header("Adjustable Drift Strength")]
    [Tooltip("How much the drift strength can be adjusted up or down.")]
    public float adjustDriftRange = 10f;

    [Tooltip("How fast the adjustment value changes per second.")]
    public float adjustDriftRate = 5f;

    [Tooltip("Current adjustment applied to the drift strength.")]
    public float adjustDriftValue = 0f;

    // Internal state variables
    public bool isDrifting = false;
    private int driftSide = 0;  // -1 for left, +1 for right.
    private Quaternion startDriftRotation;
    private Quaternion targetDriftRotation;
    private float currentDriftAngle = 0f;
    private Rigidbody rb;
    private PlayerMovement pm;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Initiate drifting if the drift button and a direction key are pressed
        if (Input.GetKey(driftButton) && pm.isGrounded)
        {
            if (!isDrifting)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    StartDrift(-1);  // Drift to the left
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    StartDrift(1);   // Drift to the right
                }
            }
        }
        else
        {
            if (isDrifting)
            {
                StopDrift();
            }
        }

        // Adjust drift strength mid-drift
        if (isDrifting)
        {
            AdjustDriftStrength();
        }
    }

    void FixedUpdate()
    {
        if (isDrifting)
        {
            float effectiveDriftStrength = driftStrength + adjustDriftValue;
            currentDriftAngle += effectiveDriftStrength * driftSide * Time.fixedDeltaTime;
            targetDriftRotation = Quaternion.Euler(0f, currentDriftAngle, 0f) * startDriftRotation;
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetDriftRotation, Time.fixedDeltaTime * rotationLerpSpeed);
            rb.MoveRotation(newRotation);
            
            rb.linearVelocity = newRotation * Vector3.forward * driftSpeed;
            
            boostCharge = Mathf.Clamp(boostCharge + boostChargeRate * Time.fixedDeltaTime, 0f, maxBoostCharge);
        }
    }

    void StartDrift(int side)
    {
        isDrifting = true;
        driftSide = side;
        startDriftRotation = transform.rotation;
        currentDriftAngle = 0f;
        boostCharge = 0f; 
        adjustDriftValue = 0f;
        pm.canMove = false;
        
    }

    void StopDrift()
    {
        isDrifting = false;
        driftSide = 0;
        rb.AddForce(transform.forward * boostCharge, ForceMode.Impulse);
        boostCharge = 0f;
        pm.canMove = true;
    }

    void AdjustDriftStrength()
    {
        
        float adjustAmount = adjustDriftRate * Time.deltaTime;

        if (driftSide == -1) // Drifting left
        {
            if (Input.GetKey(KeyCode.A))
            {
                adjustDriftValue = Mathf.Clamp(adjustDriftValue + adjustAmount, -adjustDriftRange, adjustDriftRange);
            }
            if (Input.GetKey(KeyCode.D))
            {
                adjustDriftValue = Mathf.Clamp(adjustDriftValue - adjustAmount, -adjustDriftRange, adjustDriftRange);
            }
        }
        else if (driftSide == 1) // Drifting right
        {
            if (Input.GetKey(KeyCode.A))
            {
                adjustDriftValue = Mathf.Clamp(adjustDriftValue - adjustAmount, -adjustDriftRange, adjustDriftRange);
            }
            if (Input.GetKey(KeyCode.D))
            {
                adjustDriftValue = Mathf.Clamp(adjustDriftValue + adjustAmount, -adjustDriftRange, adjustDriftRange);
            }
        }
    }
}
