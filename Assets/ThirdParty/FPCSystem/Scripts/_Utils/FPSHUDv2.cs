// Attach this to any object to make a frames/second indicator.
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// corstartRect overall FPS even if the interval renders something like
// 5.5 frames.

// Original code from Unity's wiki.
// http://wiki.unity3d.com/index.php?title=FramesPerSecond
// Changed to work in a GUIText too. 02/02/2013
// Converted to C# 06/05/2013.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu( "Utilities/FPSHUDv2")]
public class FPSHUDv2 : MonoBehaviour {

    //public Rect startRect = new Rect( 10, 10, 75, 50 ); // The rect the window is initially displayed at.
    public bool updateColor = true; // Do you want the color to change if the FPS gets low
                                    //public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
    public float frequency = 0.5f; // The update frequency of the fps
    public int nbDecimal = 1; // How many decimal do you want to display

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float fps = 0;
    private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private string sFPS = string.Empty; // The fps formatted into a string.
                                        //private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.

    private Text m_Text;
    private int _decimal = 1;

    // Use this for initialization
    IEnumerator Start()
    {
        accum = 0f;
        frames = 1;
        m_Text = GetComponent<Text>();
        m_Text.color = Color.black;
        color = Color.green;
        _decimal = Mathf.Clamp(nbDecimal, 0, 10);

        // Infinite loop executed every "frenquency" secondes.
        while (Application.isPlaying)
        {
            // Update the FPS
            //var fps : float = accum / (frames > 0 ? frame : 1);
            fps = accum / frames;
            sFPS = fps.ToString("f" + _decimal.ToString());

            //Update the color
            if (updateColor)
                color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);

            accum = 0;
            frames = 0;

            yield return new WaitForSeconds(frequency);
        }
    }

    // Update is called once per frame
    void Update()
    {
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (updateColor)
            m_Text.color = color;
        m_Text.text = sFPS;// + " FPS";
    }

    /*void OnGUI(){
		GUI.color = Color.white;
	}*/

    /*void OnGUI(){
		if(!GetComponent<GUIText>()){
		    // Copy the default label skin, change the color and the alignement
		    if( style == null ){
		        style = new GUIStyle( GUI.skin.label );
		        style.normal.textColor = Color.white;
		        style.alignment = TextAnchor.MiddleCenter;
		    }
		 
		    GUI.color = updateColor ? color : Color.white;
		    startRect = GUI.Window(2, startRect, DoMyWindowFPS, "");
		}
	}
	 
	void DoMyWindowFPS( int windowID ){
	    GUI.Label( new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style );
	    if( allowDrag ) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
	}*/

}
