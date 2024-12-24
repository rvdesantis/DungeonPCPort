using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public Action afterAction;

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

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        exitBT.Select();
    }

    public void OpenDungeonWin()
    {
        StatsTracker statsTimer = FindObjectOfType<StatsTracker>();
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
            controller = FindObjectOfType<SceneController>();
        }
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

    private bool IsMouseOverButton()
    {
        bool isOver = false;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == exitBT.gameObject)
            {
                isOver = true;
            }
        }
        return isOver;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
            }
            if (IsMouseOverButton())
            {
                exitBT.Select();
            }
        }
        
    }

}
