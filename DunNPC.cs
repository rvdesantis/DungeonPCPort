using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class DunNPC : MonoBehaviour
{
    public Animator anim;
    public bool opened;
    public bool inRange;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public FakeWall fakeWall;


    public virtual void NPCTrigger()
    {
        if (inRange && !opened)
        {
            if (fakeWall == null)
            {
                opened = true;
                if (anim != null)
                {
                    
                }
                if (audioSource != null)
                {
                    if (audioClips.Count > 0)
                    {
                        audioSource.PlayOneShot(audioClips[0]);
                    }
                }
            }
            if (fakeWall != null)
            {
                if (fakeWall.wallBreak)
                {
                    opened = true;
                    if (anim != null)
                    {

                    }
                    if (audioSource != null)
                    {
                        if (audioClips.Count > 0)
                        {
                            audioSource.PlayOneShot(audioClips[0]);
                        }
                    }
                }
            }
        }
    }



}
