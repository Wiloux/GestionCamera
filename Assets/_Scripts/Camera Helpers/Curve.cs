using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;
    public Transform d;

    private int echantillions = 500;

    public Vector3 GetPosition(float t)
    {
        if (!a || !b || !c || !d || t < 0) { return Vector3.zero; }

        return MathUtils.CubicBezier(a.localPosition, b.localPosition, c.localPosition, d.localPosition, t);
    }

    public Vector3 GetPosition(float t, Matrix4x4 localToWorldMatrix)
    {
        if (!a || !b || !c || !d ||  t < 0 || localToWorldMatrix == null) { return Vector3.zero; }

        return localToWorldMatrix.MultiplyPoint(GetPosition(t));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan * new Color(1, 1, 1, 0.5f);

        if (a) { Gizmos.DrawSphere(a.position, 0.175f); }
        if (b) { Gizmos.DrawSphere(b.position, 0.175f); }
        if (c) { Gizmos.DrawSphere(c.position, 0.175f); }
        if (d) { Gizmos.DrawSphere(d.position, 0.175f); }
        if (a && b) { Gizmos.DrawLine(a.position, b.position); }
        if (b && c) { Gizmos.DrawLine(b.position, c.position); }
        if (c && d) { Gizmos.DrawLine(c.position, d.position); }

        Gizmos.color = Color.red;

        for (int i = 0; i < echantillions - 1; i++)
        {
            float t0 = (float)i / echantillions;
            float t1 = (float)(i + 1) / echantillions;
            Gizmos.DrawLine(GetPosition(t0, transform.localToWorldMatrix), GetPosition(t1, transform.localToWorldMatrix));
        }
    }
}
