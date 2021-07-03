using UnityEngine;
using System;

//Contains all needed info to spawn audio object
[Serializable]
public struct AudioContents
{
	[Tooltip("Optional: add names to your interaction for organisation")] 
	public string objectName;

	public enum StartType
	{
		OnCollision,
		OnTrigger,
		OnClick
	}

	[Tooltip("Select what interaction will trigger the audio")]
	public StartType startType;
	[Tooltip("Add the gameobject which will trigger the audio")]
	public GameObject gameObject;
	[Tooltip("Add the audio clip which will play upon triggering")]
	public AudioClip audioClip;
	[Tooltip("Ajust the volume of the audioclip")]
	[Range(0, 100)] public float volume;
	[Tooltip("Ajust the range of the audio")]
	[Range(0, 100)] public float range;
}

