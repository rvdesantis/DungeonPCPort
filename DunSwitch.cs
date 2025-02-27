using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DunSwitch : MonoBehaviour
{
    public bool switchOn;
    public bool inRange;
    public bool locked;
    public bool flipping;
    public bool lockOnFlip;
    public enum AnimType { animator, playable}
    public AnimType animType;
    public Animator switchAnim;
    public PlayableDirector switchPlayable;
    public AudioSource audioSource;
    public List<AudioClip> switchSounds;

    public IEnumerator FlipTimer()
    {
        yield return new WaitForSeconds(.5f);
        if (switchSounds.Count > 0)
        {
            audioSource.PlayOneShot(switchSounds[0]);
        }
        flipping = false;
    }

    public virtual void FlipSwitch()
    {
        if (!locked && !flipping)
        {
            DunUIController uiController = FindObjectOfType<DunUIController>();
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
            uiController.ToggleKeyUI(gameObject, false);

            if (!switchOn)
            {
                if (animType == AnimType.animator)
                {
                    switchOn = true;
                    flipping = true;
                    switchAnim.SetTrigger("switchOn");
                    if (audioSource != null && switchSounds.Count > 0)
                    {
                        audioSource.PlayOneShot(switchSounds[0]);
                    }
                    StartCoroutine(FlipTimer());
                }
                if (animType == AnimType.playable)
                {

                }
                if (lockOnFlip)
                {
                    locked = true;
                }
                return;
            }
            if (switchOn)
            {
                if (animType == AnimType.animator)
                {
                    switchAnim.SetTrigger("switchOff");
                    switchOn = false;
                    flipping = true;
                    StartCoroutine(FlipTimer());
                }
                if (animType == AnimType.playable)
                {

                }
            }
        }
    }

    public virtual void Update()
    {
        if (inRange && !flipping)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                FlipSwitch();
            }
        }
    }
}
