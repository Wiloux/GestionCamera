using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFollowView : AView
{
    public FreeFollowConfig topConfiguration = new FreeFollowConfig();
    public FreeFollowConfig middleConfiguration = new FreeFollowConfig();
    public FreeFollowConfig bottomConfiguration = new FreeFollowConfig();

    public Curve curve;
    public float curveSpeed = 1;
    private float curvePosition = 0.5f;

    [System.Serializable]
    public class FreeFollowConfig
    {
        public float roll = 0;
        public float pitch = 0;
        [Range(90f, 180f)] public float fov = 90f;
    }

    public float yawSpeed = 3;

    private float roll = 0;
    private float pitch = 0;
    private float yaw = 0;
    private float fov = 90;

    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, fov); }

    public Transform target;

    private void Update()
    {
        if (curve && target)
        {
            yaw += -Input.GetAxis("Mouse X") * yawSpeed;
            if (yaw > 180) { while (yaw > 180) { yaw -= 360; } }
            if (yaw < -180) { while (yaw < -180) { yaw += 360; } }

            curvePosition += -Input.GetAxis("Mouse Y") * curveSpeed;
            curvePosition = Mathf.Clamp01(curvePosition);
            transform.position = curve.GetPosition(curvePosition, Matrix4x4.TRS(target.position, Quaternion.Euler(0, 180 + yaw, 0), Vector3.one));

            FreeFollowConfig avgConfig = null;
            if (curvePosition < 0.5f) { avgConfig = LerpConfiguration(middleConfiguration, bottomConfiguration, curvePosition * 2); }
            else { avgConfig = LerpConfiguration(topConfiguration, middleConfiguration, curvePosition * 2 - 1); }

            if (avgConfig != null)
            {
                pitch = avgConfig.pitch;
                roll = avgConfig.roll;
                fov = avgConfig.fov;
            }
        }
    }

    public FreeFollowConfig LerpConfiguration(FreeFollowConfig a, FreeFollowConfig b, float t)
    {
        FreeFollowConfig avgConfig = new FreeFollowConfig();

        avgConfig.pitch = t * a.pitch + (1 - t) * b.pitch;
        avgConfig.roll = t * a.roll + (1 - t) * b.roll;
        avgConfig.fov = t * a.fov + (1 - t) * b.fov;

        return avgConfig;
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
