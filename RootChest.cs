using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootChest : DunChest
{
    public GameObject rootBlocker;



    private void Start()
    {
        if (!rootBlocker.activeSelf)
        {
            rootBlocker.SetActive(true);
            locked = true;
        }
    }

    public override void OpenChest()
    {
        if (rootBlocker.activeSelf)
        {
            locked = true;
        }
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
