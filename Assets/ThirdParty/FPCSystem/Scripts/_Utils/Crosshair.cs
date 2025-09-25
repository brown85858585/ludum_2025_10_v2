using UnityEngine;
using System.Collections;

// Original script from OrangeLightning user.
// http://answers.unity3d.com/questions/203653/smart-crosshair.html
// Basic crosshair properties.



public enum crosshairType
{
    Normal = 0,
    Rotated = 1,
    OnlyCircled = 2,
    NormalAndCircled = 3,
    RotatedAndCircled = 4
}

[System.Serializable]
public class spreading
{
    [Tooltip("Crosshair default spread (when not moving, jumping or firing).")]
	public float spread = 10f; //Adjust this for a bigger or smaller crosshair

    [Tooltip("Crosshair maximum spread that can be reached at any time.")]
	public float maxSpread = 60f;

    [HideInInspector]
	public float minSpread = 10f;

}
 // Actual spread values after applying accuracy. Weapon fire spreading values
[System.Serializable]
public class fireSpreading
{
    [Tooltip("Crosshair spread will expand at this rate when firing.")]
	public float spreadPerSecond = 30f; // CrossAir size change rate depending on time firing a weapon.

    [Tooltip("Crosshair spread will return to the default values at this rate when fire stops.")]
	public float decreasePerSecond = 25f;
}

// Weapon movement spreading
// current spread when walking or firing.
[System.Serializable]
public class moveSpreading
{
    [Tooltip("Crosshair spread expand rate depending on pl current velocity.")]
	public float velocitySpread = 0.8f; // CrossAir size change rate depending on pl current velocity.

    [Tooltip("Crosshair maximum spread when walking.")]
	public float maxWalkSpread = 30f; // Maximun spread value when walking

    [Tooltip("Crosshair maximum spread when running.")]
	public float maxRunSpread = 40f; // Maximun spread value when running

