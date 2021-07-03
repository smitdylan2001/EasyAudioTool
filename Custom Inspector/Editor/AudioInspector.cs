using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//Custom Inspector for the AudioManager component
[CustomEditor(typeof(AudioManager))]
public class AudioInspector : Editor
{
	private ReorderableList list;

	private void OnEnable()
	{
		//Initailize reorderable list
		list = new ReorderableList(serializedObject,
				serializedObject.FindProperty("AudioContents"),
				true, true, true, true);

		//Draw reorderable list
		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, 90, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("startType"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 95, rect.y, (rect.width - 100 - 30)/2 - 5, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("gameObject"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + 95 + (rect.width - 100 - 30) / 2, rect.y, (rect.width - 100 - 30) / 2, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("audioClip"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("volume"), GUIContent.none);
			
			//TODO: Add other variables & descriptions
			//FIXME: Spacing not correct
			//EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y + EditorGUIUtility.singleLineHeight, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("range"), GUIContent.none);
		};

		//Add label to list
		list.drawHeaderCallback = (Rect rect) => 
		{
			EditorGUI.LabelField(rect, "Audio Objects");
		};

		//Ping selected GameObject when list item is selected
		list.onSelectCallback = (ReorderableList l) => {
			var gameObject = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("gameObject").objectReferenceValue as GameObject;
			if (gameObject) EditorGUIUtility.PingObject(gameObject.gameObject);
		};

		//Remove count when list item is removed
		list.onCanRemoveCallback = (ReorderableList l) => 
		{
			return l.count > 1;
		};

		//Give pop-up when item is removed from list
		list.onRemoveCallback = (ReorderableList l) => 
		{
			if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete the object?", "Yes", "No"))
			{
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			}
		};

		//Add starting variables to list item
		list.onAddCallback = (ReorderableList l) => 
		{
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			var element = l.serializedProperty.GetArrayElementAtIndex(index);
			element.FindPropertyRelative("startType").enumValueIndex = 0;
			element.FindPropertyRelative("volume").floatValue = 100;
			element.FindPropertyRelative("range").floatValue = 10;
			element.FindPropertyRelative("gameObject").objectReferenceValue = null;
			element.FindPropertyRelative("audioClip").objectReferenceValue = null;
		};
		
		//Add dropdown list with function
		list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => 
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("OnClick"), false, clickHandler, new ObjectCreationParams() { Type = AudioContents.StartType.OnClick });
			menu.AddItem(new GUIContent("OnCollision"), false, clickHandler, new ObjectCreationParams() { Type = AudioContents.StartType.OnCollision });
			menu.AddItem(new GUIContent("OnTrigger"), false, clickHandler, new ObjectCreationParams() { Type = AudioContents.StartType.OnTrigger });

			menu.ShowAsContext();
		};
	}

	//Draw inspector
	public override void OnInspectorGUI()
	{
		//Draw all other serialized/public variables
		base.OnInspectorGUI();

		//Draw reorderabe list
		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();

		//TODO: Check if gameobject has an collider on it!
	}

	//Save selectable params
	private struct ObjectCreationParams
	{
		public AudioContents.StartType Type;
	}

	//Add functionality to add dropdown item being clicked
	private void clickHandler(object target) 
	{
		var data = (ObjectCreationParams)target;
		var index = list.serializedProperty.arraySize;
		list.serializedProperty.arraySize++;
		list.index = index;
		var element = list.serializedProperty.GetArrayElementAtIndex(index);
		element.FindPropertyRelative("startType").enumValueIndex = (int)data.Type;
		element.FindPropertyRelative("volume").floatValue = 100;
		element.FindPropertyRelative("gameObject").objectReferenceValue = null;
		element.FindPropertyRelative("range").floatValue = 10;
		element.FindPropertyRelative("audioClip").objectReferenceValue = null;
		serializedObject.ApplyModifiedProperties();
	}
}