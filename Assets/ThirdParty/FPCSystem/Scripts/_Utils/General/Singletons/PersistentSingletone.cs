// Singleton that creates itself if doesn't exists but if it does, destroy itself
// The older PersistenSingleton will be always the only Singleton that will survive.

using UnityEngine;

public class PersistentSingletone<T> : MonoBehaviour where T : MonoBehaviour 
{

	private static T _instance = null;

	public static T instance {
		get {
			if (_instance == null) 
			{
				_instance = GameObject.FindObjectOfType(typeof(T)) as T;
				if (_instance == null) 
				{
					_instance = new GameObject ().AddComponent<T> ();
					_instance.gameObject.name = _instance.GetType ().Name;
				}
			}

			return _instance;
		}
	}

	public virtual void Awake ()
	{
		if (!Application.isPlaying) { return; }

        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

	public static bool IsDestroyed 
	{
		get { return (_instance == null); }
	}

}
