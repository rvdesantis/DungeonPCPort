using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public BattleController battleC;
    public SceneController controller;
    public InventoryController inventory;
    public StatsTracker statsTimer;
    public TextMeshProUGUI titleTXT;
    public TextMeshProUGUI bodyTXT;
    public Button exitBT;
    public Action afterAction;

    public List<GameObject> itemParentObjs;
    public List<Image> rewardImages;

    public bool tier1;
    public bool tier2;
    public bool tier3;

    public List<DunItem> rewardItems;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = controller.inventory;
        }
    }

    public void OpenVictory(int gold, int totalXP, int totalCombos)
    {
        gameObject.SetActive(true);
        controller.uiController.uiActive = true;
        string goldString = "";
        string XPString = "";
        string comboString = "";

        goldString = "Gold Looted: " + gold;
        XPString = "XP Gained Per Hero: " + totalXP;
        comboString = "Combinations Landed: " + totalCombos;
        bodyTXT.text = goldString + "\n" + XPString + "\n" + comboString + "\n";
        bodyTXT.text = bodyTXT.text + "Found Items:";        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (tier1)
        {
            int rewardNum = UnityEngine.Random.Range(0, inventory.tier1RewardItems.Count);
            DunItem rewardItem0 = inventory.tier1RewardItems[rewardNum];
            rewardItems.Add(rewardItem0);
        }
        if (tier2)
        {
            int rewardNum = UnityEngine.Random.Range(0, inventory.tier2RewardItems.Count);
            DunItem rewardItem1 = inventory.tier1RewardItems[rewardNum];
            rewardItems.Add(rewardItem1);
        }
        if (tier3)
        {
            int rewardNum = UnityEngine.Random.Range(0, inventory.tier3RewardItems.Count);
            DunItem rewardItem2 = inventory.tier3RewardItems[rewardNum];
            rewardItems.Add(rewardItem2);
        }

        if (rewardItems.Count > 0)
        {
            if (rewardItems.Count >= 1)
            {
                itemParentObjs[0].SetActive(true);
                rewardImages[0].sprite = rewardItems[0].icon;
                itemParentObjs[1].SetActive(false);
                itemParentObjs[2].SetActive(false);
            }
            if (rewardItems.Count >= 2)
            {
                itemParentObjs[1].SetActive(true);
                rewardImages[1].sprite = rewardItems[1].icon;
                itemParentObjs[2].SetActive(false);
            }
            if (rewardItems.Count == 3)
            {
                itemParentObjs[2].SetActive(true);
                rewardImages[2].sprite = rewardItems[2].icon;
            }
        }

        foreach (DunItem rewardItem in rewardItems)
        {
            rewardItem.pickUpMessage = false;
            rewardItem.PickUp();
        }

        exitBT.Select();
    }

    public void OpenDungeonWin()
    {
        StatsTracker statsTimer = FindAnyObjectByType<StatsTracker>();
        string goldString = "";
        string XPString = "";
        string comboString = "";

        goldString = "Gold Looted: " + statsTimer.battleGold;
        XPString = "XP Gained Per Hero: " + statsTimer.battleXP;
        comboString = "Combinations Landed: " + statsTimer.battleCombos;

        titleTXT.text = "Dungeon Cleared!";
        bodyTXT.text = goldString + "\n" + XPString + "\n" + comboString + "\n";

        statsTimer.StopTimer();
        float minutes = Mathf.FloorToInt(statsTimer.elapsedTime / 60f);
        float seconds = statsTimer.elapsedTime % 60f;
        string timeString = string.Format("{0:00}:{1:00.##}", minutes, seconds);

        bodyTXT.text = bodyTXT.text + timeString + "\n";
    }

    public void CloseUI()
    {
        if (controller == null)
        {
            controller = FindAnyObjectByType<SceneController>();
        }
        rewardItems.Clear();
        controller.playerController.controller.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller.uiController.uiActive = false;
        controller.uiController.compassObj.SetActive(true);
        // removed Scene Reload.  SetBossBattle used for targeted miniboss battles;
        if (afterAction != null)
        {
            afterAction.Invoke();
            afterAction = null;
        }
        gameObject.SetActive(false);
    }  

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
            }
        }
        
    }

}
