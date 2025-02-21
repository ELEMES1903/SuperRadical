using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public static float GRAVITY_FORCE = 25;
    
    private Vector3 overrideGravityDirection = Vector3.zero;
    private bool overrideGravity = false;
    public bool useOverrideGravity = true; // Toggle override gravity usage

    public void SetGravityDirection(Vector3 newDirection, bool overrideGravity)
    {
        this.overrideGravity = overrideGravity;
        overrideGravityDirection = newDirection.normalized;
        _rigidbody.WakeUp();
    }

    public Vector3 GravityDirection
    {
        get
        {
            if (useOverrideGravity && overrideGravity) return overrideGravityDirection;
            if (_gravityAreas.Count == 0) return Vector3.zero;
            _gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            return _gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }

    private Rigidbody _rigidbody;
    private List<GravityArea> _gravityAreas;

    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _gravityAreas = new List<GravityArea>();
    }
    
        void FixedUpdate()
    {
        // Apply gravity force
        _rigidbody.AddForce(GravityDirection * (GRAVITY_FORCE * Time.fixedDeltaTime), ForceMode.VelocityChange);


        // Calculate new rotation to align with gravity direction
        Vector3 gravityUp = -GravityDirection; // Up direction relative to gravity
        Vector3 playerForward = Vector3.ProjectOnPlane(transform.forward, gravityUp).normalized;

        if (playerForward.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerForward, gravityUp);
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, Time.fixedDeltaTime * 3f));
        }
    }

    public void AddGravityArea(GravityArea gravityArea)
    {
        _gravityAreas.Add(gravityArea);
    }

    public void RemoveGravityArea(GravityArea gravityArea)
    {
        _gravityAreas.Remove(gravityArea);
    }
}