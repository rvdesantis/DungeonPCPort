using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistance : MonoBehaviour
{
    public AudioSource audioSource;
    public DistanceController distance;
    public float minVol;
    public float maxVol;

    public bool looper;
    public bool triggered;
    private float currentTime = 0f;

    private void Start()
    {
        if (distance == null)
        {
            distance = FindObjectOfType<DistanceController>();
            if (!distance.audioDistanceControllers.Contains(this))
            {
                distance.audioDistanceControllers.Add(this);
            }
        }
    }

    public void LowerVolume()
    {
        StartCoroutine(LowerVolumeOverTime());
    }

    public void RaiseVolume()
    {
        StartCoroutine(RaiseVolumeOverTime());
    }

    private IEnumerator LowerVolumeOverTime()
    {
        float elapsedTime = 0f;
        float currentVolume = audioSource.volume;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            currentTime += Time.deltaTime;

            currentVolume = Mathf.Lerp(maxVol, minVol, currentTime / 1);
            audioSource.volume = currentVolume;

            yield return null;
        }

        audioSource.volume = minVol;
        audioSource.Stop();
    }

    private IEnumerator RaiseVolumeOverTime()
    {
       

        audioSource.volume = 0;
        audioSource.Play();

        yield return new WaitForSeconds(.25f);
        float qVol = maxVol / 4;
        audioSource.volume = qVol;
        yield return new WaitForSeconds(.25f);
        float hVol = maxVol / 2;
        audioSource.volume = hVol;

        yield return new WaitForSeconds(.5f);
        audioSource.volume = maxVol;

    }
}
