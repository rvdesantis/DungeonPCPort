using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLauncherTester : MonoBehaviour
{
    public BattleTester bTester;
    public Button launchButton;
    public int battleNum;
    public bool boss;
    public BattleRoom bossRoom;

    public void LaunchBattle()
    {
        Debug.Log("Launching Battle");
        if (!boss)
        {
            bTester.SetBattle(battleNum);
        }
        if (boss)
        {
            bTester.SetBossBattle(battleNum, bossRoom);
        }
        launchButton.gameObject.SetActive(false);
    }

}
