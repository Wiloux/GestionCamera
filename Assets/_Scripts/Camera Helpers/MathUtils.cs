using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target)
    {
        Vector3 ab = b - a;
        float dot = Mathf.Clamp(Vector3.Dot((target - a), ab.normalized), 0, ab.magnitude);

        return a + ab.normalized * dot;
    }

    public static Vector3 LinearBezier(Vector3 A, Vector3 B, float t)
    {
        return (1-t) * A + t * B;
        //return (B - A).normalized * (Vector3.Distance(A, B) / t);
    }

    public static Vector3 QuadraticBezier(Vector3 A, Vector3 B, Vector3 C, float t)
    {
        return (1 - t) * LinearBezier(A, B, t) + t * LinearBezier(B, C, t);
    }

    public static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
    {
        return (1 - t) * QuadraticBezier(A, B, C, t) + t * QuadraticBezier(B, C, D, t);
    }
}
