using UnityEngine;
using System.Collections;

[System.Serializable]
public class KeyStatus
{
	[Tooltip("Is the key pressed right now? ---> is the same as Input.GetButtonDown().")]
	public bool isDown;
	[Tooltip("Is the key being pressed? ---> is the same as Input.GetButton().")]
	public bool isPressed;
	[Tooltip("Is the key released right now? ---> is the same as Input.GetButtonUp().")]
	public bool isUp;
}


public class InputManager : Singletone <InputManager>
{
    public bool isActive = true;

    // Marcadores de  las pulsaciones de teclado
    [Space(10)]
    [Header("Regular Control Buttons Detection")]
    public bool IsHorAxisActive = false;
    public float HorizontalValue = 0; // Horizontal value (GetAxis) Could be used with a joystick

    [Space(5)]
    public bool IsVertAxisActive = false;
    public float VerticalValue = 0; // Vertical value (GetAxis) Could be used with a joystick

    [Space(10)]
	public KeyStatus horizontalKey = new KeyStatus();
	public KeyStatus verticalKey = new KeyStatus();
	public KeyStatus jumpKey = new KeyStatus();
	public KeyStatus crouchKey = new KeyStatus();
	public KeyStatus runKey = new KeyStatus();
	public KeyStatus proneKey = new KeyStatus();
	public KeyStatus pushKey = new KeyStatus();
	public KeyStatus flashLightKey = new KeyStatus();
	public KeyStatus flyKey = new KeyStatus();
	public KeyStatus rocketKey = new KeyStatus();
	public KeyStatus dayLightKey = new KeyStatus();

    // Marcadores de  las pulsaciones de teclado y movimientos y pulsaciones del raton.
    [Space(10)]
    [Header("Mouse Control Buttons and Mouse Movement Detectors")]
    public bool IsAxisXActive = false;
    public float MouseXValue = 0; // Mouse X value (GetAxis)

    [Space(5)]
    public bool IsAxisYActive = false;
    public float MouseYValue = 0; // Mouse Y value (GetAxis)

    [Space(10)]
	public KeyStatus fire1Key = new KeyStatus();
	public KeyStatus fire2Key = new KeyStatus();
	public KeyStatus fire3Key = new KeyStatus();

    // Marcadores de  las pulsaciones de teclado de Fxx (F1 - F12) 
    [Space(10)]
    [Header("Special F1-F12 key Buttons")]
	public KeyStatus f1Key = new KeyStatus();
	public KeyStatus f2Key = new KeyStatus();
	public KeyStatus f3Key = new KeyStatus();
	public KeyStatus f4Key = new KeyStatus();
	public KeyStatus f5Key = new KeyStatus();
	public KeyStatus f6Key = new KeyStatus();
	public KeyStatus f7Key = new KeyStatus();
	public KeyStatus f8Key = new KeyStatus();
	public KeyStatus f9Key = new KeyStatus();
	public KeyStatus f10Key = new KeyStatus();
	public KeyStatus f11Key = new KeyStatus();
	public KeyStatus f12Key = new KeyStatus();


    public void DisableInput()
    {
        ResetKeys ();
        HorizontalValue = 0;
        VerticalValue = 0;
        MouseXValue = 0;
        MouseYValue = 0;

        isActive = false;
    }

    public void EnableInput()
    {
        isActive = true;
    }

    public bool IsButtonDown(KeyStatus _keyStatus)
    {
        return _keyStatus.isDown;
    }

    public void SetButtonDown(KeyStatus _keyStatus)
    {
        _keyStatus.isDown = true;
        _keyStatus.isPressed = true;
        _keyStatus.isUp = false;
    }

    public void SetButtonPressed(KeyStatus _keyStatus)
    {
        _keyStatus.isDown = false;
        _keyStatus.isPressed = true;
        _keyStatus.isUp = false;
    }

    public IEnumerator SetButtonUp(KeyStatus _keyStatus)
    {
        _keyStatus.isDown = false;
        _keyStatus.isPressed = false;
        _keyStatus.isUp = true;
        yield return new WaitForSeconds(0.25f);
        SetButtonUntouched(_keyStatus);
    }

    public void SetButtonUntouched(KeyStatus _keyStatus)
    {
        _keyStatus.isDown = false;
        _keyStatus.isPressed = false;
        _keyStatus.isUp = false;
    }

    public bool IsHorVerAxisMoving(){ return IsHorAxisActive || IsVertAxisActive; }

    public bool IsMouseMoving(){ return IsAxisXActive || IsAxisYActive; }

    public bool IsFowardButtonPressed() { return verticalKey.isDown || verticalKey.isPressed; }


    // Force by code the forward button to be active (the walker will walk forward on LowLeges).
    [Space(10)]
    [SerializeField]
    [Disable]
    private bool isForceForwardEnabled = false;
    public void SetForceForwardButton(bool _force)
    {
        isForceForwardEnabled = _force;
        ForceFowardButtonPressed();
    }

    private void ForceFowardButtonPressed()
    {
        if (verticalKey.isDown != isForceForwardEnabled)
        {
            verticalKey.isDown = isForceForwardEnabled;
            verticalKey.isPressed = isForceForwardEnabled;
            IsVertAxisActive = isForceForwardEnabled;
            VerticalValue = isForceForwardEnabled ? 1f : 0f;
        }
    }

    // Force by code the run button to be active.
    [SerializeField]
    [Disable]
    private bool isForceRunEnabled = false;
    public void SetForceRunButton(bool _force)
    {
        isForceRunEnabled = _force;
        ForceRunButtonPressed();
    }

