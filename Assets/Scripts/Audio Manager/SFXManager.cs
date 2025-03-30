using UnityEngine;

public class SFXManager : MonoBehaviour
{
	
	public static SFXManager instance;

	public AudioSource sFXObject;

	private void Awake() 
	{
		if (instance == null) 
		{
			instance = this;
		}
	}

	public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume)
	{
		AudioSource audioSource = Instantiate(sFXObject, spawnTransform.position, Quaternion.identity);
		
		audioSource.clip = audioClip;

		audioSource.volume = volume;

		audioSource.Play();


		float clipLen = audioSource.clip.length;

		Destroy(audioSource.gameObject, clipLen);
	}


	private void PlayMenuMusic()
	{

	}

	
}
