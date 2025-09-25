/* MinMaxRangeIntDrawer.cs
* by Eddie Cameron – For the public domain
* ———————————————————–
* — EDITOR SCRIPT : Place in a subfolder named ‘Editor’ —
* ———————————————————–
* Renders a MinMaxRange field with a MinMaxRangeAttribute as a slider in the inspector
* Can slide either end of the slider to set ends of range
* Can slide whole slider to move whole range
* Can enter exact range values into the From: and To: inspector fields
*
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer( typeof( MinMaxRangeIntAttribute ) )]
public class MinMaxRangeIntDrawer : PropertyDrawer
{
	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		return base.GetPropertyHeight( property, label ) + 16;
	}
	
	// Draw the property inside the given rect
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		// Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
		if ( property.type != "MinMaxRangeInt" )
			Debug.LogWarning( "Use only with MinMaxRangeInt type" );
		else
		{
			var range = attribute as MinMaxRangeIntAttribute;
			var minValue = property.FindPropertyRelative( "rangeStart" );
			var maxValue = property.FindPropertyRelative( "rangeEnd" );
			var newMin = minValue.intValue;
			var newMax = maxValue.intValue;

			var newMinf = (float)newMin;
			var newMaxf = (float)newMax;
			
			var xDivision = position.width * 0.33f;
			var yDivision = position.height * 0.5f;
			EditorGUI.LabelField( new Rect( position.x, position.y, xDivision, yDivision )
			                     , label );
			
			EditorGUI.LabelField( new Rect( position.x, position.y + yDivision, position.width, yDivision )
			                     , range.minLimit.ToString( "0.##" ) );
			EditorGUI.LabelField( new Rect( position.x + position.width - 18f, position.y + yDivision, position.width, yDivision )
			                     , range.maxLimit.ToString( "0.##" ) );
			EditorGUI.MinMaxSlider( new Rect( position.x + 24f, position.y + yDivision, position.width - 48f, yDivision )
				, ref newMinf, ref newMaxf, (float)range.minLimit, (float)range.maxLimit );
			
			EditorGUI.LabelField( new Rect( position.x + xDivision, position.y, xDivision, yDivision )
			                     , "From: " );
			newMin = Mathf.Clamp( EditorGUI.IntField( new Rect( position.x + xDivision + 37, position.y, xDivision - 37, yDivision )
												, newMin )
			                     , range.minLimit, newMax );
			EditorGUI.LabelField( new Rect( position.x + xDivision * 2f, position.y, xDivision, yDivision )
			                     , "To: " );
			newMax = Mathf.Clamp( EditorGUI.IntField( new Rect( position.x + xDivision * 2f + 24, position.y, xDivision - 30, yDivision )
											, newMax )
			                     , newMin, range.maxLimit );

			newMin = (int)newMinf;
			newMax = (int)newMaxf;
			minValue.intValue = newMin;
			maxValue.intValue = newMax;
		}
	}
}