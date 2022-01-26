using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedView : AView
{
    [Range(-180, 180)] public float yaw;
    [Range(-180, 180)] public float pitch;
    [Range(-180, 180)] public float roll;
    [Range(90, 180)] public float fov = 90;

    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, fov); }

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