    [Tooltip("Crosshair spread will return to the default values at this rate when Pro stops.")]
	public float decreasePerSecond = 20;
}

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Activation")]
    [Tooltip("Draw the crossair GUI on screen?.")]
    public bool drawCrosshair = true;
    [Tooltip("Detect the F7 key to change the crossair image.")]
	public bool F7KeyEnabled = true;

    [Header("Crosshair Visual Configuration")]
    [Tooltip("Crosshair Type to draw.")]
	public crosshairType CrosshairType = crosshairType.Normal;
    [Tooltip("Change the drawn sphere depending on the accuracy. Accuracy can get lower (less accuracy means greater visual sphere) when firing.")]
	public bool updateSphereAccuracy = true;
    [Tooltip("Crosshair 'normal' state color (when no enemy has been detected).")]
	public Color crosshairColor  = Color.white;
    [Tooltip("Crosshair line width used to draw it. (Adjust it in runtime mode to see its effects).")]
	public float lineWidth = 2;
    [Tooltip("Crosshair line lenght used to draw it. (Adjust it in runtime mode to see its effects).")]
	public float lineLength = 15;

    private Texture2D tex;
    private GUIStyle lineStyle;
    private float angle;

    [Space(5)]
    [Tooltip("Crosshair 'detect' state color (when some enemy has been detected).")]
	public Color crosshairDetectColor = Color.red;
    [Tooltip("Gameobjects layers that will fire the detect state to true (changing the crossair color if that happens).")]
    public string[] detectLayers;
    [Tooltip("Maximum distance some object/enemy can be in order to be detected by the crosshair. If it is beyond that maximum distance it will be ignored.")]
	public float visibilityDist = 100;

    private bool isDetectingTarget;

    private Texture2D detectTex;

    [Header("Crosshair Main Configuration")]
    [Tooltip("Crosshair default accuracy (when not moving, jumping or firing).")]
	public float accuracy = 0.1f;
    private float currentAccuracy;

    [Tooltip("Crosshair default spread values. The spread will be represented in screen by getting close/far the crosshair lines.")]
    public spreading spread;
	private spreading currentSpread = new spreading();
    private float actualSpread;

    [Tooltip("Crosshair firing spread modifying values. These values can be changed by weapon because each weapon shound have their own spread and accuracy values.")]
    public fireSpreading fireSpread;
    [Tooltip("Crosshair moving spread modifying values.")]
    public moveSpreading moveSpread;

    private bool wasFiring; // Detect when was firing (or moving) to apply a descrease value or the other one (there are two).
	private float startTime = 0.1f; // wait some time to start calculate the currentSpread, the initial spread is too big.

    private CharacterController controller; // Needed because the crossair changes depending on player velocity and status.
	private Status statusSrc;
	private Core coreScr;



    // It's caused by pl that returns a insane velocity in the first miliseconds.
    // enable/disable the crossair in screen
    public void EnableCrossAir(bool _enabled)
    {
        drawCrosshair = _enabled;
    }

    // return the current accuracy given the current visual status of the CrossAir.
    public float GetCurrentAccuracy()
    {
        return currentAccuracy;
    }

   void Awake()
    {
		statusSrc = GameObject.FindWithTag("Player").GetComponent<Status>();
		coreScr = statusSrc.GetComponent<Core>();
		controller = statusSrc.gameObject.GetComponent<CharacterController>();

		tex = new Texture2D(1, 1);
		SetColor(tex, crosshairColor); //Set color of default crossair
		detectTex = new Texture2D(1, 1);
		SetColor(detectTex, crosshairDetectColor); //Set color of detected crossair
		lineStyle = new GUIStyle();
		lineStyle.normal.background = tex;
		spread.minSpread = spread.spread; // Set the minspread to spread value
		actualSpread = spread.minSpread; // inicialize the actualSpead (the spread changes in runtime when moving or firing)
    }

    void Update()
    {
        if (!drawCrosshair) { return; }
        if (Pause.instance.IsPaused()) { return; }

        //RaycastHit hit = default(RaycastHit);

        // ========================================================================================
        // TODO.
        // USED FOR DEMO PURPOSES. Can be commented at any time.
        //
        // Change the crossair shape by pressing F7
        // 
        if (InputManager.instance.f7Key.isDown && F7KeyEnabled)
        {
			CrosshairType = (CrosshairType < (crosshairType) 4) ? (CrosshairType + 1) : (crosshairType) 0;
        }

        
        //
        // ========================================================================================
        // Get the minimun aim visual accuracy based on the accuracy parameter.
        // if defines how much closed are the crossair lines betwen them.
       currentSpread.spread = spread.spread + (spread.spread * accuracy);
       currentSpread.spread = Mathf.Clamp(currentSpread.spread, spread.minSpread, spread.maxSpread);
       currentSpread.minSpread = currentSpread.spread;
       currentSpread.maxSpread = spread.maxSpread - (currentSpread.minSpread * accuracy);
       currentSpread.maxSpread = Mathf.Clamp(currentSpread.maxSpread, spread.minSpread, spread.maxSpread);
        if (startTime < Time.timeSinceLevelLoad)
        {
            // Increment the spread when moving
            if (!statusSrc.isStop)
            {
                actualSpread = actualSpread + (statusSrc.isInGround ? (controller.velocity.magnitude * (1 - moveSpread.velocitySpread)) * 0.1f : currentSpread.maxSpread);
                // dont allow to increase the spread beyond walk or run spread values.
                if (!statusSrc.isJumping)
                {
                    if (statusSrc.isWalking && actualSpread > moveSpread.maxWalkSpread)
                    {
                        actualSpread = Mathf.Lerp(actualSpread, moveSpread.maxWalkSpread, 10 * Time.deltaTime);
                    }
                    else
                    {
                        if (statusSrc.isRunning && actualSpread > moveSpread.maxRunSpread)
                        {
                            actualSpread = Mathf.Lerp(actualSpread, moveSpread.maxRunSpread, 10 * Time.deltaTime);
                        }
                    }
                }
                wasFiring = false;
            }
            else
            {
                if (!wasFiring)
                {
                    actualSpread = actualSpread - (moveSpread.decreasePerSecond * Time.deltaTime); //Decrement the spread to minimun.
                }
            }
        }

        //Increment the spread when firing
        if (InputManager.instance.fire1Key.isPressed)
        {
            actualSpread = actualSpread + (fireSpread.spreadPerSecond * Time.deltaTime);
            wasFiring = true;
        }
        else
        {
            Fire();
            if (wasFiring)
            {
                actualSpread = actualSpread - (moveSpread.decreasePerSecond * Time.deltaTime); //Decrement the spread to minimun.
            }
        }

        // Clamp the value betwen his minimun and maximun visual accuracy values.
        actualSpread = Mathf.Clamp(actualSpread, currentSpread.minSpread, currentSpread.maxSpread);
        // Get current acuracy based on the actual size of the crossAir.
        currentAccuracy = accuracy + Mathf.InverseLerp(currentSpread.minSpread, currentSpread.maxSpread, actualSpread);
        currentAccuracy = Mathf.Clamp(currentAccuracy, accuracy, 1f);
      
		// detectLayers (an Enemy for example) Detection using raycast from the camera towards a given distance (visibility dist).
        isDetectingTarget = false;

		// Activate the fromt camera sensor cast detector
		coreScr.forwardSensorOptions.useSensorDetector = true;

		// if the the camera sensor detector has something
		if(coreScr.forwardCameraSensorOptions.sensorDistance != 0 && coreScr.forwardCameraSensorOptions.sensorDistance < visibilityDist)
		{
			foreach (string MyTag in detectLayers)
			{
				if (coreScr.forwardCameraSensorOptions.sensorHitTag.Contains(MyTag))
				{
					isDetectingTarget = true;
				}
			}
		}

        //  NOt needed anymore. Now we have pl sensors to detect targets.
        /*Vector3 fwd = Camera.main.transform.TransformDirection(Vector3.forward);
        int layerMask = 0;
        int i = 0;
        while (i < detectLayers.Length)
        {
            layerMask = layerMask | (1 << LayerMask.NameToLayer(detectLayers[i]));
            i++;
        }
        
        if (Physics.Raycast(Camera.main.transform.position, fwd, out hit, visibilityDist, layerMask))
        {
            foreach (string MyTag in detectLayers)
            {
                if (hit.transform.tag == MyTag)
                {
                    isDetectingTarget = true;
                }
            }
        }*/
    }

    // Draw the crossair usind the spread parameter to open/close the crossair (visual accuracy).
    void OnGUI()
    {
        if (!drawCrosshair) { return; }
        if (Pause.instance.IsPaused()) { return; }

        lineStyle.normal.background = isDetectingTarget ? detectTex : tex;
        Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        // Modify the angle if the crosshair is rotated.

		angle = (CrosshairType == crosshairType.Rotated || CrosshairType == crosshairType.RotatedAndCircled) ? 45 : 0;

        // Draw the lines of the crosshair if that crosshair-mode has been selected.
        if (CrosshairType != crosshairType.OnlyCircled)
        {
            Matrix4x4 matrixBackup = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, centerPoint);
            GUI.Box(new Rect(centerPoint.x, (centerPoint.y - lineLength) - actualSpread, lineWidth, lineLength), GUIContent.none, lineStyle);
            GUI.Box(new Rect(centerPoint.x, centerPoint.y + actualSpread, lineWidth, lineLength), GUIContent.none, lineStyle);
            GUI.Box(new Rect(centerPoint.x + actualSpread, centerPoint.y, lineLength, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect((centerPoint.x - actualSpread) - lineLength, centerPoint.y, lineLength, lineWidth), GUIContent.none, lineStyle);
            GUI.matrix = matrixBackup;
        }

        // Draw a circle of the crosshair if that crosshair-mode has been selected.
        if (((CrosshairType == crosshairType.OnlyCircled) || (CrosshairType == crosshairType.NormalAndCircled)) || (CrosshairType == crosshairType.RotatedAndCircled))
        {
			int cx = Mathf.FloorToInt(centerPoint.x);
			int cy = Mathf.FloorToInt(centerPoint.y);
            if (updateSphereAccuracy)
                Circle(cx, cy, (int) actualSpread);
            else
                Circle(cx, cy, (int) currentSpread.minSpread);
        }
    }

    //Carry out your normal shooting and stuff
    public virtual void Fire()
    {
        return;
    }

    //Applies color to the crosshair
    public virtual void SetColor(Texture2D myTexture, Color myColor)
    {
        int y = 0;
        while (y < myTexture.height)
        {
            int x = 0;
            while (x < myTexture.width)
            {
                myTexture.SetPixel(x, y, myColor);
                ++x;
            }
            ++y;
        }
        myTexture.Apply();
    }

    // Original script: Eric Haines (Eric5h5) 
    // http://wiki.unity3d.com/index.php?title=TextureDrawCircle
    // Improved to use a GUI.Box instead of a texture2D.
    public virtual void Circle(int cx, int cy, int r)
    {
        int y = r;
        int d = (1 / 4) - r;
        float end = Mathf.Ceil(r / Mathf.Sqrt(2));
        int x = 0;
        while (x <= end)
        {
            GUI.Box(new Rect(cx + x, cy + y, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx + x, cy - y, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx - x, cy + y, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx - x, cy - y, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx + y, cy + x, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx - y, cy + x, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx + y, cy - x, lineWidth, lineWidth), GUIContent.none, lineStyle);
            GUI.Box(new Rect(cx - y, cy - x, lineWidth, lineWidth), GUIContent.none, lineStyle);
            d = d + ((2 * x) + 1);
            if (d > 0)
            {
                d = d + (2 - (2 * y--));
            }
            x++;
        }
    }

}