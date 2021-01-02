using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource source;

    private IEnumerator fadeSound;

    private float startVolume;
    void Start()
    {
        fadeSound = null;
        source = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        source.clip = clip;
        if (fadeSound != null)
        {
            StopCoroutine(fadeSound);
            fadeSound = null;
            source.volume = startVolume;
        }
        source.Play();
    }

    public void Stop()
    {
        fadeSound = FadeOut(1);
        StartCoroutine(fadeSound);
    }

    public IEnumerator FadeOut(float FadeTime)
    {
        startVolume = source.volume;
 
        while (source.volume > 0) {
            source.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        source.Stop();
        source.volume = startVolume;
        fadeSound = null;
    }
}
