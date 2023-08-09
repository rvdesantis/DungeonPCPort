using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    public DunUIController uiController;
    public PlayerController playerController;
    public AudioSource audioSource;
    public List<AudioClip> stepSoundsNormal;
    public List<AudioClip> stepSoundsGravel;
    public List<AudioClip> stepSoundsWet;
    public enum FloorType {stone, gravel, wet }
    public FloorType floorType;

    public bool engaged;

    public void WalkSounds()
    {

    }

    IEnumerator StepSounds()
    {
        engaged = true;
        yield return new WaitForSeconds(.25f);
        if (floorType == FloorType.stone)
        {
            int clipNum = Random.Range(0, stepSoundsNormal.Count);
            audioSource.PlayOneShot(stepSoundsNormal[clipNum], .2f);
        }
        if (floorType == FloorType.gravel)
        {
            int clipNum = Random.Range(0, stepSoundsNormal.Count);
            audioSource.PlayOneShot(stepSoundsNormal[clipNum], .2f);
        }
        if (floorType == FloorType.wet)
        {
            int clipNum = Random.Range(0, stepSoundsNormal.Count);
            audioSource.PlayOneShot(stepSoundsNormal[clipNum], .2f);
        }
        engaged = false;
    }


    private void Update()
    {
        if (playerController.walking && !engaged && uiController.uiActive == false)
        {
            StartCoroutine(StepSounds());
        }

    }

}
