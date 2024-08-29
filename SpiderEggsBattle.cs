using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEggsBattle : EnemyBattleModel
{
    public SpiderGruntBattle spiderGrunt;


    public override void StartAction()
    {

        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        int x = battleC.enemyParty.IndexOf(this);
        if (x == 0) // works for Enemy side
        {
            afterAction = null;
            afterAction = battleC.enemyParty[1].StartAction;
        }
        if (x == 1)
        {
            afterAction = null;
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (x == 2)
        {
            afterAction = null;
            afterAction = battleC.StartPostEnemyTimer;
        }
        if (skip || dead || DeadEnemiesCheck())
        {
            skip = false;
            battleC.enemyIndex++;
            afterAction.Invoke();
            afterAction = null;
            return;
        }


        Debug.Log("Spawning Spider at position " + x);
        SpiderGruntBattle spider = Instantiate(spiderGrunt, battleC.activeRoom.enemySpawnPoints[x].transform.position, battleC.activeRoom.enemySpawnPoints[x].transform.rotation);
        spider.spawnPoint = spawnPoint;
        battleC.enemyParty[x] = spider;
        spider.XP = spider.XP + XP;

        afterAction.Invoke();
        afterAction = null;

        gameObject.SetActive(false);
    }

    public override void Die(BattleModel damSource)
    {
        base.Die(damSource);
        StartCoroutine(DieTimer());
    }
    IEnumerator DieTimer()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

}

