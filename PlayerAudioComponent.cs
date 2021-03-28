using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerAudioComponent : MonoBehaviour
{
	private AudioManager _audioManager;
	private float _range = 10f;
	private bool _MouseDown;

	public void Initialize(AudioManager audioManager, float range)
	{
		_audioManager = audioManager;
		_range = range;
	}

	private void Update()
	{
		CheckMouseInput();
	}

	private void CheckMouseInput()
	{
#if ENABLE_INPUT_SYSTEM
		if (!_MouseDown)
		{
			if (Mouse.current.leftButton.ReadValue() > 0.5f)
			{
				_MouseDown = true;
				Interact();
			}
		}
		else if (Mouse.current.leftButton.ReadValue() < 0.5f)
		{
			_MouseDown = false;
		}

#elif ENABLE_LEGACY_INPUT_MANAGER
		if (Input.GetMouseButtonDown(0))
		{
			Interact();
		}
#endif
	}


	//TODO: Add functionality to automatically enable inputs (in the new input system for now)
	public void Interact()
	{
		//Shoot raycast in the middle of the player to see what is in front of the player
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, _range))
		{
			if (hit.transform.gameObject.GetComponent<AudioSource>())
			{
				//Check if target has an audiosource
				_audioManager.PlayAudio(hit.transform.gameObject, AudioContents.StartType.OnClick);
			}
		}
	}

	//Trigger when colliding with a collider
	private void OnCollisionEnter(Collision col)
	{
		//Check if player isnt colliding with itself
		if (col.gameObject != gameObject)
		{
			//Check if target has an audiosource
			if (col.gameObject.GetComponent<AudioSource>())
			{
				_audioManager.PlayAudio(col.gameObject, AudioContents.StartType.OnCollision);
			}
			else Debug.LogWarning(col.gameObject + " has no Audio Source!");
		}
	}

	//Trigger when colliding with a trigger
	private void OnTriggerEnter(Collider col)
	{
		//Check if player isnt colliding with itself
		if (col.gameObject != gameObject)
		{	
			//Check if target has an audiosource
			if (col.gameObject.GetComponent<AudioSource>())
			{
				_audioManager.PlayAudio(col.gameObject, AudioContents.StartType.OnTriggerVolume);
			}
			else Debug.LogWarning(col.gameObject + " has no Audio Source!");
		}
	}
}