    private void ForceRunButtonPressed()
    {
        if (runKey.isDown != isForceRunEnabled)
        {
            runKey.isDown = isForceRunEnabled;
            runKey.isPressed = isForceRunEnabled;
        }
    }


    private void Start()
    {
        #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        DisableInput();
        #endif
    }

    void Update()
    {
        if (!isActive) return;

        if (isForceForwardEnabled)
            ForceFowardButtonPressed();

        if (isForceRunEnabled)
            ForceRunButtonPressed();

        DetectKeys();
        DetectHorMovements();
        if (!isForceForwardEnabled)
            DetectVertMovements();
        DetectMouseMovements();
    }

    private void ResetKeys()
    {
        SetButtonUntouched(horizontalKey);
        SetButtonUntouched(verticalKey);
        SetButtonUntouched(jumpKey);
        SetButtonUntouched(crouchKey);
        SetButtonUntouched(runKey);
        SetButtonUntouched(proneKey);
        SetButtonUntouched(pushKey);
        SetButtonUntouched(flashLightKey);
        SetButtonUntouched(flyKey);
        SetButtonUntouched(rocketKey);
        SetButtonUntouched(dayLightKey);
        SetButtonUntouched(fire1Key);
        SetButtonUntouched(fire2Key);
        SetButtonUntouched(fire3Key);
        SetButtonUntouched(f1Key);
        SetButtonUntouched(f2Key);
        SetButtonUntouched(f3Key);
        SetButtonUntouched(f4Key);
        SetButtonUntouched(f5Key);
        SetButtonUntouched(f6Key);
        SetButtonUntouched(f7Key);
        SetButtonUntouched(f8Key);
        SetButtonUntouched(f9Key);
        SetButtonUntouched(f10Key);
        SetButtonUntouched(f11Key);
        SetButtonUntouched(f12Key);
    }

    private void DetectKeys()
    {
        DetectKeyInput("Horizontal", horizontalKey);
        if (!isForceForwardEnabled)
            DetectKeyInput("Vertical", verticalKey);
        DetectKeyInput("Jump", jumpKey);
        DetectKeyInput("Crouch", crouchKey);
        if (!isForceRunEnabled)
            DetectKeyInput("Run", runKey);
        DetectKeyInput("Prone", proneKey);
        DetectKeyInput("Push", pushKey);
        DetectKeyInput("FlashLight", flashLightKey);
        DetectKeyInput("Fly", flyKey);
        DetectKeyInput("Rocket", rocketKey);
        DetectKeyInput("DayLight", dayLightKey);
        DetectKeyInput("Fire1", fire1Key);
        DetectKeyInput("Fire2", fire2Key);
        DetectKeyInput("Fire3", fire3Key);
        DetectKeyRaw(KeyCode.F1, f1Key);
        DetectKeyRaw(KeyCode.F2, f2Key);
        DetectKeyRaw(KeyCode.F3, f3Key);
        DetectKeyRaw(KeyCode.F4, f4Key);
        DetectKeyRaw(KeyCode.F5, f5Key);
        DetectKeyRaw(KeyCode.F6, f6Key);
        DetectKeyRaw(KeyCode.F7, f7Key);
        DetectKeyRaw(KeyCode.F8, f8Key);
        DetectKeyRaw(KeyCode.F9, f9Key);
        DetectKeyRaw(KeyCode.F10, f10Key);
        DetectKeyRaw(KeyCode.F11, f11Key);
        DetectKeyRaw(KeyCode.F12, f12Key);

        HorizontalValue = 0f;
        if (!isForceForwardEnabled)
            VerticalValue = 0f;

        IsHorAxisActive = false;
        if (!isForceForwardEnabled)
            IsVertAxisActive = false;

        MouseXValue = 0f;
        MouseYValue = 0f;

        IsAxisXActive = false;
        IsAxisYActive = false;
    }

    private void DetectKeyInput(string _axisName, KeyStatus _keyStatus)
    {
        if (Input.GetButtonDown(_axisName))
        {
            SetButtonDown (_keyStatus);
        }
        else if (Input.GetButton(_axisName))
        {
            SetButtonPressed (_keyStatus);
        }
        else if (Input.GetButtonUp(_axisName))
        {
            StartCoroutine("SetButtonUp", _keyStatus);
        }
        else
        {
            SetButtonUntouched(_keyStatus);
        }
    }

    private void DetectKeyRaw(KeyCode _keyCode, KeyStatus _keyStatus)
    {
        if (Input.GetKeyDown(_keyCode))
        {
            SetButtonDown(_keyStatus);
        }
        else if (Input.GetKey(_keyCode))
        {
            SetButtonPressed(_keyStatus);
        }
        else if (Input.GetKeyUp(_keyCode))
        {
            StartCoroutine("SetButtonUp", _keyStatus);
        }
        else
        {
            SetButtonUntouched(_keyStatus);
        }
    }

    
    //===================================================================================
    // Detect the horizontal/vertical axis values.
    // Can be down using regular keyboard or an in screen joystick
    private void DetectHorMovements()
    {
        HorizontalValue = Input.GetAxis("Horizontal");

		IsHorAxisActive = (HorizontalValue == 0) ? false : true;
    }

    private void DetectVertMovements()
    {
        VerticalValue = Input.GetAxis("Vertical");

        IsVertAxisActive = (VerticalValue == 0) ? false : true;
    }

    // Detect the mouse axis.
    private void DetectMouseMovements()
    {
        MouseXValue = Input.GetAxis("Mouse X");
        MouseYValue = Input.GetAxis("Mouse Y");

		IsAxisXActive = (MouseXValue == 0) ? false : true;
		IsAxisYActive = (MouseYValue == 0) ? false : true;
    }

}