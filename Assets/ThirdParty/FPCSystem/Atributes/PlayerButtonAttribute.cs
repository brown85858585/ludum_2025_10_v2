// Examples:
// param1 (show sprite param): enable/disable/hidden
// param2 (alignement) left, right, center
//
// [PlayerButton(OnButtonClicked)]  == [PlayerButton(OnButtonClicked, "disable", "center")]
// [PlayerButton(OnButtonClicked, false)]
// [PlayerButton(OnButtonClicked, true, "left")]
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class PlayerButtonAttribute : PropertyAttribute
{

	#region Properties

	// is the sprite centered?
	public readonly string disable = "hidden";  // "enable", "disable", "hidden";
	public readonly string centered = "center";	// "left, "right", "center"
	public readonly int sizex = 64;
	public readonly int sizey = 64;

    // Button Part
    public static float kDefaultButtonWidth = 80;
    public static float kDefaultButtonHeight = 30;
    public readonly string MethodName;
    private float _buttonWidth = kDefaultButtonWidth;

    public float ButtonWidth
    {
        get { return _buttonWidth; }
        set { _buttonWidth = value; }
    }

    private float _buttonHeight = kDefaultButtonHeight;
    public float ButtonHeight
    {
        get { return _buttonHeight; }
        set { _buttonHeight = value; }
    }

    #endregion

    public PlayerButtonAttribute(string _methodName) : this (_methodName, "hidden", "center", 64, 64) { 	}

	public PlayerButtonAttribute(string _methodName, string _disable) :  this(_methodName, _disable, "center", 64, 64) { 	}

	public PlayerButtonAttribute(string _methodName, string _disable, string _centered) :  this(_methodName, _disable, _centered, 64, 64) { 	}

	public PlayerButtonAttribute(string _methodName, string _disable, string _centered, int _sizex, int _sizey)
	{
        this.MethodName = _methodName;
        this.disable = _disable;
		this.centered = _centered;
		this.sizex = _sizex;
		this.sizey = _sizey;
	}
}

