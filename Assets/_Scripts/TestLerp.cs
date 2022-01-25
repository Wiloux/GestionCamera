using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLerp : MonoBehaviour
{
    public AView viewA;
    public AView viewB;
    [Range(0, 10)] public float duration = 3f;

    private bool isLerping = false;

    void Start()
    {
        if(CameraController.Instance)
        {
            CameraController.Instance.ClearViews();
            CameraController.Instance.AddView(viewA);
            CameraController.Instance.UpdateActiveViewsAverageConfiguration();
        }
    }

    void Update()
    {
        if(!isLerping && CameraController.Instance && Input.GetKeyDown(KeyCode.Space))
            CameraController.Instance.ChangeTarget(new List<AView> { viewB }, duration, 10, delegate { isLerping = true; }, null, delegate { isLerping = false; });
    }
}
