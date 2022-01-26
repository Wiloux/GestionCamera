using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFollowView : AView
{

    [Range(-180, 180)] public float roll;
    private float yaw;
    private float pitch;

    [Range(90f,180f)] public float fov = 90f;
    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, fov); }

    public Transform target;

    public Transform centralPoint;

    [Range(0, 90)] public float yawOffsetMax = 10;
    [Range(0, 90)] public float pitchOffsetMax = 10;

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

    private void Awake()
    {
        pitch = CalculatePitch(centralPoint);
        yaw = CalculateYaw(centralPoint);
    }

    private void Update()
    {
        if (target && centralPoint)
        {
            float deltaPitch = CalculatePitch(target) - CalculatePitch(centralPoint);

            float deltaYaw = CalculateYaw(target) - CalculateYaw(centralPoint);
            if (deltaYaw > 180) { while (deltaYaw > 180) { deltaYaw -= 360; } }
            if (deltaYaw < -180) { while (deltaYaw < -180) { deltaYaw += 360; } }

            deltaYaw = Mathf.Clamp(deltaYaw, -yawOffsetMax, yawOffsetMax);
            deltaPitch = Mathf.Clamp(deltaPitch, -pitchOffsetMax, pitchOffsetMax);

            pitch = deltaPitch + CalculatePitch(centralPoint);
            yaw = deltaYaw + CalculateYaw(centralPoint);
        }
    }

    public override void OnDrawGizmos()
    {
        CameraConfiguration config = GetConfiguration();

        if (!Camera.main || config == null) { return; }

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(config.pivot, 0.175f);

        if (centralPoint)
        {
            Gizmos.DrawSphere(centralPoint.position, 0.175f);
            Gizmos.DrawLine(transform.position, transform.position + (centralPoint.position - transform.position));
        }

        Gizmos.color = Color.blue;
        Vector3 position = config.GetPosition();
        Gizmos.DrawLine(config.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, config.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, config.fov, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }

}
