using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource SFXSource;

    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip run;
    public AudioClip coin;
    public AudioClip motor;
    public AudioClip finish;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
