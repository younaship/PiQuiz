using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCenter : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip sound;
    public AudioClip[] sounds;

    private void Awake()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = sound;
    }

    public void Sound()
    { 
        audioSource.Play();
    }

    public void Sound(int index)
    {
        if (index >= sounds.Length) index = sounds.Length;
        audioSource.clip = sounds[index];
        audioSource.Play();
    }
}
