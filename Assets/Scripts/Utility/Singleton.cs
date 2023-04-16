using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static object _lock = new object();
    private static T _instance;
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject gameObject = new GameObject();
                        _instance = gameObject.AddComponent<T>();
                        gameObject.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(gameObject);
                    }
                }
            }
            return _instance;
        }
    }
}
