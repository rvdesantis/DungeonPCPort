using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class HealerNPC : DunNPC
{
    public MessagePanelUI messageUI;
    public PlayableDirector healingCupPlayable;

    public override void NPCTrigger()
    {
        if (!opened)
        {
            StartCoroutine(HealTimer());
        }
      
    }

    IEnumerator HealTimer()
    {
        PartyController party = FindObjectOfType<PartyController>();
        opened = true;
        healingCupPlayable.Play();
        float duration = (float)healingCupPlayable.duration;
        yield return new WaitForSeconds(2);
        string healMSS = "Party HP Fully Recovered\nAll Curses Removed";
        messageUI.OpenMessage(healMSS);
        party.DungeonHeal(999, true);
        yield return new WaitForSeconds(3);
        if (messageUI.currentString == healMSS)
        {
            messageUI.gameObject.SetActive(false);
        }
        opened = false;
    }
}
