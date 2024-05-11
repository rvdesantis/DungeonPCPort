using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserRedDragonBCube : BossCube
{
    public BattleRoom activeRoom;
    public override void ConfirmBossBattle()
    {
        StartCoroutine(EasyBossRoomStartTimer());
    }

    public void BossBattleClear()
    {
        Debug.Log("Boss Battle Finished, Victory TEST");
    }

    IEnumerator EasyBossRoomStartTimer()
    {
        Debug.Log("Starting Boss Battle");


        PartyController party = FindObjectOfType<PartyController>();
        party.AssignCamBrain(bossStartPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(bossStartPlayable);
            model.transform.position = bossStartPlayable.transform.position;
            model.transform.parent = bossStartPlayable.transform;
            model.gameObject.SetActive(false); // set false instead of true
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
        }
        bossModel.AssignToDirector(bossStartPlayable, 4);
        bossTrigger = true;
        hallwayEnter.Stop();
        bossStartPlayable.Play();
        yield return new WaitForSeconds((float)bossStartPlayable.duration + .1f);
        bossModel.gameObject.SetActive(false);
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        } 
        StartBossBattle();
    }

    public override void StartBossBattle()
    {
        BattleController battleC = FindObjectOfType<BattleController>();
        battleC.afterBattleAction = BossBattleClear;
        battleC.SetBossBattle(0, activeRoom);
    }
}
