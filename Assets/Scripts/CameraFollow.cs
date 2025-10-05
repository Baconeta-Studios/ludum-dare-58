using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new(0.0181f, 11.61f, -4.9f);
    public Quaternion rotation = Quaternion.Euler(60f, 0f, 0f);
    public float followSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired position relative to the target
        Vector3 desiredPos = target.position + target.rotation * offset;

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Smoothly rotate camera
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, followSpeed * Time.deltaTime);
    }
}