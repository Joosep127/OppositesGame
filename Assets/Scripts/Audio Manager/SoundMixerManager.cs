using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
	public AudioMixer audioMixer;

	public void SetMasterVolume (float level)
	{
		audioMixer.SetFloat("Master volume", level);
	}

	public void SetSFXVolume (float level)
	{
		audioMixer.SetFloat("SFX volume", level);
	}

	public void SetMusicVolume (float level)
	{
		audioMixer.SetFloat("Music volume", level);

	}
	
}

