using UnityEngine;
using Dreamteck.Splines;
using System.Collections;

public class RailGrind : MonoBehaviour
{
    [SerializeField] private float grindSpeed = 10f;
    [SerializeField] private LayerMask railLayer;
    [SerializeField] private float grindCooldownDuration = 0.1f;
    [SerializeField] private float jumpForce = 8f;

    private SplineFollower splineFollower;
    private Rigidbody rb;
    public bool isGrinding = false;
    private bool canGrind = true;
    
    void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        rb = GetComponent<Rigidbody>();
        splineFollower.follow = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrinding) // Jump off rail
        {
            StopGrinding();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canGrind) return;

        if (((1 << collision.gameObject.layer) & railLayer) != 0) // Check if it's a rail
        {
            SplineComputer spline = collision.gameObject.GetComponent<SplineComputer>();
            if (spline != null)
            {
                StartGrinding(spline);
            }
        }
    }

    void StartGrinding(SplineComputer spline)
    {
        isGrinding = true;
        canGrind = false;
        splineFollower.spline = spline;

        // Snap to closest point on the spline
        SplineSample sample = new SplineSample();
        spline.Project(transform.position, ref sample);
        splineFollower.SetPercent(sample.percent);

        // Determine grind direction based on player facing
        SetGrindDirection(sample);

        // Enable spline follower
        splineFollower.follow = true;
    }

    void SetGrindDirection(SplineSample sample)
    {
        Vector3 railDirection = sample.forward.normalized;
        Vector3 playerDirection = transform.forward.normalized;

        float dot = Vector3.Dot(playerDirection, railDirection);
        splineFollower.followSpeed = (dot >= 0) ? grindSpeed : -grindSpeed;
    }

    void StopGrinding()
    {
        isGrinding = false;
        splineFollower.follow = false;

        // Apply jump force when exiting rail
        rb.linearVelocity = transform.forward * grindSpeed * 0.5f + Vector3.up * jumpForce;

        StartCoroutine(GrindCooldown());
    }

    IEnumerator GrindCooldown()
    {
        yield return new WaitForSeconds(grindCooldownDuration);
        canGrind = true;
    }
}
