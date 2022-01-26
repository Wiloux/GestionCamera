using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 3;

    private Camera mainCamera;

    private CameraConfiguration averageConfig;

    private List<AView> activeViews = new List<AView>();
    public void AddView(AView view) { if (!activeViews.Contains(view)) { activeViews.Add(view); } }
    public void RemoveView(AView view) { activeViews.Remove(view); }
    public void ClearViews() { activeViews.Clear(); }
    public void SetViews(List<AView> newViews) { activeViews = newViews; }

    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }
    private void Awake()
    {
        if (Instance != null) { Debug.LogWarning("CameraController instance has been destroyed since there was already one in the scene!"); Destroy(gameObject); }
        _instance = this;
    }

    private void Start()
    {
        if (!mainCamera && Camera.main) { mainCamera = Camera.main; }
    }

    private void Update()
    {
        float s = speed * Time.deltaTime;
        averageConfig = (s < 1) ? AverageConfiguration(averageConfig, (1 - (speed * Time.deltaTime)), AverageConfiguration(activeViews), (speed * Time.deltaTime)) : AverageConfiguration(activeViews);

        SetCameraConfiguration();
    }

    private void SetCameraConfiguration()
    {
        if (mainCamera && averageConfig != null)
        {
            mainCamera.fieldOfView = averageConfig.fov;
            mainCamera.transform.position = averageConfig.GetPosition();
            mainCamera.transform.rotation = averageConfig.GetRotation();
        }
    }

    public CameraConfiguration AverageConfiguration(List<AView> views)
    {
        if (views.Count == 0) { return null; }

        Vector2 yawSum = Vector2.zero;
        float averagePitch = 0;
        float averageRoll = 0;
        float averageDistance = 0;
        float averageFOV = 0;
        Vector3 averagePivot = Vector3.zero;

        float sumWeights = 0;

        int validViews = 0;
        foreach (AView view in views)
        {
            CameraConfiguration viewConfig = view.GetConfiguration();
            if (viewConfig != null)
            {
                // Average Pitch and Roll
                averagePitch += viewConfig.pitch * view.weight;
                averageRoll += viewConfig.roll * view.weight;

                // Average Yaw
                yawSum += new Vector2(Mathf.Cos(viewConfig.yaw * Mathf.Deg2Rad),
                Mathf.Sin(viewConfig.yaw * Mathf.Deg2Rad)) * view.weight;

                // Average Pivot
                averagePivot += viewConfig.pivot * view.weight;

                // Average Distance
                averageDistance += viewConfig.distance * view.weight;

                // Average FOV
                averageFOV += viewConfig.fov * view.weight;

                // Sum of all Views Weights -> used in averaging later
                sumWeights += view.weight;

                validViews++;
            }
        }

        if (validViews == 0) { return null; }

        return new CameraConfiguration(Vector2.SignedAngle(Vector2.right, yawSum), averagePitch / sumWeights, averageRoll / sumWeights, averagePivot / sumWeights, averageDistance / sumWeights, averageFOV / sumWeights);
    }

    public CameraConfiguration AverageConfiguration(CameraConfiguration configA, float weightA, CameraConfiguration configB, float weightB)
    {
        if (configA == null || configB == null) { return null; }

        // Average Pitch and Roll
        float averagePitch = configA.pitch * weightA + configB.pitch * weightB;
        float averageRoll = configA.roll * weightA + configB.roll * weightB;

        // Average Yaw
        Vector2 yawSum = new Vector2(Mathf.Cos(configA.yaw * Mathf.Deg2Rad),
        Mathf.Sin(configA.yaw * Mathf.Deg2Rad)) * weightA;
        yawSum += new Vector2(Mathf.Cos(configB.yaw * Mathf.Deg2Rad),
        Mathf.Sin(configB.yaw * Mathf.Deg2Rad)) * weightB;

        // Average Pivot
        Vector3 averagePivot = configA.pivot * weightA + configB.pivot * weightB;

        // Average Distance
        float averageDistance = configA.distance * weightA + configB.distance * weightB;

        // Average FOV
        float averageFOV = configA.fov * weightA + configB.fov * weightB;

        // Sum of all Views Weights -> used in averaging later
        float sumWeights = weightA + weightB;

        return new CameraConfiguration(Vector2.SignedAngle(Vector2.right, yawSum), averagePitch / sumWeights, averageRoll / sumWeights, averagePivot / sumWeights, averageDistance / sumWeights, averageFOV / sumWeights);
    }

    public void OnDrawGizmos()
    {
        if (!mainCamera || averageConfig == null) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(averageConfig.pivot, 0.25f);
        Vector3 position = averageConfig.GetPosition();
        Gizmos.DrawLine(averageConfig.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, averageConfig.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, averageConfig.fov, 0.5f, 0f, mainCamera.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }
}

[System.Serializable]
public class CameraConfiguration
{
    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch;
    [HideInInspector] public float roll;

    [HideInInspector] public Vector3 pivot;

    [HideInInspector] public float distance;

    [HideInInspector] public float fov = 90;


    #region Constructors
    public CameraConfiguration()
    {
        yaw = 0;
        pitch = 0;
        roll = 0;
        pivot = Vector3.zero;
        distance = 0;
        fov = 90;
    }

    public CameraConfiguration(CameraConfiguration config)
    {
        yaw = config.yaw;
        pitch = config.pitch;
        roll = config.roll;
        pivot = config.pivot;
        distance = config.distance;
        fov = config.fov;
    }

    public CameraConfiguration(float yaw, float pitch, float roll)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
        pivot = Vector3.zero;
        distance = 0;
        fov = 90;
    }

    public CameraConfiguration(float yaw, float pitch, float roll, float fov)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
        pivot = Vector3.zero;
        distance = 0;
        this.fov = fov;
    }

    public CameraConfiguration(float yaw, float pitch, float roll, Vector3 pivot)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
        this.pivot = pivot;
        distance = 0;
        fov = 90;
    }

    public CameraConfiguration(float yaw, float pitch, float roll, Vector3 pivot, float fov)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
        this.pivot = pivot;
        distance = 0;
        this.fov = fov;
    }

    public CameraConfiguration(float yaw, float pitch, float roll, Vector3 pivot, float distance, float fov)
    {
        this.yaw = yaw;
        this.pitch = pitch;
        this.roll = roll;
        this.pivot = pivot;
        this.distance = distance;
        this.fov = fov;
    }
    #endregion

    public Quaternion GetRotation()
    {
        return Quaternion.Euler(pitch, yaw, roll);
    }

    public Vector3 GetPosition()
    {
        return pivot + GetRotation() * (Vector3.back * distance);
    }
}
