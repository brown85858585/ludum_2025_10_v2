using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CompassFullscreen : MonoBehaviour
{
    public Texture2D compassTexture;

    private float cameraAngle;
    private float internalAngle; 			 // Player lerped rotation (using the var neddle peed).

	public float compassTurnSpeed = 5; 		 // neddle rotation speed
	public float maxCompassVibration = 0.1f; // Maximum neddle ramdom innacuracy.

	private float texWidth = 1024;
	public float texHeight = 22;

    private float compX;


	public float GetCurrentAngle()
    {
        return internalAngle;
    }

    void Start()//texHeight = compassTexture.height * texWidth / compassTexture.width; // NOP.
    {
        texWidth = (((360 / Camera.main.fieldOfView) * Camera.main.aspect) * Screen.width) * 0.25f;
    }

	void Update()
	{
		cameraAngle = Camera.main.transform.eulerAngles.y;
		cameraAngle = ClampAngle(cameraAngle);

		internalAngle = Mathf.LerpAngle(internalAngle, cameraAngle, Time.deltaTime * compassTurnSpeed); // Rotation lerped value.
		internalAngle = internalAngle + Random.Range(-maxCompassVibration, maxCompassVibration); // neddle innacuracy.
		internalAngle = ClampAngle(internalAngle);
		compX = (Screen.width / 2) - ((internalAngle / 360) * texWidth);
	}

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(compX - texWidth, 0, texWidth, texHeight), compassTexture);
        GUI.DrawTexture(new Rect(compX, 0, texWidth, texHeight), compassTexture);
    }

    // Internal function to clamp, dealing with the 360ª problem.
    private float ClampAngle(float angle)
    {
        if (angle > 180)
            angle = angle - 360;
        else if (angle < -180)
            angle = angle + 360;
        return angle;
    }

}