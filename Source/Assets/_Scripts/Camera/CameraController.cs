using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float cameraSmooth = 100f;
    private float cameraDistance = -8f;
    private float adjustedDistance = -8f;

    private float zoomSmooth = 10f;
    private float maxZoom = -2f;
    private float minZoom = -15f;

    private const float initialOrbitXRotation = -20f;
    private const float initialOrbitYRotation = -180f;
    private float orbitXRotation = -20f;
    private float orbitYRotation = -180f;
    private float maxXRotation = 25f;
    private float minXRotation = -85f;
    private float orbitSmooth = 50;

    private Transform target;

    CameraCollider cameraCollider = new CameraCollider();

    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;
    Vector3 camVelocity = Vector3.zero;

    Vector3 currentMousePosition = Vector3.zero;
    Vector3 previousMousePosition = Vector3.zero;


    void Update()
    {
        if(target != null)
        {
            Zoom();
        }
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            GameObject playerObject;
            if ((playerObject = GameObject.FindGameObjectWithTag("Player")) != null)
            {
                target = playerObject.transform.Find("CameraTarget");
                MoveToTarget();

                cameraCollider.Initialize(Camera.main);
                cameraCollider.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollider.AdjustedCameraClipPoints);
                cameraCollider.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollider.DesiredCameraClipPoints);
            }
            return;
        }

        MoveToTarget();
        LookAtTarget();
        Orbit();

        cameraCollider.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollider.AdjustedCameraClipPoints);
        cameraCollider.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollider.DesiredCameraClipPoints);
        cameraCollider.CheckColliding(target.position);
        adjustedDistance = cameraCollider.GetAdjustedDistanceWithRayFrom(target.position);
    }

    void MoveToTarget()
    {
        this.destination = Quaternion.Euler(orbitXRotation, orbitYRotation + target.eulerAngles.y, 0) * -Vector3.forward * cameraDistance;
        this.destination += target.position;

        Vector3 correctedDestination = destination;
        if (cameraCollider.Colliding)
        {
            adjustedDestination = Quaternion.Euler(orbitXRotation, orbitYRotation + target.eulerAngles.y, 0) * Vector3.forward * adjustedDistance;
            adjustedDestination += target.position;
            correctedDestination = adjustedDestination;
        }

        transform.position = Vector3.SmoothDamp(transform.position, correctedDestination, ref camVelocity, 0.05f);
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraSmooth * Time.deltaTime);
    }

    void Orbit()
    {
        //Setting our current and previous mouse positions
        previousMousePosition = currentMousePosition;
        currentMousePosition = Input.mousePosition;

        if(Input.GetAxisRaw("MouseOrbit") > 0)
        {
            orbitYRotation += (currentMousePosition.x - previousMousePosition.x) * orbitSmooth * Time.deltaTime;
            orbitYRotation = orbitYRotation % 360;
            orbitXRotation += (currentMousePosition.y - previousMousePosition.y) * orbitSmooth * Time.deltaTime;
            orbitXRotation = Mathf.Clamp(orbitXRotation, minXRotation, maxXRotation);      
        }
        else
        {         
            orbitYRotation = Mathf.Lerp(orbitYRotation, initialOrbitYRotation, 10f * Time.deltaTime);  
        }
    }

    void Zoom()
    {
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * zoomSmooth;
        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);
    }
}
