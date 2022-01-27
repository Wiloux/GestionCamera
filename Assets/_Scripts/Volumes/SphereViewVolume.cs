using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereViewVolume : AViewVolume
{
    public float outerRadius = 3;
    public float innerRadius = 1;

    public Transform target;

    private float distance = 0;

    private void OnValidate()
    {
        if (outerRadius < 0) { outerRadius = 0; }
        if (innerRadius < 0) { innerRadius = 0; }
        if (outerRadius < innerRadius) { outerRadius = innerRadius; }
    }

    private void Update()
    {
        if (target)
        {
            distance = Vector3.Distance(transform.position, target.position);
            if (distance <= outerRadius && !IsActive) { SetActive(true); }
            else if (distance > outerRadius && IsActive) { SetActive(false); }
        }
    }

    public override float ComputeSelfWeight() { return (1 - Mathf.Clamp01((distance - innerRadius) / (outerRadius - innerRadius))); }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow * new Vector4(1, 1, 1, 0.3f); ;
        Gizmos.DrawSphere(transform.position, outerRadius);
        Gizmos.color = Color.green * new Vector4(1, 1, 1, 0.3f); ;
        Gizmos.DrawSphere(transform.position, innerRadius);
    }
}
