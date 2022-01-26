using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class TriggeredViewVolume : AViewVolume
{
    public LayerMask target = 0;
    public bool gizmoFilled = false;

    private void Start() 
    { 
        gameObject.GetComponent<Collider>().isTrigger = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // Detect if target was inside from start
        BoxCollider boxCollider = (BoxCollider)GetComponent<Collider>();
        if (boxCollider) 
        {
            Collider[] objectsInside = Physics.OverlapBox(transform.position + boxCollider.center, Vector3.Scale(boxCollider.size, transform.localScale) / 2, transform.rotation, target);
            if (objectsInside.Length > 0) { SetActive(true); }
            return; 
        }

        SphereCollider sphereCollider = (SphereCollider)GetComponent<Collider>();
        if (sphereCollider) 
        {
            Collider[] objectsInside = Physics.OverlapSphere(transform.position + sphereCollider.center, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z), target);
            if (objectsInside.Length > 0) { SetActive(true); }
            return; 
        }

    }

    private void OnTriggerEnter(Collider other) { if(other.gameObject.layer == target) { SetActive(true); } }
    private void OnTriggerExit(Collider other) { if (other.gameObject.layer == target) { SetActive(false); } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        try
        {
            BoxCollider boxCollider = (BoxCollider)GetComponent<Collider>();
            if (boxCollider)
            {
                if (gizmoFilled)
                { Gizmos.DrawCube(transform.position + boxCollider.center, Vector3.Scale(boxCollider.size, transform.localScale)); }
                else
                { Gizmos.DrawWireCube(transform.position + boxCollider.center, Vector3.Scale(boxCollider.size, transform.localScale)); }
                return;
            }
        }
        catch(System.Exception) { }

        try
        {
            SphereCollider sphereCollider = (SphereCollider)GetComponent<Collider>();
            if (sphereCollider)
            {
                if (gizmoFilled)
                { Gizmos.DrawSphere(transform.position + sphereCollider.center, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z)); }
                else
                { Gizmos.DrawWireSphere(transform.position + sphereCollider.center, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z)); }
                return;
            }
        }
        catch (System.Exception) { }
    }
}
