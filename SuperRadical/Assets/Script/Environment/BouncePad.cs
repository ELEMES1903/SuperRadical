using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float bounceForce = 20f; // Adjustable bounce strength

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            // Get the closest point on the collider and calculate normal
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            if (Physics.Raycast(contactPoint + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit))
            {
                Vector3 bounceDirection = hit.normal.normalized; // Use the surface normal
                rb.linearVelocity = Vector3.zero; // Reset vertical velocity
                rb.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
