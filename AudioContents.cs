using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct AudioContents
{
	public enum StartType
	{
		OnCollision,
		OnTriggerVolume,
		OnClick
	}

	public StartType startType;
	public GameObject gameObject;
	public AudioClip audioClip;
	[Range(0, 100)] public float volume;
}

