using UnityEngine;

public class WASDMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // movement speed in units per second
    public float moveX;
    public float moveZ; 

    public void Move(Vector2 move)
    {
        this.moveX = move.x;
        this.moveZ = move.y;
    }

    void Update()
    {
        // Get input axes (WASD mapped to Horizontal/Vertical by default)

        // Create movement vector
        Vector3 move = new Vector3(this.moveX, 0, this.moveZ);

        // Move relative to world space
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}