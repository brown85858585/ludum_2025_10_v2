using UnityEngine;
using System.Collections;

public class SlowMotion : MonoBehaviour {
	
	[Range(0.01f, 0.99f)] public float TimeToScale = 0.1f;
	public bool isSlowMotion = false;
	
	private float TimeScaleOriginal = 1;

	// Use this for initialization
	void Start () {
		TimeScaleOriginal = Time.timeScale;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.F11))
			isSlowMotion = !isSlowMotion;
		
		if(isSlowMotion && Time.timeScale != TimeToScale)
			Time.timeScale = TimeToScale;
		else
		if(!isSlowMotion && Time.timeScale != TimeScaleOriginal)
			Time.timeScale = TimeScaleOriginal;
	}
}
