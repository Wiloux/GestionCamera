using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyView : AView
{
    public bool isAuto = false;

    private float pitch;
    private float yaw;
    [Range(-180, 180)] public float roll;
    public float distance;
    [Range(90, 180)] public float fov = 90;

    public float speed = 3;

    public Transform target;

    public CameraRail rail;

    private float distanceOnRail = 0;


    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, distance, fov); }

    private void Awake()
    {
        transform.position = rail.GetPosition(distanceOnRail);
    }

    private void Update()
    {
        if (target && rail)
        {
            if (!isAuto)
            {
                if (Input.GetAxis("Horizontal") != 0)
                {
                    distanceOnRail += speed * Time.deltaTime * Input.GetAxis("Horizontal");
                    if (!rail.isLoop) { distanceOnRail = Mathf.Clamp(distanceOnRail, 0, rail.GetLength); }
                }
                transform.position = rail.GetPosition(distanceOnRail);
            }
            else
            {
                List<Vector3> projections = new List<Vector3>();
                for (int i = 0; i < rail.GetPoints.Count - 1; i++)
                { projections.Add(MathUtils.GetNearestPointOnSegment(rail.GetPoints[i].position, rail.GetPoints[i + 1].position, target.position)); }

                if (projections.Count > 0)
                {
                    Vector3 smallestProjection = projections[0];
                    for (int i = 1; i < projections.Count; i++)
                    {
                        if (Vector3.Distance(projections[i], target.position) < Vector3.Distance(smallestProjection, target.position))
                        { smallestProjection = projections[i]; }
                    }
                    transform.position = smallestProjection;
                }
            }

        
            pitch = CalculatePitch(target);
            yaw = CalculateYaw(target);
        }
    }

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
