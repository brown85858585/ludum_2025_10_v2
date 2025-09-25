// Property Drawer que permite añadir un label en el inspector a modo de linea sin estar
// relacionado con ninguna variable util. Se puede especificar un tooltip y un color.
// 
//
// Ejemplo:
// [Comment("Nombre","ToolTip", "Color", "Anchura")] // First var is needed.
// Default color is Red.
// Default width is 0;
//
// [Comment("TextToShow")]
// [Comment("TextToShow","Tooltip when MouseOver")]
// [Comment("TextToShow","Tooltip when MouseOver", "Color.magenta")]
// [Comment("TextToShow","Tooltip when MouseOver", "Color.magenta", 20)]
// public int Comment1;


using UnityEngine;

public class CommentAttribute : PropertyAttribute {
	public readonly string comment;
	public readonly string tooltip;
	public readonly string color;
	public readonly int width;
	
	
	public CommentAttribute(string comment) : this(comment, null, null, 0)
	{
	}
	
	public CommentAttribute(string comment, string tooltip) : this(comment, tooltip, null, 0)
	{
	}
	
	public CommentAttribute( string comment, string tooltip, string color) : this(comment, tooltip, color, 0)
	{
	}
	
	public CommentAttribute( string comment, string tooltip, string color, int width) {
		this.tooltip = tooltip;
		this.comment = comment;
		this.color = color;
		this.width = width;
	}
}

