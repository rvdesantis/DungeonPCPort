using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSwordNPC : DunNPC
{
    public GameObject swordObj;
    public GameObject swordRoots;

    private void Start()
    {
        swordObj.SetActive(true);
        swordRoots.SetActive(true);
    }

    public override void NPCTrigger()
    {
        if (inRange && !opened)
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
