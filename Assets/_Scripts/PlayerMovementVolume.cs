using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlayerMovementVolume : MonoBehaviour
{
    public CameraRail rail;
    public LayerMask target = 0;
    private int targetLayer = 0;

    private bool isActive = false;

    private void Start()
    {
        targetLayer = (int)Mathf.Log(target.value, 2);

        if (PlayerController.Instance)
        {
            Collider[] objectsInside = Physics.OverlapBox(transform.position + GetComponent<BoxCollider>().center, Vector3.Scale(GetComponent<BoxCollider>().size, transform.localScale) / 2, transform.rotation, target);
            if (objectsInside.Length > 0) { PlayerController.Instance.SetRail(rail); isActive = true; }
        }
    }

    private void OnTriggerEnter(Collider other) { if (!isActive && PlayerController.Instance && other.gameObject.layer == targetLayer) { PlayerController.Instance.SetRail(rail); isActive = true; } }
    private void OnTriggerExit(Collider other) { if (isActive && PlayerController.Instance && other.gameObject.layer == targetLayer) { PlayerController.Instance.SetRail(null); isActive = false; } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        try
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;

                Gizmos.color *= new Vector4(1, 1, 1, 0.3f); 
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }
        }
        catch (System.Exception) { }
    }
}
