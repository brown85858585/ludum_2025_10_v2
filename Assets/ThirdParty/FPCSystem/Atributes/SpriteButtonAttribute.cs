// Examples:
// param1 (show sprite param): enable/disable/hidden
// param2 (alignement) left, right, center
//
// [SpriteButton(OnButtonClicked)]  == [SpriteButton(OnButtonClicked, "disable", "center")]
// [SpriteButton(OnButtonClicked, false)]
// [SpriteButton(OnButtonClicked, true, "left")]
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage(System.AttributeTargets.Field)]
public class SpriteButtonAttribute : PropertyAttribute
{

    #region Properties

    // is the sprite centered?
    public readonly string disable = "hidden";  // "enable", "disable", "hidden";
    public readonly string centered = "center"; // "left, "right", "center"
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

    public SpriteButtonAttribute(string _methodName) : this(_methodName, "hidden", "center", 64, 64) { }

    public SpriteButtonAttribute(string _methodName, string _disable) : this(_methodName, _disable, "center", 64, 64) { }

    public SpriteButtonAttribute(string _methodName, string _disable, string _centered) : this(_methodName, _disable, _centered, 64, 64) { }

    public SpriteButtonAttribute(string _methodName, string _disable, string _centered, int _sizex, int _sizey)
    {
        this.MethodName = _methodName;
        this.disable = _disable;
        this.centered = _centered;
        this.sizex = _sizex;
        this.sizey = _sizey;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SpriteButtonAttribute))]
public class SpriteDrawerv2 : PropertyDrawer
{

    private MethodInfo _eventMethodInfo = null;
    SpriteButtonAttribute sprAtt { get { return ((SpriteButtonAttribute)attribute); } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var ident = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Rect spriteRect;

        //create object field for the sprite
        spriteRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (sprAtt.disable.Contains("disable"))
            EditorGUI.BeginDisabledGroup(true);
        if (!sprAtt.disable.Contains("hidden"))
            property.objectReferenceValue = EditorGUI.ObjectField(spriteRect, property.name, property.objectReferenceValue, typeof(Sprite), false);
        if (sprAtt.disable.Contains("disable"))
            EditorGUI.EndDisabledGroup();

        //if this is not a repain or the property is null exit now
        //if (Event.current.type != EventType.Repaint || property.objectReferenceValue == null) return;
        if (property.objectReferenceValue == null) return;

        //draw a sprite
        Sprite sp = property.objectReferenceValue as Sprite;

        if (sprAtt.centered.Contains("center"))
            spriteRect.x += EditorGUIUtility.currentViewWidth * 0.5f - (sprAtt.sizex - 10);
        else if (sprAtt.centered.Contains("left"))
            spriteRect.x += 5;
        else if (sprAtt.centered.Contains("right"))
            spriteRect.x += EditorGUIUtility.currentViewWidth - (sprAtt.sizex + 50);

        spriteRect.y += 10;
        if (!sprAtt.disable.Contains("hidden"))
            spriteRect.y += EditorGUIUtility.singleLineHeight;

        spriteRect.width = sprAtt.sizex;
        spriteRect.height = sprAtt.sizey;

        if (GUI.Button(spriteRect, sp.texture))
        {
            System.Type eventOwnerType = property.serializedObject.targetObject.GetType();
            string eventName = sprAtt.MethodName;

            if (_eventMethodInfo == null)
                _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (_eventMethodInfo != null)
                _eventMethodInfo.Invoke(property.serializedObject.targetObject, null);
            else
                Debug.LogWarning(string.Format("SpriteButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
        }
        EditorGUI.indentLevel = ident;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + sprAtt.sizey;
    }
}

#endif