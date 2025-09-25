using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CompassNeedle : MonoBehaviour
{
    public Texture2D bg; // compass background texture.
    public Texture2D needle;// compass neddle texture.

	public float neddleSpeed = 5; // neddle rotation speed
	public float maxNeedleVibration = 2; // Maximum neddle ramdom innacuracy.

	// Compass On-screen place
	public Vector2 center = new Vector2(100, 100); // Center in pixels where the compass will be drawn

	// Compass size in pixels
	public Vector2 compassSize = new Vector2(64, 64);
	public Vector2 neddleSize = new Vector2(64, 64);

    private Vector2 centerPoint; // Compass center point

    //private Transform PlayerTransform;  // Player transform to see its rotation.
    private Rect compassRect; // Background compass texture Rect
    private Rect needleRect; // Neddle compass texture Rect
    private float rot; // Players actual rotation
    private float internalAngle; // Player lerped rotation speed using the  speedneddle).

    public float GetCurrentAngle()
    {
        return internalAngle;
    }

	//Set the placement of compass from size and center
	private void UpdateCompassRect(){
		compassRect = new Rect(center.x - (compassSize.x * 0.5f), center.y - (compassSize.y * 0.5f), compassSize.x, compassSize.y);
		needleRect = new Rect(center.x - (neddleSize.x * 0.5f), center.y - (neddleSize.y * 0.5f), neddleSize.x, neddleSize.y);
	}

    void Start()
    {
		UpdateCompassRect ();
    }

    void OnGUI()
    {
		UpdateCompassRect ();
        
		// Draw background
        GUI.DrawTexture(compassRect, bg);
        
		// Draw the Needle (this is how you can rotate an OnGui texture; using GUI.Matrix).
        centerPoint = new Vector2(needleRect.x + (needleRect.width * 0.5f), needleRect.y + (needleRect.height * 0.5f)); // find the neddle center
        Matrix4x4 svMat = GUI.matrix; // save gui matrix
        GUIUtility.RotateAroundPivot(internalAngle, centerPoint); // prepare matrix to rotate
        GUI.DrawTexture(needleRect, needle); // draw the needle rotated by angle
        GUI.matrix = svMat; // restore gui matrix
    }

    // Update is called once per frame
    void Update()
    {
        rot = Camera.main.transform.eulerAngles.y; // Player Rotation.
        float velo = 0;
        internalAngle = Mathf.SmoothDampAngle(internalAngle, rot, ref velo, Time.deltaTime * neddleSpeed); // Rotation lerped value.
        internalAngle = internalAngle + Random.Range(-maxNeedleVibration, maxNeedleVibration); // neddle innacuracy.
    }

}