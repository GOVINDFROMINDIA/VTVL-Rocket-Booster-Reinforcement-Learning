using UnityEngine;

public class RocketLanding : MonoBehaviour
{
    public float descentSpeed = 10.0f; // Desired descent speed in meters per second
    public float initialHeight = 25.0f; // Initial height in meters
    public float maxThrust = 30.0f; // Maximum thrust force
    public float stabilizationForce = 50.0f; // Force to stabilize the rocket orientation
    Rigidbody rb;

    void Start()
    {
        // Set the initial position of the cylinder at 25 meters above the ground
        transform.position = new Vector3(0, initialHeight, 0);

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Initial velocity downwards
        rb.velocity = new Vector3(0, -descentSpeed, 0);
    }

    void FixedUpdate()
    {
        ApplyThrustToControlDescent();
        StabilizeRocketOrientation();
    }

    void ApplyThrustToControlDescent()
    {
        // Calculate the necessary thrust to slow down as the rocket approaches the ground
        float requiredThrust = 0;
        float velocityDifference = descentSpeed + rb.velocity.y;

        // Apply thrust only if the rocket is moving faster than the desired speed
        if (velocityDifference < 0)
        {
            requiredThrust = Mathf.Clamp(-velocityDifference * rb.mass, 0, maxThrust);
            rb.AddForce(transform.up * requiredThrust);
        }

        // Soft landing check
        if (transform.position.y <= 1.0f && Mathf.Abs(rb.velocity.y) < 1f)
        {
            // Neutralize any remaining downward velocity for a soft landing
            rb.velocity = Vector3.zero;
            rb.isKinematic = true; // Optionally make the Rigidbody kinematic after landing
        }
    }

    void StabilizeRocketOrientation()
    {
        // Calculate the angle difference from upright position
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        Quaternion currentRotation = transform.rotation;
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

        // Convert rotation difference to angle and axis
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180)
        {
            angle -= 360;
        }

        // Apply torque to correct the rotation
        rb.AddTorque(axis * angle * stabilizationForce);
    }
}
