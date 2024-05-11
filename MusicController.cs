using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public SceneController controller;

    public AudioSource audioSource;
    public AudioClip introMusicClip;
    public List<AudioClip> dungeonMusicClips;
    public List<AudioClip> battleMusicClips;

    public float fadeDuration;

    private void Start()
    {
        audioSource.clip = introMusicClip;
        audioSource.Play();
    }

    public void CrossfadeToNextClip(AudioClip nextClip)
    {
        StartCoroutine(Crossfade(nextClip));
    }

    IEnumerator Crossfade(AudioClip nextClip)
    {
        float timer = 0f;


        while (timer < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(.1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        audioSource.clip = nextClip;
        audioSource.Play();

 
        timer = 0f;
        while (timer < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, .1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = .1f;
    }


    private void FixedUpdate()
    {
        if (controller.activePlayable != null)
        {
            audioSource.Stop();
        }
    }

}
