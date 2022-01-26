using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AView : MonoBehaviour
{

    public float weight = 1;

    public void SetActive(bool isActive) 
    {
        if (CameraController.Instance) 
        {
            if (isActive) { CameraController.Instance.AddView(this); }
            else { CameraController.Instance.RemoveView(this); }
        }
    }

    public virtual CameraConfiguration GetConfiguration() { return null; }

    public virtual void OnDrawGizmos() { }

}
