using Cinemachine;
using DTT.PlayerPrefsEnhanced;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TemplarNPC : DunNPC
{
    public DunUIController uiController;
    public ConfirmUI confirmUI;
    public PlayerController playerController;
    public SkullNPC roomSkull;
    public MessagePanelUI messagePanel;
    public List<string> gameTips;
    public List<string> skullBountyDialog;
    public List<string> usedTips;
    public List<AudioClip> usedAudio;
    public bool isToggling;
    public bool skullBounty;
    public bool inPartyVoidMage;

    private void Start()
    {
        if (uiController == null)
        {
            uiController = FindAnyObjectByType<DunUIController>();
        }
        if (confirmUI == null)
        {
            confirmUI = uiController.confirmUI;
        }
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
        if (messagePanel == null)
        {
            messagePanel = uiController.messagePanelUI;
        }
    }
    public override void NPCTrigger()
    {
        if (inRange && !isToggling)
        {
            if (!skullBounty)
            {
                isToggling = true;
                StartCoroutine(MessageTimer());
            }
            if (skullBounty)
            {
                StartCoroutine(ConfirmAttack());
            }
        }
    }

    public void TriggerAttackConf()
    {
        Debug.Log("Templar Attacked", gameObject);
        DistanceController distance = FindAnyObjectByType<DistanceController>();

        distance.npcS.Remove(this);
        uiController.interactUI.gameObject.SetActive(false);
        uiController.interactUI.activeObj = null;
        uiController.rangeImage.gameObject.SetActive(false);
        uiController.customImage.gameObject.SetActive(false);
        gameObject.SetActive(false);
        StartTemplarBossFight();
        
    }
    public void StartTemplarBossFight()
    {
        SceneController controller = FindAnyObjectByType<SceneController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();
        controller.endAction = null;
        controller.activePlayable = null;

        Debug.Log("Templar Boss Battle Started");
        battleC.afterBattleAction = null;
        battleC.battleUI.victoryUI.afterAction = AfterBattle;
        battleC.monsters.targetMonster = 1;
        battleC.SetBossBattle(1, battleC.battleRooms[3]);
    }

    public void AfterBattle()
    {
        SceneController controller = FindAnyObjectByType<SceneController>();

        bool vMageCheck = EnhancedPrefs.GetPlayerPref("voidMageUnlock", false);
        if (!vMageCheck)
        {
            LibraryRoomParent library = roomSkull.roomParent.activeENV.GetComponent<LibraryRoomParent>();
            if (library == null)
            {
                Debug.Log("Did not find Library Room Parent", gameObject);
            }
            if (library != null)
            {
                library.UnlockVoidMage();
            }
        }

        controller.playerController.enabled = true;
    }
    IEnumerator ConfirmAttack()
    {
        messagePanel.OpenMessage(skullBountyDialog[0]);
        yield return new WaitForSeconds(3);
        messagePanel.gameObject.SetActive(false);
        isToggling = false;

        uiController.confirmUI.ConfirmMessageNPC("Are you sure you want to attack Templar Knight?", null, null, this);
    }

    IEnumerator MessageTimer()
    {
        if (gameTips.Count == 0)
        {
            Debug.Log("Resetting Templar Messages");
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
