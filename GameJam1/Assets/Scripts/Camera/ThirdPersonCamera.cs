using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2f;

    [Header("Rotation")]
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float minDistance = 0.5f;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        if (target == null)
            return;

        yaw = target.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // =========================
        // Rotate Camera
        // =========================

        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;

            pitch -= Input.GetAxis("Mouse Y") * sensitivity;

            pitch = Mathf.Clamp(
                pitch,
                minPitch,
                maxPitch
            );
        }

        Quaternion rotation =
            Quaternion.Euler(
                pitch,
                yaw,
                0f
            );

        Vector3 focusPoint =
            target.position +
            Vector3.up * height;

        Vector3 direction =
            rotation * Vector3.back;

        float currentDistance =
            distance;

        // =========================
        // Wall Collision
        // =========================

        RaycastHit hit;

        if (Physics.SphereCast(
            focusPoint,
            collisionRadius,
            direction,
            out hit,
            distance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            currentDistance =
                Mathf.Max(
                    hit.distance - 0.1f,
                    minDistance
                );
        }

        Vector3 desiredPosition =
            focusPoint +
            direction * currentDistance;

        transform.position =
            desiredPosition;

        transform.rotation =
            rotation;
    }
}