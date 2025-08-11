using UnityEngine;

public abstract class FastSingleton<T> : MonoBehaviour where T : FastSingleton<T>
{
    private static T _instance;
    private static bool _instantiated;

    public static T instance
    {
        get
        {
            if (_instantiated)
                return _instance;

            _instance = (T)FindAnyObjectByType(typeof(T));
            if (!_instance)
            {
                GameObject go = new GameObject(typeof(T).ToString());
                _instance = go.AddComponent<T>();
            }

            _instantiated = true;
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (!_instance)
        {
            _instance = (T)this;
            _instantiated = true;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _instantiated = false;
        }
    }
}
