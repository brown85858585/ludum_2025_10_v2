// This script is meant to show the compass angle and orientation using text (using the old OnGUI function).
// Drag this script in the compass you want. Right now is in the 3D compass.

using UnityEngine;
using System.Collections;

public class CompassText : MonoBehaviour
{
	public Rect position = new Rect(100, 100, 100, 100); // Position in pixels where the compass text will be drawn
	public Color GUIColor = Color.white;
    public GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.

	private CompassTypes compassActive = CompassTypes.None;
    private CompassNeedle CompassNeddle;
    private CompassBubble CompassBubble;
    private CompassFullscreen CompassLinear;
    private Compass3D Compass3D;
    private float angle; 		// compass current angle.


    // Figure out in witch compass this script is being placed (in the inspector).
    void Start()
    {
        if (GetComponent<CompassNeedle>() != null)
        {
			CompassNeddle = GetComponent<CompassNeedle>();
            compassActive = CompassTypes.Neddle;
        }
		else if (GetComponent<CompassBubble>() != null)
        {
			CompassBubble = GetComponent<CompassBubble>();
            compassActive = CompassTypes.Bubble;
        }
		else if (GetComponent<CompassFullscreen>() != null)
        {
			CompassLinear = GetComponent<CompassFullscreen>();
            compassActive = CompassTypes.Linear;
        }
		else if (GetComponent<Compass3D>() != null)
        {
			Compass3D = GetComponent<Compass3D>();
            compassActive = CompassTypes.Standard;
        }
        else
        {
            compassActive = CompassTypes.None;
        }
    }

    // Depending on the compass angle a message will be displayed in the screen.
    void OnGUI()
    {
        float currentAngle = Mathf.FloorToInt(angle);
        string compassText = null;
        if (((currentAngle >= 340) && (currentAngle <= 360)) || ((currentAngle >= 0) && (currentAngle <= 20)))
            compassText = "N";
        else if ((currentAngle > 20) && (currentAngle <= 70))
            compassText = "NE";
        else if ((currentAngle > 70) && (currentAngle <= 110))
            compassText = "E";
        else if ((currentAngle > 110) && (currentAngle <= 160))
            compassText = "SE";
        else if ((currentAngle > 160) && (currentAngle <= 200))
            compassText = "S";
        else if ((currentAngle > 200) && (currentAngle <= 250))
            compassText = "SW";
        else if ((currentAngle > 250) && (currentAngle <= 295))
            compassText = "W";
        else if ((currentAngle > 295) && (currentAngle < 340))
            compassText = "NW";
       
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleLeft;
        }
        else
            style.normal.textColor = GUIColor;

        GUI.Label(position, (compassText + " ") + currentAngle.ToString(), style);
    }

    void Update()
    {
        switch (compassActive)
        {
            case CompassTypes.None:
                break;
            case CompassTypes.Neddle:
                angle = CompassNeddle.GetCurrentAngle();
                break;
            case CompassTypes.Bubble:
                angle = CompassBubble.GetCurrentAngle();
                break;
            case CompassTypes.Linear:
                angle = CompassLinear.GetCurrentAngle();
                break;
            case CompassTypes.Standard:
                angle = Compass3D.GetCurrentAngle();
                break;
        }

        angle = ClampAngle(angle);
    }

    // Internal function to clamp, dealing with the 360ª problem.
    private float ClampAngle(float angle)
    {
        while ((angle < 0f) || (angle > 360f))
		{
            if (angle < 0f)
                angle = angle + 360f;
            else if (angle > 360f)
                angle = angle - 360f;
        }
        return angle;
    }

}