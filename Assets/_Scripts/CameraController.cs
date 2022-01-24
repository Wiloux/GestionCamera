using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;

    private CameraConfiguration averageConfig;

    private List<AView> activeViews = new List<AView>();
    public void AddView(AView view) { if (!activeViews.Contains(view)) { activeViews.Add(view); UpdateAverageConfiguration(); } }
    public void RemoveView(AView view) { activeViews.Remove(view); UpdateAverageConfiguration(); }

    private static CameraController _instance;
    public static CameraController Instance { get { return _instance; } }
    private void Awake()
    {
        if (Instance != null) { Debug.LogWarning("CameraController instance has been destroyed since there was already one in the scene!"); Destroy(gameObject); }
        _instance = this;
    }

    private void Start()
    {
        if (!camera && Camera.main) { camera = Camera.main; }
    }

    private void Update()
    {
        SetCameraConfiguration();
    }

    private void SetCameraConfiguration()
    {
        if (camera && averageConfig != null)
        {
            camera.fieldOfView = averageConfig.fov;
            camera.transform.position = averageConfig.GetPosition();
            camera.transform.rotation = averageConfig.GetRotation();
        }
    }

    public void UpdateAverageConfiguration()
    {
        Vector2 yawSum = Vector2.zero;
        float averagePitch = 0;
        float averageRoll = 0;
        float averageDistance = 0;
        float averageFOV = 0;
        Vector3 averagePivot = Vector3.zero;

        float sumWeights = 0;

        foreach (AView view in activeViews)
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
            }
        }

        averageConfig = new CameraConfiguration(Vector2.SignedAngle(Vector2.right, yawSum), averagePitch / sumWeights, averageRoll / sumWeights, averagePivot / sumWeights, averageDistance / sumWeights, averageFOV / sumWeights);
    }

    public Coroutine Lerp(AView a, AView b, float length, System.Action start = null, System.Action update = null, System.Action end = null)
    {
        return StartCoroutine(LerpLoop(a, b, length, start, update, end));
    }

    private IEnumerator LerpLoop(AView a, AView b, float length, System.Action start = null, System.Action update = null, System.Action end = null)
    {
        start?.Invoke();

        if (a && b && length > 0)
        {
            activeViews = new List<AView>();

            AddView(a);
            AddView(b);

            float weightA = a.weight;
            float weightB = b.weight;

            a.weight = weightA;
            b.weight = 0;
            UpdateAverageConfiguration();

            float tx = Time.timeSinceLevelLoad;
            float elapsedTime = 0;
            while (elapsedTime < length)
            {
                update?.Invoke();

                a.weight = (1 - (elapsedTime / length)) * weightA;
                b.weight = (elapsedTime / length) * weightB;
                UpdateAverageConfiguration();

                elapsedTime += Time.timeSinceLevelLoad - tx;
                tx = Time.timeSinceLevelLoad;

                yield return null;
            }

            a.weight = 0;
            b.weight = weightB;
            UpdateAverageConfiguration();
        }

        end?.Invoke();
    }

    public void DrawGizmos(Color color)
    {
        if (!camera || averageConfig != null) { return; }

        Gizmos.color = color;
        Gizmos.DrawSphere(averageConfig.pivot, 0.25f);
        Vector3 position = averageConfig.GetPosition();
        Gizmos.DrawLine(averageConfig.pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, averageConfig.GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, averageConfig.fov, 0.5f, 0f, Camera.main.aspect);
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
