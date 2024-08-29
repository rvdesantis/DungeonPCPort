using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrinketWolfCub : BattleTrinket
{
    public BattleModel puppy;

    public override void ActiveBattleTrinket()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        EnemyBattleModel enemyTarget = null;
        List<EnemyBattleModel> availEnemies = new List<EnemyBattleModel>(); 

        foreach (EnemyBattleModel enemy in battleC.enemyParty)
        {
            if (!enemy.dead && !enemy.pHolder)
            {
                availEnemies.Add(enemy);
            }
        }

        if (availEnemies.Count > 0)
        {
            int enNumber = Random.Range(0, availEnemies.Count);

            enemyTarget = availEnemies[enNumber];          
            
            StartCoroutine(PuppyAttackTimer(enemyTarget));
        }
    }


    IEnumerator PuppyAttackTimer(EnemyBattleModel enemyTarget)
    {
        float xFL = .5f + enemyTarget.hitbuffer;
        Vector3 attackPosition = enemyTarget.transform.position + (enemyTarget.transform.forward * xFL);
        BattleModel attackPup = Instantiate(puppy, attackPosition, Quaternion.identity);
        attackPup.transform.LookAt(enemyTarget.transform);

        attackPup.attCam.m_Priority = 20;
        attackPup.anim.SetTrigger("attack0");
        attackPup.audioSource.PlayOneShot(attackPup.actionSounds[0]);
        yield return new WaitForSeconds(1);
        enemyTarget.impactFX.StandardImpact();
        enemyTarget.TakeDamage(effectAmount, attackPup);
        attackPup.audioSource.PlayOneShot(attackPup.actionSounds[1]);
        yield return new WaitForSeconds(.5f);
        attackPup.attCam.m_Priority = -10;
        attackPup.gameObject.SetActive(false);
        Destroy(attackPup.gameObject);
    }
}
