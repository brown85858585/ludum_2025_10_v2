// file TouchableEditor.cs
// Correctly backfills the missing Touchable concept in Unity.UI's OO chain.


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Touchable))]
public class Touchable_Editor : Editor{ 
	
	public override void OnInspectorGUI(){} 

}
#endif
