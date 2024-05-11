using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalChest : DunChest
{
    public VictoryUI victorUI;
    public override void OpenChest()
    {
        victorUI = FindObjectOfType<BattleController>().battleUI.victoryUI;
        if (victorUI != null)
        {
            StartCoroutine(OpenEndSequence());
        }
        if (victorUI == null)
        {
            Debug.Log("ERROR, VictoryUI not attached to Final Chest");
        }
    }

    public IEnumerator OpenEndSequence()
    {
        
        float waitTime = 0;       
        opened = true; 
        waitTime = .5f;
        if (anim != null)
        {
            anim.SetTrigger("openLid");
        }
        if (audioSource != null)
        {
            if (audioClips.Count > 0)
            {
                audioSource.PlayOneShot(audioClips[0]);
            }
        }
        yield return new WaitForSeconds(waitTime);
        if (audioClips.Count >= 3)
        {
            if (audioClips[2] != null)
            {
                audioSource.PlayOneShot(audioClips[2]);
            }
        }

        victorUI.gameObject.SetActive(true);
        victorUI.OpenDungeonWin();
      
    }
}
