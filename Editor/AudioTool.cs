using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//TODO: THIS IS A WIP FILE AND IS NOT FINISHED OR IN APLHA PHASE!
public class AudioTool : EditorWindow
{
    private GameObject _audioManagerGO;
    private AudioManager _audioManagerScript;
    private GameObject _player;

    [MenuItem("Window/Eazy Audio Tool")]
    static void ShowWindow()
    {
        GetWindow<AudioTool>("Audio Tool");
    }

    private void OnGUI()
    {
        EditorStyles.label.wordWrap = true;

        //Get audio manager's gameobject from input field
        _audioManagerGO = (GameObject)EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Audio Manager", _audioManagerGO, typeof(GameObject), true);

        //Find audio manager game object
        if (!_audioManagerGO)
        {
            _audioManagerScript = GameObject.FindObjectOfType<AudioManager>();
            if(_audioManagerScript) _audioManagerGO = _audioManagerScript.gameObject;
        }
        if (_audioManagerGO) _audioManagerScript = _audioManagerGO.GetComponent<AudioManager>();

        //Try to find audio manager's game object
        if (!_audioManagerGO)
        {
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "No Game Object Selected");
            GUILayout.FlexibleSpace();

            //Create new audio manager
            if (GUILayout.Button("Generate Audio Manager"))
            {
                _audioManagerGO = new GameObject("AudioManager");
                _audioManagerScript = _audioManagerGO.AddComponent<AudioManager>();
            }
        }
        //Try to get the audio manager script
        else if (!_audioManagerScript) 
        {   
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "No Audio Manager Component on " + _audioManagerGO.name);
            GUILayout.FlexibleSpace();

            //Add script to selected game object
            if (GUILayout.Button("Add Audio Manager Component to " + _audioManagerGO.name))
            {
                _audioManagerScript = _audioManagerGO.AddComponent<AudioManager>();
            } 
        }
        
        //Make sure player object is on or found by the audio manager
        if (_audioManagerScript && _audioManagerGO)
		{
            if(!_player) _player = GetPlayer();
            if(!_player) EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 60), "Warning:", "No GameObject named 'Player' or with tag 'Player' exists, please drag the player Game Object onto the 'AudioManager' component on the '" + _audioManagerGO.name + "' Game Object in your scene.");
        }

        this.Repaint();
    }

    //Get player object if possible
	private GameObject GetPlayer()
	{
        if (_audioManagerScript.Player) return _audioManagerScript.Player;
        if (GameObject.FindGameObjectWithTag("Player")) return GameObject.FindGameObjectWithTag("Player");
        if (GameObject.Find("Player")) return GameObject.Find("Player");
        return null;
	}
}