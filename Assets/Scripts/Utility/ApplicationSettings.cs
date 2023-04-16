using UnityEngine;

public class ApplicationSettings : Singleton<ApplicationSettings>
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
    }
}
