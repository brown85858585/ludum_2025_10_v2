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

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomPropertyDrawer(typeof(CommentAttribute))]
public class CommentDrawer : PropertyDrawer {
	int textHeight = 25;
	
	CommentAttribute commentAttribute { get { return (CommentAttribute)attribute; } }
	
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return textHeight + commentAttribute.width;
	}
	
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		Rect newPosition = position;
		//newPosition.y += commentAttribute.width;
		/*if(textHeight == 25)
			textHeight += commentAttribute.width;*/
		EditorGUI.LabelField(newPosition,new GUIContent(" "));
		newPosition.y += commentAttribute.width;
		
		GUIStyle labelStyle = new GUIStyle();
		switch(commentAttribute.color){
			case "Color.black":
				labelStyle.normal.textColor = Color.black;
				break;
			case "Color.blue":
				labelStyle.normal.textColor = Color.blue;
				break;
			case "Color.clear":
				labelStyle.normal.textColor = Color.clear;
				break;
			case "Color.cyan":
				labelStyle.normal.textColor = Color.cyan;
				break;
			case "Color.gray":
				labelStyle.normal.textColor = Color.gray;
				break;
			case "Color.green":
				labelStyle.normal.textColor = Color.green;
				break;
			case "Color.grey":
				labelStyle.normal.textColor = Color.grey;
				break;
			case "Color.magenta":
				labelStyle.normal.textColor = Color.magenta;
				break;
			case "Color.red":
				labelStyle.normal.textColor = Color.red;
				break;
			case "Color.white":
				labelStyle.normal.textColor = Color.white;
				break;
			case "Color.yellow":
				labelStyle.normal.textColor = Color.yellow;
				break;
			default:
				labelStyle.normal.textColor = Color.black;
				break;
		}

		EditorGUI.LabelField(newPosition,new GUIContent(commentAttribute.comment,commentAttribute.tooltip), labelStyle);
	}
}

#endif