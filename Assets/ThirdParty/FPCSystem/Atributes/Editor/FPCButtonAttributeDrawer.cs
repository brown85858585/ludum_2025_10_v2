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
using UnityEditor;


[CustomPropertyDrawer(typeof(PlayerButtonAttribute))]
public class SpriteDrawerv3 : PropertyDrawer {

	private static GUIStyle s_TempStyle = new GUIStyle();
    //private MethodInfo _eventMethodInfo = null;
    PlayerButtonAttribute sprAtt { get { return ((PlayerButtonAttribute) attribute); } }

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{                                    
		var ident = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect spriteRect;

		//create object field for the sprite
		spriteRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		if(sprAtt.disable.Contains("disable"))
			EditorGUI.BeginDisabledGroup(true);
		if(!sprAtt.disable.Contains("hidden"))
			property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.name, property.objectReferenceValue, typeof(Sprite), false);
		if(sprAtt.disable.Contains("disable"))
			EditorGUI.EndDisabledGroup();

		//if this is not a repain or the property is null exit now
		//if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null) return;
        if (property.objectReferenceValue == null) return;

        //draw a sprite
        Sprite sp = property.objectReferenceValue as Sprite;

		if(sprAtt.centered.Contains("center")) 
			spriteRect.x += EditorGUIUtility.currentViewWidth * 0.5f - (sprAtt.sizex-10);
		else if(sprAtt.centered.Contains("left")) 
			spriteRect.x += 5;
		else if(sprAtt.centered.Contains("right")) 
			spriteRect.x += EditorGUIUtility.currentViewWidth - (sprAtt.sizex+50);

		spriteRect.y += 10;
		if(!sprAtt.disable.Contains("hidden"))
			spriteRect.y += EditorGUIUtility.singleLineHeight;
		
		spriteRect.width = sprAtt.sizex;
		spriteRect.height = sprAtt.sizey;
		
        s_TempStyle.normal.background = sp.texture;
        EditorGUI.indentLevel = ident;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + sprAtt.sizey;
	}
}

