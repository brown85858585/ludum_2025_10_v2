// Examples:
// param1 (show sprite param): enable/disable/hidden
// param2 (alignement) left, right, center
//
// [SpriteDraw]	== [SpriteDraw("disable", "center")]
// [SpriteDraw(false)]
// [SpriteDraw(true, "left")]
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteDrawAttribute : PropertyAttribute {
	#region Properties

	// is the sprite centered?
	public readonly string disable = "hidden";  // "enable", "disable", "hidden";
	public readonly string centered = "center";	// "left, "right", "center"
	public readonly int sizex = 64;
	public readonly int sizey = 64;

	#endregion

	public SpriteDrawAttribute () : this ("hidden", "center", 64, 64) { 	}

	public SpriteDrawAttribute (string _disable) :  this(_disable, "center", 64, 64) { 	}

	public SpriteDrawAttribute (string _disable, string _centered) :  this(_disable, _centered, 64, 64) { 	}

	public SpriteDrawAttribute (string _disable, string _centered, int _sizex, int _sizey)
	{
		this.disable = _disable;
		this.centered = _centered;
		this.sizex = _sizex;
		this.sizey = _sizey;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SpriteDrawAttribute))]
public class SpriteDrawer : PropertyDrawer {

	private static GUIStyle s_TempStyle = new GUIStyle();
	SpriteDrawAttribute sprAtt { get { return ((SpriteDrawAttribute) attribute); } }

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
		if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null)
			return;

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
		s_TempStyle.Draw(spriteRect, GUIContent.none, false, false, false, false);

		EditorGUI.indentLevel = ident;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + sprAtt.sizey;
	}
}

#endif