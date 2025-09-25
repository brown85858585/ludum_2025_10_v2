// http://www.jianshu.com/p/4047088b6861
// http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/


using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReorderableListBase), true)]
public class ReorderableListDrawer : PropertyDrawer
{
	private ReorderableList _list;

	private ReorderableList GetReorderableList(SerializedProperty property)
	{
		if (_list == null)
		{
			var listProperty = property.FindPropertyRelative("List");

			_list = new ReorderableList(property.serializedObject, listProperty, true, true, true, true);

			_list.drawHeaderCallback += delegate (Rect rect)
			{
				EditorGUI.LabelField(rect, property.displayName);
			};

			_list.drawElementCallback = delegate (Rect rect, int index, bool isActive, bool isFocused)
			{
				EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
			};
		}

		return _list;
	}


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return GetReorderableList(property).GetHeight();
	}


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var list = GetReorderableList(property);

		var listProperty = property.FindPropertyRelative("List");
		var height = 0f;
		for (var i = 0; i < listProperty.arraySize; i++)
		{
			height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
		}

		list.elementHeight = height;
		list.DoList(position);
	}
}