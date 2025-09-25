using UnityEngine;

public class FramerateLimit : MonoBehaviour {

    public bool limitFrameRate = true;
    [ShowWhen("limitFrameRate")]
    public int targetFramerate = 60;

	void Awake ()
    {
        if (limitFrameRate)
        {
            Application.targetFrameRate = targetFramerate;
        }
    }
	
}
