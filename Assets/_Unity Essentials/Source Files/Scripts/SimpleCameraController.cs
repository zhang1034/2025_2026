using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    public float moveSpeed = 3.0f;           // Movement speed
    public float rotationSpeed = 100.0f;      // Rotation speed for A/D keys in degrees per second
    public float mouseSensitivity = 1.0f;    // Mouse look sensitivity

    private float rotationX = 0.0f;          // Rotation around the X axis (up and down look)

    void Update()
    {
        // Movement
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        
        // Translate only on the X-Z plane (global Y remains unchanged)
        Vector3 moveDirection = new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized;
        transform.Translate(moveDirection * moveForward, Space.World);

        // Rotation with A/D keys
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0, Space.World); // Rotate around the global Y axis

        // Mouse Look
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;  // Subtracting to invert the up and down look
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);  // Clamp the up and down look to avoid flipping

        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y, 0);
    }
}
