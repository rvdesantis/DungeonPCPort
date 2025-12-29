using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellNPC : DunNPC
{
    public int coinCount;
    public bool flipping;
    public GameObject healVFX;
    public GameObject waterBucketVFX;
    public InventoryController inventory;

    bool GoldCheck()
    {
        bool funds = false;
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<InventoryController>();
        }
        int goldAmount = inventory.GetAvailableGold();
        if (goldAmount > 0)
        {
            funds = true;
        }
        return funds;
    }
    public override void NPCTrigger()
    {
        if (inRange && !flipping)
        {
            if (GoldCheck() == true)
            {
                flipping = true;
                coinCount++;
                StartCoroutine(WellTimer());
            }
            else
            {
                DunUIController uiController = FindAnyObjectByType<DunUIController>();
                string goldString = "1 gold required to throw down well";
                uiController.messagePanelUI.OpenMessage(goldString);
                uiController.messagePanelUI.CloseMessageTimer(4);
            }
        }
    }

    IEnumerator WellTimer()
    {
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        uiController.messagePanelUI.OpenMessage("you throw a coin down the well (-1 Gold)");
        inventory.ReduceGold(1);
        audioSource.PlayOneShot(audioClips[0]);

        yield return new WaitForSeconds(.25f);
        audioSource.PlayOneShot(audioClips[1]);
        waterBucketVFX.gameObject.SetActive(true);
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
