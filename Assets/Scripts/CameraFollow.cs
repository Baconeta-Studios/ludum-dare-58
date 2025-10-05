using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 10f;
    public float rotationSpeed = 70f;
    public bool allowOrbit = true;

    private float yaw = 0f;
    private float pitch = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // Handle orbit input (optional)
        if (allowOrbit)
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -20f, 60f);
        }

        // Calculate rotation and position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}