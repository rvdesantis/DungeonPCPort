using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

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
    public Sprite icon;
    public string modelInfo;
    public Sprite modelIcon;
    public GameObject uiObject;
    public CinemachineVirtualCamera faceCam;
    public bool singleUse;


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
    } // triggered from DistanceController

    public virtual void NPCIdleLoopTrigger()
    {

    }

    public virtual void OpenUI()
    {
        if (uiObject != null)
        {

        }
    }


}
