using UnityEngine;
using System.Collections;

// Cange the bumpmap frames to create a waterDrop effect in screen. It's called when exiting the water.
public class AnimatedRenderer : MonoBehaviour
{
	public float fps = 10.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private Renderer _renderer;
    private float fadeTime = 5;

    void Start()
    {
		_renderer = GetComponent<Renderer>();
		NextFrame();
    }

    void NextFrame()
    {
		_renderer.material.SetTexture("_BumpMap", frames[frameIndex]);
		frameIndex = (frameIndex + 1) % frames.Length;
        if (frameIndex == (frames.Length - 1))
            StopAnimation();
    }

	/*IEnumerator NextFrame(float _ratio)
	{
		do {
			_renderer.material.SetTexture("_BumpMap", frames[frameIndex]);
			frameIndex = (frameIndex + 1) % frames.Length;
			if (frameIndex == frames.Length - 1) StopAnimation();
			yield return new WaitForSeconds(_ratio);
		} while(true);
	}*/

    public void SetFadeTime(float _time){ fadeTime = _time; }

    public void StartAnimation()
    {
		frameIndex = 0;
		NextFrame();
		Invoke("StopAnimation", fadeTime);
		InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    public void StopAnimation()
    {
        //Debug.Log("Stopping Animation.");
		CancelInvoke("NextFrame");
		frameIndex = 0;
		NextFrame();
    }

}