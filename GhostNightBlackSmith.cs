using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostNightBlackSmith : BlacksmithNPC
{
    public GhostKnightModel knightModel;
    public bool interact;
    

    public override void NPCTrigger()
    {
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        string mss = "Interact with Ghost Knight?\n(DiceRoll)";
        uiController.confirmUI.ConfirmMessageNPC(mss, this);
    }

    public override void OpenUI()
    {
        interact = true;
        int diceRoll = Random.Range(0, 2);
        if (diceRoll == 0)
        {
            base.OpenUI();
        }
       if (diceRoll == 1)
        {
            PartyController party = FindAnyObjectByType<PartyController>();
            party.AssignCamBrain(knightModel.battlePlayable);
            StartCoroutine(CombatTimer());

            // initiate Combat
        }
    }

    IEnumerator CombatTimer()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        BattleController battleC = FindAnyObjectByType<BattleController>();

        player.enabled = false;
        knightModel.battlePlayable.Play();
        yield return new WaitForSeconds((float)knightModel.battlePlayable.duration);

        battleC.SetBattle(8);

        InteractUI interact = FindAnyObjectByType<InteractUI>();
        if (interact != null)
        {
            if (interact.gameObject.activeSelf)
            {
                interact.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }
}
