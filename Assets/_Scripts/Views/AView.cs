using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AView : MonoBehaviour
{
    [SerializeField] private float weight = 1;

    private float currentWeight = 1;

    public float Weight { get { return currentWeight; } set { currentWeight = value; } }
    public void ResetWeight() { currentWeight = weight; }

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
