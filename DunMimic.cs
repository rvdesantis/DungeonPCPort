using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunMimic : DunChest
{

    public override void OpenChest()
    {
        if (inRange && !opened && !locked)
        {    
            if (fakeWall == null)
            {
                StartCoroutine(JumpTimer());
            }
            if (fakeWall != null)
            {
                if (fakeWall.wallBroken)
                {
                    DunUIController uiController = FindObjectOfType<DunUIController>();
                    uiController.ToggleKeyUI(gameObject, false);
                    StartCoroutine(JumpTimer());
                }
            }
        }
    }

    IEnumerator JumpTimer()
    {
        opened = true;
        if (animType == AnimType.animator)
        {
            if (anim != null)
            {
                anim.SetTrigger("open");
            }
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClips[0]);
            }
            yield return new WaitForSeconds(.5f);
        }
    }
}
