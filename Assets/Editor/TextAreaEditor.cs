using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextAreaScript)), CanEditMultipleObjects]
public class TextAreaEditor : Editor {
	
	public SerializedProperty longStringProp;
	
	void OnEnable () {
		longStringProp = serializedObject.FindProperty ("longString");
		Debug.Log ("ciaoneeee");
	}
	
	public override void OnInspectorGUI(){
		Debug.Log ("ciaoneeee");
		serializedObject.Update ();
		longStringProp.stringValue = EditorGUILayout.TextArea( longStringProp.stringValue, GUILayout.MaxHeight(75) );
		serializedObject.ApplyModifiedProperties ();
	}
}