using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KnightVeteran : DunNPC
{
    public MessagePanelUI messagePanel;
    public List<string> gameTips;
    public List<string> usedTips;
    public List<AudioClip> usedAudio;
    public bool isToggling;

    public override void NPCTrigger()
    {
        if (inRange && !isToggling)
        {
            isToggling = true;
            StartCoroutine(MessageTimer());
        }
    }

    IEnumerator MessageTimer()
    {
        if (gameTips.Count == 0)
        {
            Debug.Log("Resetting Vet Tips");
            foreach (string tip in usedTips)
            {
                gameTips.Add(tip);
            }
            usedTips.Clear();
        }
        int audX = Random.Range(0, audioClips.Count);
        audioSource.PlayOneShot(audioClips[audX]);

        int x = Random.Range(0, gameTips.Count);
        messagePanel.OpenMessage(gameTips[x]);
        yield return new WaitForSeconds(3);
        if (messagePanel.currentString == gameTips[x])
        {
            messagePanel.gameObject.SetActive(false);
        }
        isToggling = false;
        usedTips.Add(gameTips[x]);
        gameTips.Remove(gameTips[x]);
    }
}
