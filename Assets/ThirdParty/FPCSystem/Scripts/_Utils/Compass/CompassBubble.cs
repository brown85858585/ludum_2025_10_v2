// Author - Vikram, http://8bitmemories.blogspot.com
//

using UnityEngine;
using System.Collections;

public class CompassBubble : MonoBehaviour
{
	//For holding the compass textures
    public Texture2D bg;
    public Texture2D bubble;

	public float bubbleSpeed = 5; // neddle rotation speed
	public float maxBubbleVibration = 0.05f; // Maximum neddle ramdom innacuracy.

	private float north = 0; //"North" in the game --  0 for + Z Axis, 90 for + X Axis, etc

	public float radius; //Where the compass bubble needs to be inside the compass
	public Vector2 center; //Where the compass needs to be placed

    //Size in pixels about how big the compass should be
    public Vector2 compassSize;
    public Vector2 bubbleSize;

	private Rect compassRect;
	private float internalAngle;			// Player lerped rotation (using the var neddle peed).
	private float rotCam, rot, rotAux, x, y;// Players actual rotation


    public float GetCurrentAngle()
    {
        return rotCam;
    }

    // Use this for initialization
    void Start()
    {
        //Set the placement of compass from size and center
        compassRect = new Rect(center.x - (compassSize.x * 0.5f), center.y - (compassSize.y * 0.5f), compassSize.x, compassSize.y);
    }

    void OnGUI()
    {
        // Draw background
        GUI.DrawTexture(compassRect, bg);
        // Draw bubble
        GUI.DrawTexture(new Rect((center.x + x) - (bubbleSize.x * 0.5f), (center.y + y) - (bubbleSize.y * 0.5f), bubbleSize.x, bubbleSize.y), bubble);
    }

    // Update is called once per frame
    void Update()
    {
        rotCam = Camera.main.transform.eulerAngles.y;
        rotCam = ClampAngle(rotCam);

        // Note -90 compensation cos north is along 2D Y axis
        rot = ((-90 + rotCam) - north) * Mathf.Deg2Rad;
        if (((rotAux > 0) && (rot < 0)) || ((rotAux < 0) && (rot > 0)))
            internalAngle = rot;
        rotAux = rot;

        internalAngle = Mathf.LerpAngle(internalAngle, rot, Time.deltaTime * bubbleSpeed); // Rotation lerped value.
        internalAngle = internalAngle + Random.Range(-maxBubbleVibration, maxBubbleVibration); // bubble innacuracy.

        // Bubble position
        x = radius * Mathf.Cos(internalAngle);
        y = radius * Mathf.Sin(internalAngle);
    }

	// Internal function to clamp, dealing with the 360ª problem.
	private float ClampAngle (float angle){
		if (angle < -360F)
			angle += 360F;
		else if (angle > 360F)
			angle -= 360F;
		return angle;
	}

}