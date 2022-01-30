using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AViewVolume : MonoBehaviour
{
    public int priority = 0;
    public AView view;
    public bool isCutOnSwitch = false;

    public virtual float ComputeSelfWeight() { return 1; }

    protected bool IsActive { get; private set; }

    private void Awake() { SetActive(false); }

    protected void SetActive(bool isActive)
    {
        if(ViewVolumeBlender.Instance)
        {
            if (isActive) { ViewVolumeBlender.Instance.AddVolume(this); }
            else { ViewVolumeBlender.Instance.RemoveVolume(this); if (view) { view.ResetWeight(); } }
            IsActive = isActive;

            if (isCutOnSwitch)
            {
                ViewVolumeBlender.Instance.Update();
                if (CameraController.Instance) { CameraController.Instance.Cut(); }
            }
        }
    }
}
