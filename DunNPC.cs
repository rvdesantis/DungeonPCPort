using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class DunNPC : MonoBehaviour
{
    public Animator anim;
    public bool opened;
    public bool engaged;
    public bool inRange;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public FakeWall fakeWall;
    public PlayableDirector idlePlayableLoop;

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
                if (fakeWall.wallBroken)
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

    public virtual void NPCIdleLoopTrigger()
    {

    }



}
