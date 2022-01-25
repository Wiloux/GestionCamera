using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFollowView : AView
{

    public float roll;
    public float fov;

    public Transform target;



    public float CalculateYaw(Transform target)
    {
       Vector3 dir = (transform.position - target.position).normalized;

       return Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    public float CalculatePitch(Transform target)
    {
        Vector3 dir = (transform.position - target.position).normalized;

        return -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
    }

}
