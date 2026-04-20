using System;
using _Bifrost.Runtime.Portals;
using UnityEngine;

public class ViewPortal : MonoBehaviour
{
    public Transform portalA;
    public Transform portalB;

    public Camera playerCam;
    public Camera portalCam;
    void LateUpdate()
    {
        UpdatePortalCamera();
    }

    void UpdatePortalCamera()
    {
        Vector3 relativePos = portalA.InverseTransformPoint(playerCam.transform.position);

        relativePos.x = 0f;
        
        portalCam.transform.position = portalB.TransformPoint(relativePos);
        portalCam.fieldOfView = playerCam.fieldOfView;
        
        Vector3 toPortal = portalB.position - portalCam.transform.position;
        
        float dist =  Vector3.Dot(portalB.forward, toPortal);
        float targetNear = Mathf.Clamp(dist, 0.01f, 10f);

        portalCam.nearClipPlane = Mathf.Lerp(portalCam.nearClipPlane, targetNear, Time.deltaTime * 10f);
    }
}
