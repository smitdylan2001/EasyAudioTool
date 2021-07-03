using UnityEngine;

//Used to organize audio objects into areas
[System.Serializable]
public struct AudioAreaList
{
    [Tooltip("Optional: contains the name of the area")]
    public string areaName;
    [Tooltip("Contains all data from the audio objects")]
    public AudioContents[] contents;
}
