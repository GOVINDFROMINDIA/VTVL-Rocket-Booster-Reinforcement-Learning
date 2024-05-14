using UnityEngine;

public class RocketLanding : MonoBehaviour
{
    public float descentSpeed = 10.0f;
    public float initialHeight; // Removed the default value to set it randomly
    public float maxThrust = 30.0f;
    public float stabilizationForce = 50.0f;
    public bool hasLanded = false;
    Rigidbody rb;

    void Start()
    {
        // Set a random initial height between, for example, 20 and 50 meters
        initialHeight = Random.Range(20.0f, 50.0f);

        // Set the initial position of the rocket to the random height
        transform.position = new Vector3(0, initialHeight, 0);
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, -descentSpeed, 0);
        rb.angularDrag = 10;  // Increase angular drag to help slow down rotation
    }

    void FixedUpdate()
    {
        if (!hasLanded)
        {
            ApplyThrustToControlDescent();
            StabilizeRocketOrientation();
        }
    }

    void ApplyThrustToControlDescent()
    {
        float requiredThrust = 0;
        float velocityDifference = descentSpeed + rb.velocity.y;

        if (velocityDifference < 0)
        {
            requiredThrust = Mathf.Clamp(-velocityDifference * rb.mass, 0, maxThrust);
            rb.AddForce(transform.up * requiredThrust);
        }

        if (transform.position.y <= 1.0f && Mathf.Abs(rb.velocity.y) < 1f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            hasLanded = true;
        }
    }

    void StabilizeRocketOrientation()
    {
        if (!hasLanded)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            Quaternion currentRotation = transform.rotation;
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

            rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180)
            {
                angle -= 360;
            }

            rb.AddTorque(axis * angle * stabilizationForce * Time.fixedDeltaTime);
        }
    }
}
