using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLerp : MonoBehaviour
{
    public AView viewA;
    public AView viewB;
    [Range(0, 10)] public float transitionSpeed = 3f;

    bool isA = true;

    void Start()
    {
        if(CameraController.Instance)
        {
            CameraController.Instance.ClearViews();
            CameraController.Instance.AddView(viewA);
        }
    }

    void Update()
    {

        if (CameraController.Instance && Input.GetKeyDown(KeyCode.Space)) { CameraController.Instance.SetViews(new List<AView> { isA ? viewB : viewA }); isA = !isA; }
    }
}
