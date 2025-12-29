using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class ArmorStandNPC : BlacksmithNPC
{
    public bool triggered;
    public bool assembled;
    public PlayableDirector startPlayable;
    public PlayableDirector idlePlayable;
    public PlayableDirector collapsePlayable;
    public DemonicStatueParent statueRoomParent;

    public List<GameObject> floatingPieces;
    public List<GameObject> fallingPieces;

    public bool upgraded;

    public override void NPCTrigger()
    {
        if (!upgraded)
        {
            DunUIController uicontroller = FindAnyObjectByType<DunUIController>();
            BlacksmithUI blackUI = uicontroller.blackSmithUI;
            if (uiObject = null)
            {
                uiObject = blackUI.gameObject;
            }
            if (!triggered)
            {
                triggered = true;
                Debug.Log("assembling Armor", gameObject);
                StartCoroutine(Assemble());
            }
            if (assembled && !upgraded)
            {
                UpgradeAllArmor();
            }
        }      
    }

    IEnumerator Assemble()
    {   
        startPlayable.Play();
        yield return new WaitForSeconds((float)startPlayable.duration);
        idlePlayable.Play();
        assembled = true;
    }

    public void UpgradeAllArmor()
    {
        upgraded = true;
        PartyController party = FindAnyObjectByType<PartyController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        float increaseAmount = 4.5f;
        float currentpercent = 0;

        foreach (BattleModel character in party.combatParty)
        {
            currentpercent = EnhancedPrefs.GetPlayerPref(character.modelName + "DefPercent", 0f);
            PlayerController player = FindAnyObjectByType<PlayerController>();

            player.vfxLIST[0].gameObject.SetActive(true);
            player.vfxLIST[0].Play();
            player.audioSource.PlayOneShot(player.audioClips[1]);

            float newCount = currentpercent + increaseAmount;

            EnhancedPrefs.SetPlayerPref(character.modelName + "DefPercent", newCount);
            int count = EnhancedPrefs.GetPlayerPref(character.modelName + "DEFUpCount", 0) + 1;
            EnhancedPrefs.SetPlayerPref(character.modelName + "DEFUpCount", count);
            EnhancedPrefs.SavePlayerPrefs();

            if (!player.enabled)
            {
                player.enabled = true;
            }
        }

        string mess = "DEF permanently increased by " + increaseAmount;
        uiController.messagePanelUI.gameObject.SetActive(true);
        uiController.messagePanelUI.MessageTimer(3);
        uiController.messagePanelUI.text.text = mess;
        remove = true;
        Disassemble();
    }

    public void Disassemble()
    {
        idlePlayable.Stop();
        idlePlayable.gameObject.SetActive(false);
        foreach (GameObject obj in floatingPieces)
        {
            gameObject.SetActive(false);
        }
        foreach (GameObject obj in fallingPieces)
        {
            gameObject.SetActive(true);
            gameObject.transform.SetParent(null);
        }
        IEnumerator MessageTimer()
        {
            DunUIController uiController = FindAnyObjectByType<DunUIController>();
            yield return new WaitForSeconds(3);
            uiController.messagePanelUI.gameObject.SetActive(false);            
        } StartCoroutine(MessageTimer());
    }

}
