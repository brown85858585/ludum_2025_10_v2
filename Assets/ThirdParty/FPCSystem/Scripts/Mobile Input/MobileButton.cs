using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public partial class MobileButton : MonoBehaviour
{
    public string buttonName;

    [Space(5)]
    [Tooltip("Is this button work for an sustained key? (By default run/walk,crouch and prone are a Non sustained key, Jump is a sustained key (you'll jump oly when pressing the jump key))")]
    public bool isSustained = true;    // Is this button sustained (in mobiles, Run, Crouch & Prone can be nonSustained). This setting will be override by Core->mobileSustainedOptions.
    [ShowWhen("isSustained", ShowWhenAttribute.Condition.Equals, false)]
    public Color onColor = Color.blue;     // Buttons color when the button is 'On' (only works for non-sustained buttons)
    [ShowWhen("isSustained", ShowWhenAttribute.Condition.Equals, false)]
    public Color offColor = Color.gray;    // Buttons color when the button is 'On' (only works for non-sustained buttons)

    [Space(10)]
    [SerializeField]
    [Disable]
    private KeyStatus _inputKey;
    [SerializeField]
    [Disable]
    private bool isBeingPressed;
    [SerializeField]
    [Disable]
    private bool isButtonOn;        // is a non sustained button being activated (a nonsustained button acts like a switch (On/Off))

    private Button myButton;
    private Image myImage;

    private GameObject MyObj;
    private Status statusSrc;
    private Core coreSrc;


    public bool IsButtonOn() { return isButtonOn;  }

    public virtual void Start()
    {

        MyObj = GameObject.FindWithTag("Player"); // other way: GameObject.Find("Player");
        if (MyObj == null)
        {
            Debug.LogError("SoundPlayer -> Player NOT Found!");
        }
        statusSrc = MyObj.GetComponent<Status>();
        coreSrc = statusSrc.GetCoreScr();


        // Change the sustained configuration of this button depending on the settings in pl regarding mobile buttons.
        switch (buttonName)
        {
            case "Run":
                isSustained = coreSrc.mobileSustainedOptions.sprintSustained;
                break;
            case "Crouch":
                isSustained = coreSrc.mobileSustainedOptions.crouchSustained;
                break;
            case "Prone":
                isSustained = coreSrc.mobileSustainedOptions.proneSustained;
                break;
            default:
                break;
        }


        myButton = GetComponent<Button>();
        myImage = GetComponent<Image>();
        isButtonOn = false;
        ChangeSustainedButtonColor();

        switch (buttonName)
        {
            case "Jump":
                _inputKey = InputManager.instance.jumpKey;
                break;
            case "Crouch":
                _inputKey = InputManager.instance.crouchKey;
                break;
            case "Run":
                _inputKey = InputManager.instance.runKey;
                break;
            case "Prone":
                _inputKey = InputManager.instance.proneKey;
                break;
            case "Push":
                _inputKey = InputManager.instance.pushKey;
                break;
            case "FlashLight":
                _inputKey = InputManager.instance.flashLightKey;
                break;
            case "Fly":
                _inputKey = InputManager.instance.flyKey;
                break;
            case "Rocket":
                _inputKey = InputManager.instance.rocketKey;
                break;
            case "DayLight":
                _inputKey = InputManager.instance.dayLightKey;
                break;
            case "Fire1":
                _inputKey = InputManager.instance.fire1Key;
                break;
            case "Fire2":
                _inputKey = InputManager.instance.fire2Key;
                break;
            case "Fire3":
                _inputKey = InputManager.instance.fire3Key;
                break;
            default:
                _inputKey = InputManager.instance.jumpKey;
                break;
        }
    }

    public virtual void Update()
    {

        // Check crouch/prone buttons transitions (player can press crouch and prone, so the buttons can malfunction if not checked).
        // crouch/prone button have to exclude each other. if you are crouched, prone has to be disabled even if it is a nonsustained button.
        if (!isSustained)
        {
            bool isChanged = false;
            switch (buttonName)
            {
                case "Run":
                    break;
                case "Crouch":
                    isButtonOn = statusSrc.isCrouched;
                    isChanged = true;
                    break;
                case "Prone":
                    isButtonOn = statusSrc.isProned;
                    isChanged = true;
                    break;
                default:
                    break;
            }

            // if the button status change, we update its color in screen.
            if (isChanged)
            {
                ChangeSustainedButtonColor();
            }
        }

        if (isBeingPressed && !InputManager.instance.IsButtonDown(_inputKey))
        {
            InputManager.instance.SetButtonPressed(_inputKey);
            //Debug.Log(buttonName + " button is being pressed");
        }
    }

    public virtual void SetDownState()
    {
        if (!isBeingPressed)
        {
            InputManager.instance.SetButtonDown(_inputKey);
            isBeingPressed = true;
            isButtonOn = !isButtonOn;
            StartCoroutine("_ResetDownFlag", _inputKey);    // Let the button stay in DownState for 1 frame before going to press state.
            //Debug.Log(buttonName + " button has been pressed down");
        }
    }

    IEnumerator _ResetDownFlag(KeyStatus _inputKey)
    {
        yield return new WaitForEndOfFrame();
        InputManager.instance.SetButtonPressed(_inputKey);
    }

    public virtual void SetUpState()
    {
        StartCoroutine(InputManager.instance.SetButtonUp(_inputKey));
        isBeingPressed = false;
        ChangeSustainedButtonColor();
        //Debug.Log(buttonName + " button is being released");
    }

    private void ChangeSustainedButtonColor ()
    {
        if (isSustained) return;    // if the button is sustained then do nothing.
        myImage.color = isButtonOn ? onColor : offColor;
    }
}