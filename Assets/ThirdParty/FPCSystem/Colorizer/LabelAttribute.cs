// Property Drawer que permite añadir un label diferente al nombre de la variable y un tooltip para esa variable y color.
//
// Uso : 
// [Label("NameOfVar", "ToolTip", "Color", bool ColorizeAll)] // First value is needed.
//
// 
// [Label("Update Wheels Allways")] // only 1 parameter
// [Label("Update Wheels Allways", "Update the whells of  your car!!!!")] // 2 parameters
// [Label("Update Wheels Allways", "Update the whells of  your car!!!!", "Color.black")]	// 3 parameters
// [Label("Update Wheels Allways", "Update the whells of  your car!!!!", "Color.black", true)] // 4 parameters
//	public bool updateWheelsAllways = false;// Var to use the propertydrawer on.


using UnityEngine;


public class LabelAttribute : PropertyAttribute
{
	public readonly string text;
	public readonly string tooltip;
	public readonly string color;
	public readonly bool colorizeAll;
	
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public LabelAttribute(string text) : this(text, null, null, false)
	{
	}
	
	public LabelAttribute(string text, string tooltip) : this(text, tooltip, null, false)
	{
	}
	
	public LabelAttribute(string text, string tooltip, string color) : this(text, tooltip, color, false)
	{
	}
	
	public LabelAttribute(string text, string tooltip, string color, bool _all)
	{
		this.text = text;
		this.tooltip = tooltip;
		this.color = color;
		this.colorizeAll = _all;
	}
}
