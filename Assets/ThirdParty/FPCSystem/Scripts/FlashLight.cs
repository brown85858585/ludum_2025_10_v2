
using UnityEngine.UI;
using UnityEngine;
using System.Collections;


[System.Serializable]
public class LightValues
{
    [Tooltip("The maximum time the light can be 'On' (after time is reached, the light will turn off).")]
    public float lightOnTime = 30; // Time the light can be on.

    [Tooltip("Light battery time recovery ratio. (when light is off).")]
	public float recoveryRatio = 0.5f; // Recovery speed of the battery bar.

    [Tooltip("Minimum value to turn of the light. Can be 'zero' if you want the light battery's gui to disappear.")]
	public float minOnValue = 5; // Minimun value to turn the light on.

    [Tooltip("Light's battery GUITexture that shows the light time in screen.")]
    public Slider LightGUI; // UI battery bar of the light

    [Tooltip("FadeIn the UI light battery bar when light is On.")]
	public bool isLightFadeable = true; // UI bar for diving and swiming.

    [Tooltip("Battery bar FadeIn/FadeOut speed.")]
	public float guiFadeSpeed = 2; // speed of the fading (how fast the alpha goes)

    [Tooltip("Time to wait to perform the fadeOut when the turns off the light.")]
	public float guiWaitTimeFadeout = 2; // time to wait before fading out.
}


public class FlashLight : MonoBehaviour
{
    [Tooltip("The object (scene's light gameobject) to activate/deactivate when the light key is pressed.")]
    public GameObject LigthObject;

    [Tooltip("Light configuration. NOTE: It uses a battery timer (showed in screen) to control the time the light will be 'On'. All the cofiguration parameters are based on that internal battery timer.")]
	public LightValues LightOptions = new LightValues();

    // Light private vars.
	private bool isLightOn = false;
    private float lighInternalTime = 0; // Internal time to know how many time the light is ON.
    private SliderFader lightScript; // the script that controls the battery bar fade (alpha)



	// public function to set the light On/Off.
	public void SetFlashLight(bool _value){ isLightOn = _value; LigthObject.SetActive(_value); }

    // Inicialize the light and the light bar in the UI.
    void Start()
    {
        lighInternalTime = LightOptions.lightOnTime;
        if (LightOptions.LightGUI)
        {
            lightScript = (SliderFader) LightOptions.LightGUI.GetComponent(typeof(SliderFader));
            LightOptions.LightGUI.maxValue = LightOptions.lightOnTime;
            LightOptions.LightGUI.value = lighInternalTime;
            if (LightOptions.isLightFadeable && lightScript)
                StartCoroutine(lightScript.StartFadeOut(15, 0)); // Make it invisible quickly (almost inmediatly)
        }
    }

    void Update()
    {
        // Detect what to do when pressing the light button in the keyboard. (Switch On/Off the light).
        if (InputManager.instance.flashLightKey.isDown)
        {
            if (!isLightOn)
            {
                if (lighInternalTime > LightOptions.minOnValue)
                {
                    SetFlashLight(true);
                    if (LightOptions.isLightFadeable && lightScript)
                        lightScript.StartFadeIn(LightOptions.guiFadeSpeed);
                }
            }
            else
            {
                SetFlashLight(false);
                if (LightOptions.isLightFadeable && lightScript)
                    StartCoroutine(lightScript.StartFadeOut(LightOptions.guiFadeSpeed, LightOptions.guiWaitTimeFadeout));
            }
        }

        // Light is On: keep track of time and update the battery bar.
        if (isLightOn)
        {
            lighInternalTime = lighInternalTime - Time.deltaTime;
            if (lighInternalTime <= 0)
            {
                SetFlashLight(false);
                lighInternalTime = 0;
            }

            if (lightScript)
                LightOptions.LightGUI.value = lighInternalTime;
        }
        else
        {
             //lightScript.SetCurrentValue(lighInternalTime);
             // Light is off: start the recovery time and update the battery bar.
            if (lighInternalTime < LightOptions.lightOnTime)
                lighInternalTime = lighInternalTime + (LightOptions.recoveryRatio * Time.deltaTime);
            else if (lighInternalTime >= LightOptions.lightOnTime)
                lighInternalTime = LightOptions.lightOnTime;

            if (lightScript)
                LightOptions.LightGUI.value = lighInternalTime;
        }
    }

}