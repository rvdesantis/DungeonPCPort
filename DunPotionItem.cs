using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunPotionItem : DunItem
{
    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        StartCoroutine(DunPotionTimer());
    }

    IEnumerator DunPotionTimer()
    {
        PartyController party = FindAnyObjectByType<PartyController>();
        MessagePanelUI messageUI = FindAnyObjectByType<DunUIController>().messagePanelUI;
        string healMSS = "Party Recovered 20HP";

        messageUI.gameObject.SetActive(true);
        messageUI.OpenMessage(healMSS);
        party.DungeonHeal(20, true);
        yield return new WaitForSeconds(3);
        if (messageUI.currentString == healMSS)
        {
            messageUI.gameObject.SetActive(false);
        }
    }
}
