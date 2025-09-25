// Based on the clever code of maydinunlu
// https://github.com/maydinunlu/virtual-joystick-unity/blob/master/VirtualJoystick/Assets/Scripts/VirtualJoystick.cs
//

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum JoystickType
{
    Movement = 0,
    Camera = 1
}

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    public JoystickType joystickType = JoystickType.Movement;

    public Image ImgBg;
    public Image ImgJoystick;

    [Space(10)]
    public float deadZone = 20;
    public Vector2 speed = Vector2.one;

    private KeyStatus _inputKeyHor;
    private KeyStatus _inputKeyVert;

    [Space(5)]
    public bool showDebug = false;

    [Header("Runtime Watcher")]
    [SerializeField]
    [Disable]
    private bool isBeingPressed;

    [SerializeField]
    [Disable]
    Vector2 pos;
    [SerializeField]
    [Disable]
    Vector2 posAux;

    private Vector2 _inputVector2D; // Vector2D to emulate the button isDown & isPressed events.
    [SerializeField]
    [Disable]
    private Vector3 _inputVector;
    public Vector3 InputVector
    {
        get
        {
            return _inputVector;
        }
    }



    void Start()
    {
        switch (joystickType)
        {
            case JoystickType.Movement:
                _inputKeyHor = InputManager.instance.horizontalKey;
                _inputKeyVert = InputManager.instance.verticalKey;
                break;
            case JoystickType.Camera:
                break;
        }
    }


    public void OnPointerDown(PointerEventData e)
    {
        OnDrag(e);
    }


    public void OnDrag(PointerEventData e)
    {
        //Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ImgBg.rectTransform,
                                                                    e.position,
                                                                    e.pressEventCamera,
                                                                    out pos))
        {

            // Get the relative position to the simage background size.
            posAux.x = pos.x / ImgBg.rectTransform.sizeDelta.x;
            posAux.y = pos.y / ImgBg.rectTransform.sizeDelta.y;

            if (Mathf.Abs(pos.x) <= deadZone)
                posAux.x = 0;
            if (Mathf.Abs(pos.y) <= deadZone)
                posAux.y = 0;

            // Smooth the joystick value using an speed multiplier.
            _inputVector = new Vector3(posAux.x * speed.x, 0, posAux.y * speed.y);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            // Draw the inner stick based on original click/touch positon
            pos.x = Mathf.Clamp(pos.x, -ImgBg.rectTransform.sizeDelta.x, ImgBg.rectTransform.sizeDelta.x) * 0.4f;
            pos.y = Mathf.Clamp(pos.y, -ImgBg.rectTransform.sizeDelta.y, ImgBg.rectTransform.sizeDelta.y) * 0.4f;

            ImgJoystick.rectTransform.anchoredPosition = pos;


            // Update (more like feeding) InputManager data (with our joystick data)
            switch (joystickType)
            {
                case JoystickType.Movement:
                    _inputVector2D.x = _inputVector.x;
                    _inputVector2D.y = _inputVector.z;
                    SetMovementDownState(_inputVector2D); // Set Button
                    DetectJoystickMovements();
                    break;
                case JoystickType.Camera:
                    DetectJoystickCamera();
                    break;
            }
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        ResetJoystick();

        switch (joystickType)
        {
            case JoystickType.Movement:
                SetUpState();
                ResetJoystickMovements();
                break;
            case JoystickType.Camera:
                ResetJoystickCamera();
                break;
        }
    }


    public void ResetJoystick()
    {
        _inputVector = Vector3.zero;
        ImgJoystick.rectTransform.anchoredPosition = Vector3.zero;
    }


    //=====================================================================================
    // Button emulation
    //=====================================================================================

    public virtual void Update()
    {
        if (!isBeingPressed) return;

        if (isBeingPressed && !InputManager.instance.IsButtonDown(_inputKeyHor))
        {
            InputManager.instance.SetButtonPressed(_inputKeyHor);
            if(showDebug) Debug.Log(joystickType.ToString() + " joystick is being pressed horizontally");
        }

        if (isBeingPressed && !InputManager.instance.IsButtonDown(_inputKeyVert))
        {
            InputManager.instance.SetButtonPressed(_inputKeyVert);
            if (showDebug) Debug.Log(joystickType.ToString() + " joystick is being pressed vertically");
        }
    }

    public virtual void SetMovementDownState(Vector2 _input)
    {
        if (_input.x != 0)
        {
            InputManager.instance.SetButtonDown(_inputKeyHor);
            isBeingPressed = true;
            StartCoroutine("_ResetDownFlag", _inputKeyHor);    // Let the button stay in DownState for 1 frame before going to press state.
            if (showDebug) Debug.Log(joystickType.ToString() + " joystick has been pressed down horizontally");
        }

        if (_input.y != 0)
        {
            InputManager.instance.SetButtonDown(_inputKeyVert);
            isBeingPressed = true;
            StartCoroutine("_ResetDownFlag", _inputKeyVert);    // Let the button stay in DownState for 1 frame before going to press state.
            if (showDebug) Debug.Log(joystickType.ToString() + " joystick has been pressed down vertically");
        }
    }

    IEnumerator _ResetDownFlag(KeyStatus _inputKey)
    {
        yield return new WaitForEndOfFrame();
        InputManager.instance.SetButtonPressed(_inputKey);
    }

    public virtual void SetUpState()
    {
        StartCoroutine(InputManager.instance.SetButtonUp(_inputKeyHor));
        StartCoroutine(InputManager.instance.SetButtonUp(_inputKeyVert));
        isBeingPressed = false;
        //Debug.Log(buttonName + " button is being released");
    }



    //===================================================================================
    //
    // Mouse like values for the horizontal and vertical values (Player movement)
    //
    //===================================================================================
    private void DetectJoystickMovements ()
    {
        InputManager.instance.HorizontalValue = _inputVector.x;
        InputManager.instance.VerticalValue = _inputVector.z;

        InputManager.instance.IsHorAxisActive = (InputManager.instance.HorizontalValue == 0) ? false : true;
        InputManager.instance.IsVertAxisActive = (InputManager.instance.VerticalValue == 0) ? false : true;
    }

    private void ResetJoystickMovements()
    {
        InputManager.instance.HorizontalValue = 0;
        InputManager.instance.VerticalValue = 0;

        InputManager.instance.IsHorAxisActive = false;
        InputManager.instance.IsVertAxisActive = false;
    }


    //===================================================================================
    //
    // Mouse like values for the x and y values (Player camera rotation / player point of ciew)
    //
    //===================================================================================
    private void DetectJoystickCamera()
    {
        InputManager.instance.MouseXValue = _inputVector.x;
        InputManager.instance.MouseYValue = _inputVector.z;

        InputManager.instance.IsAxisXActive = (InputManager.instance.MouseXValue == 0) ? false : true;
        InputManager.instance.IsAxisYActive = (InputManager.instance.MouseYValue == 0) ? false : true;
    }

    private void ResetJoystickCamera()
    {
        InputManager.instance.MouseXValue = 0;
        InputManager.instance.MouseYValue = 0;

        InputManager.instance.IsAxisXActive = false;
        InputManager.instance.IsAxisYActive = false;
    }



    //=====================================================================================
    // Button emulation
    //=====================================================================================

    /*public float Horizontal()
    {
        if (_inputVector.x != 0)
        {
            return _inputVector.x;
        }

        return Input.GetAxis("Horizontal");
    }

    public float Vertical()
    {
        if (_inputVector.z != 0)
        {
            return _inputVector.z;
        }

        return Input.GetAxis("Vertical");
    }*/

}