using MigalhaSystem.Extensions;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Profiling;

public static class CheckObjectInView {

    private static CinemachineBrain currentCamera => CinemachineBrain.GetActiveBrain(0);

    public static bool IsObjectInView(params Vector3[] points) {

        if (points == null || points.Length <= 0) return false;
        foreach (Vector3 point in points)
            if (IsObjectInView(point)) return true;
        return false;

    }

    public static bool IsObjectInView(this Transform _Object, float m_Offset = 0) => IsObjectInView(_Object.position, m_Offset);

    public static bool IsObjectInView(this Vector2 _Position, float m_Offset = 0) => IsObjectInView((Vector3)_Position, m_Offset);

    public static bool IsObjectInView(this Vector3 _Position, float m_Offset = 0) {

        if (currentCamera == null)
            return false;

        Profiler.BeginSample("InCamera");
        Vector3 viewportPosition = GetWorldToViewportPosition(_Position);
        var cameraScaleCorrection = GetScaleCorrection();

        var inObjectView = viewportPosition.x >= 0 - m_Offset * cameraScaleCorrection && viewportPosition.x <= 1 + m_Offset * cameraScaleCorrection &&
                        viewportPosition.y >= 0 - m_Offset && viewportPosition.y <= 1 + m_Offset;

        Profiler.EndSample();
        return inObjectView;

    }

    public static Vector2 GetCameraSize() {

        if (currentCamera == null)
            return Vector2.zero;
        return currentCamera.OutputCamera.ViewportToWorldPoint(Vector2.one) - currentCamera.OutputCamera.transform.position;

    }

    public static float GetScaleCorrection()
        => currentCamera.OutputCamera.pixelHeight / (float)currentCamera.OutputCamera.pixelWidth;

    public static Vector3 GetWorldToViewportPosition(this Vector3 _Position)
        => currentCamera.OutputCamera.WorldToViewportPoint(_Position);

    public static Vector3 GetWorldToScreenPosition(this Vector3 _Position)
        => currentCamera.OutputCamera.WorldToScreenPoint(_Position);

    public static Vector3 GetViewportToWorld(this Vector3 _Position)
        => currentCamera.OutputCamera.ViewportToWorldPoint(_Position);

    public static float GetCameraOrthographicSize()
        => (currentCamera == null) ? 0 : currentCamera.OutputCamera.orthographicSize;

    public static void SetCameraOrthographicSize(float _OrtographicSize) {

        CinemachineCamera virtualCam = currentCamera.ActiveVirtualCamera as CinemachineCamera;
        if (virtualCam != null) {

            var lens = virtualCam.State.Lens;
            lens.OrthographicSize = _OrtographicSize;
            virtualCam.Lens = lens;

        }

    }

    public static Vector3 GetPositionInsideOfCameraRange(FloatRange _SpawnDistanceRange) => GetPositionOfCameraRange(_SpawnDistanceRange, 1);
    public static Vector3 GetPositionOutsideOfCameraRange(FloatRange _SpawnDistanceRange) => GetPositionOfCameraRange(_SpawnDistanceRange, GetScaleCorrection());
    public static Vector3 GetPositionOfCameraRange(FloatRange _SpawnDistanceRange, float _CameraScaleCorrection) {

        float Offset = _SpawnDistanceRange.GetRandomValue() - 1;
        _CameraScaleCorrection = (Offset > 0)? _CameraScaleCorrection : 1f;

        float spawnX;
        float spawnY;
        float randomValue = Random.value;

        if (randomValue > 0.5f) {

            spawnX = Random.Range(0 - Offset * _CameraScaleCorrection, 1 + Offset * _CameraScaleCorrection);
            spawnY = (randomValue < 0.75f) ? 0 - Offset : 1 + Offset;

        } else {

            spawnX = (randomValue < 0.25f) ? 0 - Offset * _CameraScaleCorrection : 1 + Offset * _CameraScaleCorrection;
            spawnY = Random.Range(0 - Offset, 1 + Offset);

        }

        return Vector3.Scale(GetViewportToWorld(new Vector2(spawnX, spawnY)), new Vector3(1, 1, 0));

    }

    public static float GetNearClip() {
        return currentCamera.OutputCamera.nearClipPlane;
    }

}
