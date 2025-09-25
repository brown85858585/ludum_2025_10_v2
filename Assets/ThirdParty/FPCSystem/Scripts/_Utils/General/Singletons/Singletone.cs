// Simple Singleton that creates itself if doesn't exists

using UnityEngine;

public class Singletone<T> : MonoBehaviour where T : MonoBehaviour 
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

	public void Awake ()
	{
		if (!Application.isPlaying) { return; }
		_instance = this as T;			
	}

	public static bool IsDestroyed 
	{
		get { return (_instance == null); }
	}

}
