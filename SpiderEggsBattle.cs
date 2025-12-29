using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEggsBattle : EnemyBattleModel
{
    public SpiderGruntBattle spiderGrunt;
    public ParticleSystem eggBurstFX;
    public GameObject meshObject;
    public GameObject larvMeshObject;


    public override void StartAction()
    {

        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
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
            if (dead)
            {
                foreach (GameObject bodyPart in bodyObjects)
                {
                    bodyPart.SetActive(false);
                }
            }
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
        audioSource.PlayOneShot(actionSounds[0]);
        meshObject.SetActive(false);
        larvMeshObject.SetActive(false);
        eggBurstFX.gameObject.SetActive(true);
        eggBurstFX.Play();
        yield return new WaitForSeconds(4);
        gameObject.SetActive(false);
    }

}

