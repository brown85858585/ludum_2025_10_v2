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

#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;



[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelDrawer : PropertyDrawer
{
	private static Type _editorType = null;
	private static MethodInfo _layerMaskFieldMethod = null;
	//private static MethodInfo _gradientFieldMethod = null;
	
	private Type _fieldType = null;
	private GUIContent _label = null;
	
	LabelAttribute commentAttribute { get { return (LabelAttribute)attribute; } }
	
	// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent oldLabel)
	{
		
		GUIStyle labelStyle = new GUIStyle();
		labelStyle.normal.textColor = AssignColor(commentAttribute.color);

        //EditorGUIUtility.LookLikeControls();
        // Get the defaulkt values for labelWidth & fieldWidth (149 & 50)
        //float _labelWidth = EditorGUIUtility.labelWidth;
        //float _fieldWidth = EditorGUIUtility.fieldWidth;
        //Debug.Log(_labelWidth +"-"+ _fieldWidth);
        EditorGUIUtility.labelWidth = 149;
        EditorGUIUtility.fieldWidth = 50;

        if (commentAttribute.colorizeAll)
			SetColor(labelStyle.normal.textColor);
		position.x += 5;
		position.width -= 8;
		
		switch(property.propertyType)
		{
			//case SerializedPropertyType.Generic:
			//{
			//    break;
			//}
			//case SerializedPropertyType.ArraySize:
			//{
			//    break;
			//}
			//case SerializedPropertyType.Character:
			//{
			//	EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
			//	property.stringValue = EditorGUI.TextField(position, " ", property.stringValue);
			//    break;
			//}
	        //case SerializedPropertyType.Gradient:
	        //{
			//	EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
			//	Debug.Log(property.GetType());
			//	gradientFieldMethod.Invoke(property.arraySize, new object[] { position, property, new GUIContent(" ") });*/
	        //	break;
	        //}
			//case SerializedPropertyType.ArraySize:
			//{
			//  break;
			//}
			
			case SerializedPropertyType.AnimationCurve:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				position.width -= 4;
				property.animationCurveValue = EditorGUI.CurveField(position, " ", property.animationCurveValue);
				break;
			}
			case SerializedPropertyType.Boolean:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.boolValue = EditorGUI.Toggle(position, " ", property.boolValue);
				break;
			}
			case SerializedPropertyType.Bounds:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				position.x += 15;
				position.y += 15;
				position.width -= 18;
				property.boundsValue = EditorGUI.BoundsField(position, property.boundsValue);
				break;
			}
			case SerializedPropertyType.Color:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.colorValue = EditorGUI.ColorField(position, " ", property.colorValue);
				break;
			}
			case SerializedPropertyType.Enum:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.enumValueIndex = (int)(object)EditorGUI.EnumPopup(	position, 
																			" ",
				                                                           	Enum.Parse(GetFieldType(property), 
																			property.enumNames[property.enumValueIndex]) as Enum);
				break;
			}
			case SerializedPropertyType.Float:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.floatValue = EditorGUI.FloatField(position, " ", property.floatValue);
				break;
			}
			case SerializedPropertyType.Integer:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.intValue = EditorGUI.IntField(position," ", property.intValue);
				break;
			}
			case SerializedPropertyType.LayerMask:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				layerMaskFieldMethod.Invoke(property.intValue, new object[] { position, property, new GUIContent(" ") });
				break;
			}
			case SerializedPropertyType.ObjectReference:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.objectReferenceValue = EditorGUI.ObjectField(position, " ", property.objectReferenceValue,
				                                                      GetFieldType(property), true);
				break;
			}
			case SerializedPropertyType.Rect:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				position.width -= 4;
				property.rectValue = EditorGUI.RectField(position, " ", property.rectValue);
				break;
			}
			case SerializedPropertyType.String:
			{
				EditorGUI.LabelField(position,new GUIContent(label), labelStyle);
				property.stringValue = EditorGUI.TextField(position, " ", property.stringValue);
				break;
			}
	       case SerializedPropertyType.Vector2:
	       {
				Rect newPosition = position;
				EditorGUI.LabelField(newPosition,new GUIContent(label), labelStyle);
				newPosition.y += 15;
				EditorGUI.BeginChangeCheck();
				Vector2 vector2Value = EditorGUI.Vector2Field(newPosition, "", property.vector2Value);
				if (EditorGUI.EndChangeCheck())
	                property.vector2Value = vector2Value;
	           	break;
	       }
	       case SerializedPropertyType.Vector3:
	       {
				Rect newPosition = position;
				EditorGUI.LabelField(newPosition,new GUIContent(label), labelStyle);
				newPosition.y += 15;
	            EditorGUI.BeginChangeCheck();
				Vector3 vector3Value = EditorGUI.Vector3Field(newPosition, "", property.vector3Value);
	            if (EditorGUI.EndChangeCheck())
	                property.vector3Value = vector3Value;
	           	break;
	       }
			default:
			{
				Debug.LogWarning("LabelDrawer: found an un-handled type: " + property.propertyType);
				break;
			}
		}
		
		if(commentAttribute.colorizeAll)
			GUI.color = Color.white;
	}
	
	
	//==================================================================================================
	//
	//
	//
	//==================================================================================================
	
	#region Internal
	
	private void SetColor(Color _color){
		if(_color != Color.black)
			GUI.color = _color;
	}
	
	private Color AssignColor(string _color){
		Color finalColor = Color.red;
		
		switch(_color){
			case "Color.black":
				finalColor = Color.black;
				break;
			case "Color.blue":
				finalColor = Color.blue;
				break;
			case "Color.clear":
				finalColor = Color.clear;
				break;
			case "Color.cyan":
				finalColor = Color.cyan;
				break;
			case "Color.gray":
				finalColor = Color.gray;
				break;
			case "Color.green":
				finalColor = Color.green;
				break;
			case "Color.grey":
				finalColor = Color.grey;
				break;
			case "Color.magenta":
				finalColor = Color.magenta;
				break;
			case "Color.red":
				finalColor = Color.red;
				break;
			case "Color.white":
				finalColor = Color.white;
				break;
			case "Color.yellow":
				finalColor = Color.yellow;
				break;
			default:
				finalColor = Color.black;
				break;
		}
		
		return finalColor;
	}
	
	private static Type editorType
	{
		get
		{
			if(_editorType == null)
			{
				Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.EditorGUI));
				_editorType = assembly.GetType("UnityEditor.EditorGUI");
				if(_editorType == null)
				{
					Debug.LogWarning("LabelDrawer: Failed to open source file of EditorGUI");
				}
			}
			return _editorType;
		}
	}
	
	private static MethodInfo layerMaskFieldMethod
	{
		get
		{
			if(_layerMaskFieldMethod == null)
			{
				Type[] typeDecleration = new Type[] {typeof(Rect), typeof(SerializedProperty), typeof(GUIContent)};
				_layerMaskFieldMethod = editorType.GetMethod("LayerMaskField", BindingFlags.NonPublic | BindingFlags.Static,
				                                             Type.DefaultBinder, typeDecleration, null);
				if(_layerMaskFieldMethod == null)
				{
					Debug.LogError("LabelDrawer: Failed to locate the internal LayerMaskField method.");
				}
			}
			return _layerMaskFieldMethod; 
		}
	}
	
	/*private static MethodInfo gradientFieldMethod
	{
		get
		{
			if(_gradientFieldMethod == null)
			{
				Type tyEditorGUILayout = typeof(EditorGUILayout);
				Type[] typeDecleration = new Type[] {typeof(string), typeof(SerializedProperty), typeof(GUILayoutOption[])};
				_gradientFieldMethod = tyEditorGUILayout.GetMethod("GradientField", BindingFlags.NonPublic | BindingFlags.Static,
				                                             Type.DefaultBinder, typeDecleration, null);
				if(_gradientFieldMethod == null)
				{
					Debug.LogError("LabelDrawer: Failed to locate the internal gradientFieldMethod method.");
				}
			}
			return _gradientFieldMethod;
		}
	}*/
	
	private GUIContent label
	{
		get
		{
			if(_label == null)
			{
				LabelAttribute labelAttribute = attribute as LabelAttribute;
				_label = new GUIContent(labelAttribute.text, labelAttribute.tooltip);
			}
			return _label;
		}
	}
	
	
	
	private Type GetFieldType(SerializedProperty property)
	{
		if(_fieldType == null)
		{
			Type parentClassType = property.serializedObject.targetObject.GetType();
			FieldInfo fieldInfo = parentClassType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			
			if(fieldInfo == null)
			{
				Debug.LogError("LabelDrawer: Could not locate the object in the parent class");
				return null;
			}
			_fieldType = fieldInfo.FieldType;
		}
		return _fieldType;
	}
	
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
		float height = 0;
		if( (prop.propertyType == SerializedPropertyType.Vector2) ||
			(prop.propertyType == SerializedPropertyType.Vector3) )
        	height = base.GetPropertyHeight(prop, label) * 2 + 4;
		else
		if( (prop.propertyType == SerializedPropertyType.Rect) ||
			(prop.propertyType == SerializedPropertyType.Bounds) )
			height = base.GetPropertyHeight(prop, label) * 3 + 4;
		else
			height = base.GetPropertyHeight(prop, label);
		
		// if the property is expanded go thru all its children and get their height
		// Using LookLikeControls (no expanded vars in inspector)
        /*if(prop.isExpanded)
        {
            var propEnum = prop.GetEnumerator ();
            while (propEnum.MoveNext())
                height += EditorGUI.GetPropertyHeight((SerializedProperty)propEnum.Current, GUIContent.none, true);
        }*/
        return height;
    }
	
	#endregion
}

#endif