using UnityEngine;

public class PortalView : MonoBehaviour
{
    private Transform playerCamera;
    [SerializeField] private Transform portal;          // дверь (вход)
    [SerializeField] private Transform portalTarget;    // точка в другом мире
    [SerializeField] private Transform portalCamera;
    [SerializeField] private float parallaxStrength = 1.0f;

    void Start()
    {
        playerCamera = Camera.main.transform;
    }
    void LateUpdate()
    {
        Vector3 offset = playerCamera.position - portal.position;

        offset.z = 0;

        Vector3 lookPoint = portalTarget.position + offset * parallaxStrength;

        portalCamera.position = portalTarget.position;

        portalCamera.rotation = Quaternion.LookRotation(
            (lookPoint - portalCamera.position),
            Vector3.up
        );
    }
}