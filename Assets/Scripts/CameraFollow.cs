using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset  = new(0.0181f, 11.61f, -4.9f);
    public float followSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        var desiredPos = target.position + worldOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        //transform.LookAt(target.position);
    }
}