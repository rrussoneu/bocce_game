using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;       // Speed for WASD panning
    public float rotationSpeed = 75f;  // Degrees per second for Q/E rotation
    public float zoomSpeed = 10f;       // Speed for Z/X zooming

    void Update()
    {
        // WASD Movement
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W))
            vertical += 1f;
        if (Input.GetKey(KeyCode.S))
            vertical -= 1f;
        if (Input.GetKey(KeyCode.A))
            horizontal -= 1f;
        if (Input.GetKey(KeyCode.D))
            horizontal += 1f;

        Vector3 moveDir = (transform.forward * vertical) + (transform.right * horizontal);
        moveDir.y = 0f; // For vertical drifting
        transform.position += moveDir * moveSpeed * Time.deltaTime;

       
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f, Space.World);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        if (Input.GetKey(KeyCode.Z))
            transform.position += transform.forward * zoomSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.X))
            transform.position -= transform.forward * zoomSpeed * Time.deltaTime;
    }
}