using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFollowView : AView
{

    public float roll;
    private float yaw;
    private float pitch;

    [Range(90f,180f)]
    public float fov = 90f;
    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, fov); }

    public Transform target;

    public GameObject centralPoint;

    public float yawOffsetMax = 50;
    public float pitchOffsetMax = 50;

    public float CalculateYaw(Transform target)
    {
        if (!target)
            return 0;

        Vector3 dir = (target.position - transform.position).normalized;
        return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    public float CalculatePitch(Transform target)
    {
        if (!target)
            return 0; 

        Vector3 dir = (target.position - transform.position).normalized;
        return -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
    }

    private void Update()
    {
      pitch = CalculatePitch(target);
      yaw = CalculateYaw(target);
        if (CameraController.Instance)
        {
            CameraController.Instance.UpdateActiveViewsAverageConfiguration();
        }
       
    }
    public override void OnDrawGizmos()
    {
        CameraConfiguration config = GetConfiguration();

        if (!Camera.main || config == null) { return; }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(config.pivot, 0.175f);
        Vector3 position = config.GetPosition();
        Gizmos.DrawLine(config.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, config.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, config.fov, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }

}
