using UnityEngine;

public class CameraFitter : MonoBehaviour
{
    float originalSize = 2.4f;
    float targeteAspect = 1080f / 1920f;
    private void Awake()
    {
        Camera.main.orthographicSize = originalSize * (targeteAspect / Camera.main.aspect);
    }
}
