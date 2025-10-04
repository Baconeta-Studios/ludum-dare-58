using UnityEngine;

public class WASDMovement1 : MonoBehaviour
{
    public float moveSpeed = 5f; // movement speed in units per second

    void Update()
    {
        // Get input axes (WASD mapped to Horizontal/Vertical by default)
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Create movement vector
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // Move relative to world space
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}