using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
	//Make AudioManager a singleton
	private static AudioManager _instance;
	public static AudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<AudioManager>();
				if (_instance == null)
				{
					GameObject audioManagerGO = new GameObject("AudioManager_AutoGenerated");
					_instance = audioManagerGO.AddComponent<AudioManager>();
					_instance._interactRange = 10f;
					_instance = GameObject.FindObjectOfType<AudioManager>();
				}
			}

			return _instance;
		}
	}
	public static AudioManager GetAudioManager()
	{
		return Instance;
	}

	[SerializeField] [Tooltip("Drag the player gameobject here. If none is present, the first object with the name 'Player' or with tag 'Player' will be used")]
	private GameObject _player;
	public GameObject Player { get { return _player; } private set { _player = value; } }
	[SerializeField] [Range(1, 30)] [Tooltip("The range within the player can interact with clickable audio objects. Can be visualized with 'visualize clickable range' below")]
	private float _interactRange = 10f;
	[SerializeField] [Tooltip("Vizualizes the range in which the player can interact with clickable audio objects")]
	private bool _visualizeClickableRange = false;
	[SerializeField] [Tooltip("Will put a crosshair in he middle of the in-game screen using a canvas. Crosshair can be set in 'Easy Audio Tool/Resources'")]
	private bool _showCrosshair = false; 
	[SerializeField] [Tooltip("Will visualize an icon on all objects in the audio contents list")]
	private bool _showAudioObjects = false;
	[SerializeField] [Tooltip("This will hold all audio objects in the scene")]
	public List<AudioAreaList> AudioContents = new List<AudioAreaList>();

	private Dictionary<GameObject, AudioComponents> _audioDictionary = new Dictionary<GameObject, AudioComponents>();

	//Stores all the required components for the audio
	private struct AudioComponents
	{
		//Fill all variables for the audio 
		public AudioComponents(AudioContents.StartType st, AudioClip ac, float v, float r)
		{
			startType = st;
			audioClip = ac;
			volume = v;
			range = r;
		}

		public AudioContents.StartType startType;
		public AudioClip audioClip;
		public float volume;
		public float range;
	}

	//Toggle debug features depending on if the game is running in Unity or as a build
	private void Awake()
	{
#if UNITY_EDITOR
		Debug.unityLogger.logEnabled = true;
#else
		_showAudioDistance = false;
		_visualizeClickableRange = false;
		
		Debug.unityLogger.logEnabled = false;
#endif

		_instance = this;
	}

	//Gather and configure all data needed for the audiomanager to work
	private void Start()
	{
		//Get player Game Object if the _player variable is empty
		_player = GetPlayer();

		InitializePlayerAudioComponent();

		AddAudioContentsToDictionary();

		if (_showCrosshair) ShowCrosshair();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		//Draw gizmos sphere if it is enabled per audio object
		foreach (AudioAreaList aal in AudioContents)
		{
			foreach (AudioContents ac in aal.contents)
			{
				if (_showAudioObjects)
				{
					Gizmos.color = new Color(1, 0.1f, 0.1f);
					if(ac.gameObject) Gizmos.DrawIcon(ac.gameObject.transform.position, "d_preAudioAutoPlayOn", true);
				}
				if (ac.showAudioDistance)
				{
					Gizmos.color = new Color(1, 0.1f, 0.1f, 0.1f);
					if (ac.gameObject) Gizmos.DrawSphere(ac.gameObject.transform.position, ac.range);
				}
			}
		}
		//Draw gizmos sphere for the range of the player
		if (_visualizeClickableRange)
		{
			Gizmos.DrawSphere(_player.transform.position, _interactRange);
		}
	}
#endif

	//Get Player GameObject if possible
	private GameObject GetPlayer()
	{
		if (_player) return _player;
		if (GameObject.FindGameObjectWithTag("Player")) return GameObject.FindGameObjectWithTag("Player");
		if (GameObject.Find("Player")) return GameObject.Find("Player");

		Debug.LogError("No player recognized in the '" + this.name +
						"' script on the '" + gameObject.name + "' Game Object!\nNew Player Object made.");
		return new GameObject("Player_AutoGenerated");
	}

	//Add PlayerAudioComponent to player
	private void InitializePlayerAudioComponent()
	{
		//Add PlayerAudioComponent script to _player Game Object for audio at runtime
		PlayerAudioComponent playerAudio;
		if (!_player.GetComponent<PlayerAudioComponent>())
		{
			playerAudio = _player.AddComponent<PlayerAudioComponent>();
		}
		else
		{
			playerAudio = _player.GetComponent<PlayerAudioComponent>();
		}
		playerAudio.Initialize(this, _interactRange);
	}

	//Convert Audio Contents to a dictionary
	private void AddAudioContentsToDictionary()
	{
		//Add all audio contents in a directory, the key will be the gameobject and the value will be all audio components needed
		if (AudioContents.Count > 0)
		{
			foreach (AudioAreaList aal in AudioContents)
			{
				foreach (AudioContents ac in aal.contents)
				{
					if (!ac.gameObject || !ac.audioClip)
					{
						if (!ac.gameObject) Debug.LogError("No Gameobject on the AudioManager component on the '" + gameObject.name + "' Game Object");
						if (!ac.audioClip) Debug.LogError("No AudioClip on the AudioManager component on the '" + gameObject.name + "' Game Object");
						return;
					}
					else if (!ac.gameObject.GetComponent<AudioSource>()) ac.gameObject.AddComponent<AudioSource>();
					if (ac.volume == 0) Debug.LogWarning("AudioClip '" + ac.audioClip.name + "' on the AudioManager Component on '" + gameObject.name +
															"' has a volume of zero and will not be heard. Increase the volume if this is unintentional.");

					AddToAudioDictionary(ac.gameObject, new AudioComponents(ac.startType, ac.audioClip, ac.volume, ac.range));
				}
			}
		}
		else Debug.LogError("There are no Audio Components on the '" + this.name +
							"' component on the '" + gameObject.name + "' Game Object!");
	}

	//Display crosshair
	private void ShowCrosshair()
	{
		Object crosshairCanvas = Instantiate(Resources.Load("CrosshairCanvas"));
		if (!crosshairCanvas)
		{
			Debug.LogError("Crosshair is enabled in the AudioManager Component on the '" + gameObject.name +
							"' Game Object, but the Crosshair canvas could not be spawned. Make sure the CrosshairCanvas prefab is in the Resources folder within the Audio Tool folder.");
		}
	}

	//External: Add audio to dictionary at runtime
	public void AddAudio(GameObject go, AudioContents ac)
	{
		AddToAudioDictionary(ac.gameObject, new AudioComponents(ac.startType, ac.audioClip, ac.volume, ac.range));
	}

	//Internal: Add audio to dictionary at runtime
	private void AddToAudioDictionary(GameObject go, AudioComponents ac)
	{
		_audioDictionary.Add(go, ac);
	}

	//Play the audio connected to the Game Object
	public void PlayAudio(GameObject go, AudioContents.StartType startType)
	{
		if (_audioDictionary.ContainsKey(go))
		{
			AudioComponents component = _audioDictionary[go];

			//Check if the start type for the audio is correct
			if (component.startType == startType)
			{
				//Get audio source and play the audio clip
				AudioSource audioSource = go.GetComponent<AudioSource>();
				audioSource.Stop();
				audioSource.clip = component.audioClip;
				audioSource.maxDistance = component.range;
				audioSource.volume = component.volume / 100;
				audioSource.Play();
			}
		}
	}
}