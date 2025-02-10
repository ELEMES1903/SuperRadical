using UnityEngine;
using Cinemachine;

public class CinemachineGravityHandler : MonoBehaviour
{
    public GravityBody gravityBody; // Reference to your GravityBody script
    public CinemachineFreeLook freeLookCamera; // Reference to your Cinemachine FreeLook camera

    private Transform customWorldUp; // Temporary transform to manage "up"

    void Start()
    {
        // Create a temporary GameObject to act as the custom world up
        customWorldUp = new GameObject("CameraWorldUp").transform;
    }

    void LateUpdate()
    {
        if (gravityBody == null || freeLookCamera == null || customWorldUp == null)
            return;

        // Update the custom "world up" direction
        customWorldUp.position = gravityBody.transform.position; // Match player position
        customWorldUp.up = -gravityBody.GravityDirection.normalized; // Match gravity-based "up"

        // Assign the custom up direction to the FreeLook camera
        freeLookCamera.m_RecenterToTargetHeading.m_enabled = false; // Ensure manual control
        freeLookCamera.m_Follow = gravityBody.transform; // Follow the player
        freeLookCamera.m_LookAt = gravityBody.transform; // Look at the player
    }
}
 