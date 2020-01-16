using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource PlayerAudioSource;
    public AudioSource MusicAudioSource;
    public AudioSource SoundEffectsAudioSource;

    public AudioClip ButtonClickClip;
    public AudioClip WalkClip;
    public AudioClip DoorOpenClip;
    public AudioClip SwordSwooshClip;
    public AudioClip MagicShotClip;
    public AudioClip FireClip;

    public AudioClip MenuMusic;
    public AudioClip GameMusic;

    //Static instance for singleton pattern.
    public static SoundManager Instance;


    void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
	}

    private SoundManager()
    {
        Instance = this;
    }

    /// <summary>
    /// This method changes the volume of the environment audio source
    /// </summary>
    /// <param name="vol">Vol.</param>
    public void ChangePlayerVolume(float vol)
    {
        PlayerAudioSource.volume = vol;
    }

    /// <summary>
    /// This method changes the volume of the music audio source
    /// </summary>
    /// <param name="vol">Vol.</param>
    public void ChangeMusicVolume(float vol)
    {
        MusicAudioSource.volume = vol;
    }

    /// <summary>
    /// This method changes the volume of the game sounds audio source
    /// </summary>
    /// <param name="vol">Vol.</param>
    public void ChangeGameVolume(float vol)
    {
        SoundEffectsAudioSource.volume = vol;
    }
}
