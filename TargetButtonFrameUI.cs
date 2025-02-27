using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetButtonFrameUI : MonoBehaviour
{
    public GameObject frameObj;
    public BattleController battleC;
    public TextMeshProUGUI HPtxt;
    public TextMeshProUGUI ATTtxt;
    public TextMeshProUGUI DEFtxt;

    public Image boostIcon;
    public Image shieldIcon;
    public Image skipIcon;
    public Image deadIcon;

    public int buttonIndex;
    public bool enemySide;
    public Button attachedButton;
    public bool open;

    public void SetInfo(BattleModel targetModel)
    {
        HPtxt.text = "HP: " + targetModel.health;
        if (targetModel.health > targetModel.maxH)
        {
            HPtxt.color = Color.green;
        }
        if (targetModel.health <= targetModel.maxH && targetModel.health > targetModel.maxH / 2)
        {
            HPtxt.color = Color.white;
        }
        if (targetModel.health <= targetModel.maxH / 2 || targetModel.dead)
        {
            HPtxt.color = Color.red;
        }
        ATTtxt.text = "ATT: " + targetModel.power;
        DEFtxt.text = "DEF: " + targetModel.def;

        if (targetModel.statusC.boost && targetModel.statusC.boostAmount > 0)
        {
            boostIcon.gameObject.SetActive(true);
            ATTtxt.color = Color.green;
            int newPower = targetModel.power + targetModel.statusC.boostAmount;
            ATTtxt.text = "ATT: " + newPower;
        }
        if (!targetModel.statusC.boost || targetModel.statusC.boostAmount == 0)
        {
            boostIcon.gameObject.SetActive(false);
            ATTtxt.color = Color.white;
        }

        if (targetModel.statusC.DEF && targetModel.statusC.DEFboost > 0)
        {
            shieldIcon.gameObject.SetActive(true);
            DEFtxt.color = Color.green;
            int newDEF = targetModel.def + targetModel.statusC.DEFboost;
            DEFtxt.text = "DEF: " + newDEF;
        }
        if (!targetModel.statusC.DEF || targetModel.statusC.DEFboost == 0)
        {
            shieldIcon.gameObject.SetActive(false);
            DEFtxt.color = Color.white;
        }

        if (targetModel.skip)
        {
            skipIcon.gameObject.SetActive(true);
        }
        if (!targetModel.skip)
        {
            skipIcon.gameObject.SetActive(false);
        }

        if (targetModel.dead)
        {
            deadIcon.gameObject.SetActive(true);
        }
        if (!targetModel.dead)
        {
            deadIcon.gameObject.SetActive(false);
        }
    }

    public void SetClose()
    {
        open = false;
    }

    private void FixedUpdate()
    {
        if (!open)
        {
            if (EventSystem.current.currentSelectedGameObject == attachedButton.gameObject)
            {
                open = true;
                if (!enemySide)
                {
                    frameObj.SetActive(true);
                    SetInfo(battleC.heroParty[buttonIndex]);
                }
                if (enemySide)
                {
                    frameObj.SetActive(true);
                    SetInfo(battleC.enemyParty[buttonIndex]);
                }
            }
        }
        if (open)
        {
            if (EventSystem.current.currentSelectedGameObject != attachedButton.gameObject)
            {
                frameObj.SetActive(false);
                open = false;
            }
            if (EventSystem.current.currentSelectedGameObject == attachedButton.gameObject && !frameObj.activeSelf)
            {
                frameObj.SetActive(true);
                if (!enemySide)
                {
                    frameObj.SetActive(true);
                    SetInfo(battleC.heroParty[buttonIndex]);
                }
                if (enemySide)
                {
                    frameObj.SetActive(true);
                    SetInfo(battleC.enemyParty[buttonIndex]);
                }
            }
        }
    }
}


