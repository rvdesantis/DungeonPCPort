using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugTrapNPC : DunNPC
{
    public BugTrap bugTrap;
    public BattleController battleC;
    public bool launched;

    public override void NPCTrigger()
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
        }
        Debug.Log("Trigger Bomber Bug Battle");
        // Launch Battle
        bugTrap.bugIdlePlayable.Stop();        
        bugTrap.bugModel0.gameObject.SetActive(false);
        bugTrap.bugModel1.gameObject.SetActive(false);
        bugTrap.bugModel2.gameObject.SetActive(false);
        StartCoroutine(LaunchBattle());
    }

    IEnumerator LaunchBattle()
    {
        PlayerController player = battleC.sceneController.playerController;
        StatsTracker statsT = FindAnyObjectByType<StatsTracker>();

        player.controller.enabled = false;
        battleC.SetBattle(24);
        statsT.trapsTotal++;
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }


    private void FixedUpdate()
    {
        if (inRange && !launched)
        {
            launched = true;
            remove = true;
            NPCTrigger();
        }
    }
}
