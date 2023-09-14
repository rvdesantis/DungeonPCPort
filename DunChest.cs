using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunChest : MonoBehaviour
{
    public Animator anim;
    public bool opened;
    public bool inRange;
    public bool cursed;
    public bool locked;
    public AudioSource audioSource;
    public List<AudioClip> audioClips; // 0 - open, 1 - locked, 2 - treasure sound (coin, etc)
    public FakeWall fakeWall;

    public IEnumerator OpenSequence()
    {
        opened = true;
        if (anim != null)
        {
            anim.SetTrigger("openLid");
        }
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }
        yield return new WaitForSeconds(.25f);
        if (audioClips.Count >= 3)
        {
            if (audioClips[2] != null)
            {
                audioSource.PlayOneShot(audioClips[2]);
            }            
        }
    }

    public virtual void OpenChest()
    {
        if (inRange && !opened && !locked)
        {
            if (fakeWall == null)
            {
                StartCoroutine(OpenSequence());
            }
            if (fakeWall != null)
            {
                if (fakeWall.wallBroken)
                {
                    StartCoroutine(OpenSequence());
                }
            }
        }
        if (inRange && !opened && locked)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
    }
}
