
using UnityEngine;
using System.Collections;

public class CatchStatusGUI : MonoBehaviour
{
	public bool showGUIStatus = true;
	public Rect GUIRect = new Rect(555, 22, 200, 120); // The rect the window is initially displayed at.

	private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label. 
    private CatchObject myCatchObj;
    private Crosshair crosshairScript;


	// Player Status keyboard detector, to show Status GUI. Pressing F6
	/*private void CheckInput()
	{
		if (InputManager.instance.f6Key.isDown)
			showGUIStatus = !showGUIStatus;
	}*/

    void Start()
    {
		myCatchObj = GetComponent<CatchObject>();
        crosshairScript = GetComponent<Crosshair>();
    }

	/*void Update()
	{
		CheckInput ();
	}*/

    // Show Player Status in screen.
    void OnGUI()
    {
        if (!showGUIStatus) { return; } // Security Sentence

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleLeft;
        }

        GUIRect = GUI.Window(2, GUIRect, DoMyWindowLaunchStatus, "");
    }

    public void DoMyWindowLaunchStatus(int windowID)
    {
        Rect MyRect0 = new Rect(65, 5, 150, 50);
        GUI.Label(MyRect0, "Player Interaction", style);
        // Interaction GUI.
        if (myCatchObj != null)
        {
            Rect MyRect1 = new Rect(10, 35, 200, 50);
            GUI.Label(MyRect1, "Interaction:", style);
            MyRect1 = new Rect(85, 35, 150, 50);
            if ((((!myCatchObj.IsCatching() && !myCatchObj.IsDragging()) && !myCatchObj.IsCatched()) && !myCatchObj.IsLaunching()) && !myCatchObj.IsDroping())
            {
                GUI.color = Color.green;
                GUI.Label(MyRect1, "No Interaction", style);
            }
            else
            {
                if (myCatchObj.IsCatching())
                {
                    GUI.color = Color.green;
                    GUI.Label(MyRect1, "Is Catching", style);
                }
                if (myCatchObj.IsDragging())
                {
                    GUI.color = Color.green;
                    GUI.Label(MyRect1, "Is Dragging", style);
                }
                if (myCatchObj.IsCatched())
                {
                    GUI.color = Color.blue;
                    GUI.Label(MyRect1, "Has Catched", style);
                }
                if (myCatchObj.IsLaunching())
                {
                    GUI.color = Color.red;
                    GUI.Label(MyRect1, "Is Launching", style);
                }
                if (myCatchObj.IsDroping())
                {
                    GUI.color = Color.red;
                    GUI.Label(MyRect1, "Is Dropping", style);
                }
            }

            // Name of the object being catched.
            GUI.color = Color.white;
            Rect MyRect2 = new Rect(10, 55, 200, 50);
            GUI.Label(MyRect2, "Object:", style);
            MyRect2 = new Rect(85, 70, 170, 50);
            if (myCatchObj.GetCatchedObject() != null)
            {
                GUI.color = Color.green;
                GUI.Label(MyRect2, myCatchObj.GetCatchedObject().name);
            }
            else
            {
                GUI.color = Color.green;
                GUI.Label(MyRect2, "None");
            }
        }

        if (crosshairScript != null)
        {
            if (crosshairScript.enabled && crosshairScript.drawCrosshair)
            {
                // Name of the object being catched.
                GUI.color = Color.white;
                Rect MyRect3 = new Rect(10, 75, 200, 50);
                GUI.Label(MyRect3, "Accuracy:", style);
                MyRect3 = new Rect(85, 90, 170, 50);

				GUI.color = (crosshairScript.GetCurrentAccuracy() <= 0.5f) ? Color.green : Color.green;
                GUI.Label(MyRect3, crosshairScript.GetCurrentAccuracy().ToString());
            }
        }

        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
   
}