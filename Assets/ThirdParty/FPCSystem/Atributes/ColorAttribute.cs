using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ColorAttribute : PropertyAttribute {
	#region Properties

	// The color of the line
	public readonly string col = "white";

	#endregion

	public ColorAttribute (string col)
	{
		this.col = col;
	}

	public ColorAttribute () { }

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ColorAttribute))]
public class ColorDrawer : PropertyDrawer
{
	ColorAttribute color { get { return ((ColorAttribute) attribute); } }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.normal.textColor = AssignColor(color.col.ToLower());

		//EditorGUIUtility.LookLikeControls();
		//if(colorizeAttribute.colorizeAll)
		SetGUIColor(labelStyle.normal.textColor);

		//EditorGUI.LabelField(position, label);
		EditorGUI.PropertyField(position, property, label);
		GUI.color = Color.white;
    }

	private void SetGUIColor(Color _color){
		if(_color != Color.black){
			GUI.color = _color;
			GUI.contentColor = _color;
			//GUI.backgroundColor = _color;
		}
	}


	private Color AssignColor(string _color){
		Color finalColor = Color.red;

		switch(_color){
		case "black":
			finalColor = Color.black;
			break;
		case "blue":
			finalColor = Color.blue;
			break;
		case "clear":
			finalColor = Color.clear;
			break;
		case "cyan":
			finalColor = Color.cyan;
			break;
		case "gray":
			finalColor = Color.gray;
			break;
		case "green":
			finalColor = Color.green;
			break;
		case "grey":
			finalColor = Color.grey;
			break;
		case "magenta":
			finalColor = Color.magenta;
			break;
		case "red":
			finalColor = Color.red;
			break;
		case "white":
			finalColor = Color.white;
			break;
		case "yellow":
			finalColor = Color.yellow;
			break;
		default:
			finalColor = Color.red;
			break;
		}

		return finalColor;
	}
}

#endif