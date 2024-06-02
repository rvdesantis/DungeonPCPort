using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GargEnemyModel : EnemyBattleModel
{
    public List<BattleModel> gargList;
    public PlayableDirector trioPlayable;
    public void CollectGargs()
    {
        if (gargList.Count == 0)
        {
            gargList.Add(battleC.enemyParty[0]);
            gargList.Add(battleC.enemyParty[1]);
            gargList.Add(battleC.enemyParty[2]);

            // 0 pre assigned directly in prefab
            gargList[1].AssignBattleDirector(trioPlayable, 5);
            gargList[2].AssignBattleDirector(trioPlayable, 6);
            Debug.Log("Collecting Gargoyles and Assigning to Director");

            if (!battleC.heroParty[0].dead)
            {
                battleC.heroParty[0].AssignBattleDirector(trioPlayable);
            }
            if (!battleC.heroParty[1].dead)
            {
                battleC.heroParty[1].AssignBattleDirector(trioPlayable);
            }
            if (!battleC.heroParty[2].dead)
            {
                battleC.party.AssignCamBrain(trioPlayable, 3);
            }
            battleC.heroParty[2].AssignBattleDirector(trioPlayable);
        }

    }

    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        if (battleC.enemyIndex == 0) // works for Enemy side
        {
            afterAction = null;
            afterAction = battleC.enemyParty[1].StartAction;
        }
        if (battleC.enemyIndex == 1)
        {
            afterAction = null;
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (battleC.enemyIndex == 2)
        {
            afterAction = null;
            afterAction = battleC.StartPostEnemyTimer;
        }

        if (this == battleC.enemyParty[0])
        {
            CollectGargs();
        }
        int livingCount = 0;
        foreach(BattleModel garg in gargList)
        {
            if (!garg.dead && !garg.skip)
            {
                livingCount++;
            }
        }
        if (livingCount == 3)
        {
            if (skip || dead || DeadEnemiesCheck())
            {
                skip = false;
                battleC.enemyIndex++;
                afterAction.Invoke();
                afterAction = null;
                return;
            }
            if (!skip && !dead)
            {
                StartCoroutine(GangUpTimer());
            }
        }
        if (livingCount < 3)
        {
            if (skip || dead || DeadEnemiesCheck())
            {
                skip = false;
                battleC.enemyIndex++;
                afterAction.Invoke();
                afterAction = null;
                return;
            }
            if (!skip && !dead)
            {
                SelectRandomTarget();
                Attack(actionTarget);
            }
        }
    }

    IEnumerator GangUpTimer()
    {
        float playTime = (float)trioPlayable.duration;

        trioPlayable.Play();
        yield return new WaitForSeconds(playTime + .1f);
        trioPlayable.Stop();
        foreach(BattleModel garg in gargList)
        {
            garg.transform.position = garg.spawnPoint;
        }
        foreach (BattleModel hero in battleC.heroParty)
        {
            hero.GetHit(this);
            hero.impactFX.StandardImpact();
            hero.TakeDamage(hero.maxH / 3, this);
        }
        battleC.StartPostEnemyTimer();
    }
}
