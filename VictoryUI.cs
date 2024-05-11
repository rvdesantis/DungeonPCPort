using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public BattleController battleC;
    public SceneController controller;
    public StatsTracker statsTimer;
    public TextMeshProUGUI titleTXT;
    public TextMeshProUGUI bodyTXT;
    public List<GameObject> itemParents;
    public List<Image> itemImages;
    public Button exitBT;

    public void OpenVictory(int gold, int totalXP, int totalCombos, List<DunItem> foundItems = null)
    {
        gameObject.SetActive(true);
        controller.uiController.uiActive = true;
        foreach (GameObject obj in itemParents)
        {
            obj.SetActive(false);
        }
        string goldString = "";
        string XPString = "";
        string comboString = "";

        goldString = "Gold Looted: " + gold;
        XPString = "XP Gained Per Hero: " + totalXP;
        comboString = "Combinations Landed: " + totalCombos;

        bodyTXT.text = goldString + "\n" + XPString + "\n" + comboString + "\n";
        bodyTXT.text = bodyTXT.text + "Found Items:";
        if (foundItems != null)
        {            
            int count = foundItems.Count;
            foreach (GameObject obj in itemParents)
            {
                int index = itemParents.IndexOf(obj);
                if (index < count)
                {
                    obj.SetActive(true);
                }
                itemImages[index].sprite = foundItems[index].icon;
            }
        }

        exitBT.Select();
    }

    public void OpenDungeonWin()
    {
        StatsTracker statsTimer = FindObjectOfType<StatsTracker>();
        string goldString = "";
        string XPString = "";
        string comboString = "";

        goldString = "Gold Looted: " + statsTimer.totalGold;
        XPString = "XP Gained Per Hero: " + statsTimer.totalXP;
        comboString = "Combinations Landed: " + statsTimer.totalCombos;

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
        controller.playerController.controller.enabled = true;
        controller.uiController.uiActive = false;
        controller.uiController.compassObj.SetActive(true);

        if (battleC.bossBattle)
        {
            controller.builder.ReLoadScene();
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
