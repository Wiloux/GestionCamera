using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRail : MonoBehaviour
{

    public bool isLoop = false;

    private float length = 0;
    public float GetLength { get { return length; } }

    private List<Transform> points = new List<Transform>();
    public List<Transform> GetPoints { get { return points; } }

    private void Start()
    {
        RecalculateLength();
        RecalculatePoints();
    }

    private void OnValidate()
    {
        RecalculateLength();
        RecalculatePoints();
    }

    public void RecalculateLength()
    {
        length = 0;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount - 1; i++)
            { length += Vector3.Distance(transform.GetChild(i).position, transform.GetChild(i + 1).position); }
            if (isLoop) { length += Vector3.Distance(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position); }
        }
    }

    public void RecalculatePoints()
    {
        points = new List<Transform>();
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            { points.Add(transform.GetChild(i)); }
            if (isLoop) { points.Add(transform.GetChild(0)); }
        }
    }

    public Vector3 GetPosition(float distance)
    {
        float realDistance = Mathf.Abs(distance) != Mathf.Abs(length) ? distance % length : distance;

        if (transform.childCount > 0)
        {
            float len = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                int p1 = realDistance >= 0 ? i : points.Count - i - 1;
                int p2 = realDistance >= 0 ? i + 1 : points.Count - i - 2;
                float distance2pts = Mathf.Sign(realDistance) * Vector3.Distance(points[p1].position, points[p2].position);

                if (Mathf.Abs(len + distance2pts) >= Mathf.Abs(realDistance))
                { return points[p1].position + ((points[p2].position - points[p1].position).normalized * Mathf.Abs(realDistance - len)); }

                len += distance2pts;
            }
        }

        return Vector3.zero;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (transform.childCount > 0)
        {
            Gizmos.DrawSphere(transform.GetChild(0).position, 0.175f);

            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                Gizmos.DrawSphere(transform.GetChild(i + 1).position, 0.175f);
            }

            if (isLoop) { Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position); }
        }
    }

}
