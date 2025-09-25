using UnityEngine;
using System.Collections;

public class AnimatedProjector : MonoBehaviour{
    public float fps = 30.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private Projector projector;

    void Start()
	{
        projector = GetComponent<Projector>();
		StartCoroutine("NextFrame", 1.0f/fps);
    }

	IEnumerator NextFrame(float _ratio)
	{
		do {
	        projector.material.SetTexture("_ShadowTex", frames[frameIndex]);
	        frameIndex = (frameIndex + 1) % frames.Length;
			yield return new WaitForSeconds(_ratio);
		} while(true);
    }
}