using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Audio Clips")]
    public AudioClip menuBGM;     // BGM untuk Main Menu & Level Select
    public AudioClip gameplayBGM; // BGM untuk Gameplay

    private void Awake()
    {
        // Sistem Singleton agar AudioManager tidak ganda & tidak hancur saat pindah Scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        // Cek jika lagu yang diputar sudah lagu menu, jangan di-restart!
        if (musicSource.clip == menuBGM && musicSource.isPlaying) return;

        musicSource.clip = menuBGM;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        if (musicSource.clip == gameplayBGM && musicSource.isPlaying) return;

        musicSource.clip = gameplayBGM;
        musicSource.loop = true;
        musicSource.Play();
    }
}