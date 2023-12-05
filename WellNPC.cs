using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellNPC : DunNPC
{
    public int coinCount;
    public bool flipping;
    public GameObject healVFX;

    public override void NPCTrigger()
    {
        if (inRange && !flipping)
        {
            flipping = true;
            coinCount++;
            StartCoroutine(WellTimer());
        }
    }

    IEnumerator WellTimer()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        uiController.messagePanelUI.OpenMessage("you throw a coin down the well (-1 Gold)");
        audioSource.PlayOneShot(audioClips[0]);
        yield return new WaitForSeconds(.25f);
        audioSource.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(1.75f);
        uiController.messagePanelUI.gameObject.SetActive(false);
        flipping = false;
        if (coinCount == 5)
        {
            healVFX.SetActive(true);
            audioSource.PlayOneShot(audioClips[2]);
            string healString = "Party Healed for 20HP";
            uiController.messagePanelUI.OpenMessage(healString);
            uiController.controller.party.DungeonHeal(20, true);
            yield return new WaitForSeconds(2);
            if (uiController.messagePanelUI.currentString == healString)
            {
                uiController.messagePanelUI.gameObject.SetActive(false);
            }
        }
    }

}
