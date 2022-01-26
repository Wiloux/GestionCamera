using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalViewVolume : AViewVolume
{
    public override float ComputeSelfWeight() { return 1; }

    private void Start() { SetActive(true); }


}
