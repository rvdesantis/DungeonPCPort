using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunChest : MonoBehaviour
{
    public Animator anim;
    public bool opened;
    public bool inRange;
    public bool cursed;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public FakeWall fakeWall;

    public void OpenChest()
    {
        if (inRange && !opened)
        {
            if (fakeWall == null)
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
            }
            if (fakeWall != null)
            {
                if (fakeWall.wallBreak)
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
                }
            }

        }
    }
}
