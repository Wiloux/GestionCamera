using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class TriggeredViewVolume : AViewVolume
{
    public LayerMask target = 0;
    private int targetLayer = 0;


    private void Start() 
    {
        targetLayer = (int)Mathf.Log(target.value, 2);

        gameObject.GetComponent<Collider>().isTrigger = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        // Detect if target was inside from start
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider) 
        {
            Collider[] objectsInside = Physics.OverlapBox(transform.position + boxCollider.center, Vector3.Scale(boxCollider.size, transform.localScale) / 2, transform.rotation, target);
            if (objectsInside.Length > 0) { SetActive(true); }
            return; 
        }

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider) 
        {
            Collider[] objectsInside = Physics.OverlapSphere(transform.position + sphereCollider.center, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z), target);
            if (objectsInside.Length > 0) { SetActive(true); }
            return; 
        }

    }

    private void OnTriggerEnter(Collider other) { if (other.gameObject.layer == targetLayer) { SetActive(true); } }
    private void OnTriggerExit(Collider other) { if (other.gameObject.layer == targetLayer) { SetActive(false); } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green * new Vector4(1, 1, 1, 0.3f);
        
        try
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }
        }
        catch(System.Exception) { }

        try
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z));
            }
        }
        catch (System.Exception) { }
    }
}
