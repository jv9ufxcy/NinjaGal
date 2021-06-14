using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource BackgroundMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
            //Debug.LogError("More than one AudioManager in the scene.");
        }
    }
    private void Start()
    {
        BackgroundMusic = GetComponent<AudioSource>();
    }
    private void FixedUpdate()
    {
        if (PauseManager.IsGamePaused)
            BackgroundMusic.volume = 0.1f;
        else
            BackgroundMusic.volume = 0.5f;
    }
    public void ChangeBGM(AudioClip music)
    {
        if (BackgroundMusic.clip.name == music.name)
            return;
        StopMusic();
        BackgroundMusic.clip = music;
        StartMusic();
    }
    public void StartBGM(AudioClip music)
    {
        StopMusic();
        BackgroundMusic.clip = music;
        StartMusic();
    }

    public void StartMusic()
    {
        BackgroundMusic.Play();
    }

    public void StopMusic()
    {
        BackgroundMusic.Stop();
    }
}
