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

    private Transform centralPoint;

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

    private void Start()
    {
        centralPoint = new GameObject("Follow").transform;
        centralPoint.parent = transform;

        //centralPoint.position = transform.position + (target.position - transform.position).normalized;
        centralPoint.position = transform.position + transform.forward;
        transform.LookAt(target);

        pitch = CalculatePitch(centralPoint);
        yaw = CalculateYaw(centralPoint);
    }

    private void Update()
    {
        float deltaPitch = CalculatePitch(centralPoint) - CalculatePitch(target);

        float deltaYaw = CalculateYaw(target) - CalculateYaw(centralPoint);
        if (deltaYaw > 180) { while (deltaYaw > 180) { deltaYaw -= 360; } }
        if (deltaYaw < -180) { while (deltaYaw < -180) { deltaYaw += 360; } }

        if (Mathf.Abs(deltaPitch) > pitchOffsetMax || Mathf.Abs(deltaYaw) > yawOffsetMax)
        {
            float anglePitch = Mathf.Abs(deltaPitch) > pitchOffsetMax ? Mathf.Sign(deltaPitch) * pitchOffsetMax : 0;
            float angleYaw = Mathf.Abs(deltaYaw) > yawOffsetMax ? Mathf.Sign(-deltaYaw) * yawOffsetMax : 0;

            transform.LookAt(transform.position + Quaternion.Euler(anglePitch, angleYaw, 0) * (target.position - transform.position));
        }

        pitch = CalculatePitch(centralPoint);
        yaw = CalculateYaw(centralPoint);

        if (Input.GetKey(KeyCode.W)) { target.position += Vector3.up * Time.deltaTime * 3; }
        if (Input.GetKey(KeyCode.S)) { target.position += -Vector3.up * Time.deltaTime * 3; }
        if (Input.GetKey(KeyCode.D)) { target.position += Vector3.right * Time.deltaTime * 3; }
        if (Input.GetKey(KeyCode.A)) { target.position += -Vector3.right * Time.deltaTime * 3; }
    }

    public override void OnDrawGizmos()
    {
        CameraConfiguration config = GetConfiguration();

        if (!Camera.main || config == null) { return; }

        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(config.pivot, 0.175f);

        if (centralPoint)
        {
            Gizmos.DrawSphere(centralPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position - (transform.position - centralPoint.position).normalized * 100);
        }

        Vector3 position = config.GetPosition();
        Gizmos.DrawLine(config.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, config.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, config.fov, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }

}
