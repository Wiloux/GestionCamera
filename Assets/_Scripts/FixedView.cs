using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedView : AView
{
    public float yaw;
    public float pitch;
    public float roll;
    public float fov;

    public override CameraConfiguration GetConfiguration() { return new CameraConfiguration(yaw, pitch, roll, transform.position, fov); }
}
