using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider
{
    private const int kNumClipPoints = 5;
    
    public bool Colliding { get; private set; }
    public Vector3[] AdjustedCameraClipPoints;
    public Vector3[] DesiredCameraClipPoints;

    private Camera camera;

    public void Initialize(Camera camera)
    {
        this.camera = camera;
        AdjustedCameraClipPoints = new Vector3[kNumClipPoints];
        DesiredCameraClipPoints = new Vector3[kNumClipPoints];
    }

    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion cameraRotation, ref Vector3[] clipPoints)
    {
        if (!camera)
            return;

        clipPoints = new Vector3[5];

        //TODO: Explain this
        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / 3f) * z;
        float y = x / camera.aspect;

        clipPoints[0] = (cameraRotation * new Vector3(-x, y, z)) + cameraPosition;
        clipPoints[1] = (cameraRotation * new Vector3(x, y, z)) + cameraPosition;
        clipPoints[2] = (cameraRotation * new Vector3(-x, -y, z)) + cameraPosition;
        clipPoints[3] = (cameraRotation * new Vector3(x, -y, z)) + cameraPosition;
        clipPoints[4] = cameraPosition - camera.transform.forward;
    }

    private bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    {
        for(int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
            float distance = Vector3.Distance(clipPoints[i], fromPosition);
            if(Physics.Raycast(ray, distance))
            {
                return true;
            }
        }

        return false;
    }

    public float GetAdjustedDistanceWithRayFrom(Vector3 from)
    {
        float distance = -1;

        for(int i = 0; i < DesiredCameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, DesiredCameraClipPoints[i] - from);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                {
                    distance = hit.distance;
                }
                else if(hit.distance < distance)
                {
                    distance = hit.distance;
                }                
            }
        }

        if (distance == -1)
            return 0;
        else
            return distance;
    }

    public void CheckColliding(Vector3 targetPosition)
    {
        if (CollisionDetectedAtClipPoints(DesiredCameraClipPoints, targetPosition))
        {
            Colliding = true;
        }
        else
        {
            Colliding = false;
        }
    }
}
