using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AView : MonoBehaviour
{

    public float weight = 1;

    public bool isActiveOnStart = true;
    public void SetActive(bool isActive) 
    {
        if (CameraController.Instance) 
        {
            if (isActive) { CameraController.Instance.AddView(this); }
            if (!isActive) { CameraController.Instance.RemoveView(this); }
        }
    }

    private void Start() { if (isActiveOnStart) { SetActive(true); } }

    public virtual CameraConfiguration GetConfiguration() { return null; }

    public virtual void OnDrawGizmos() { }

}
