// Property Drawer que permite añadir un label diferente al nombre de la variable y un tooltip para esa variable y color.
//
// Uso : 
// [Colorize("Color", bool ColorizeAll)] // All values are optionals
//
// 
// [Colorize] // Zero parameters
// [Colorize("Color.blue")] // 1 parameters
// [Colorize("Color.blue", true)]	// 2 parameters
//	public int MyValue = 1; // Var to use the propertydrawer on.
//
// Default values (Color == Red, ColorizeAll == false).


using UnityEngine;


public class ColorizeAttribute : PropertyAttribute
{
	public readonly string color;
	public readonly bool colorizeAll;
	
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	public ColorizeAttribute() : this(null, false)
	{
	}
	
	public ColorizeAttribute(string _color) : this(_color, false)
	{
	}
	
	public ColorizeAttribute(string _color, bool _all)
	{
		this.color = _color;
		this.colorizeAll = _all;
	}
	
}
