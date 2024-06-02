using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MinotaurRunt : EnemyBattleModel
{
    public PlayableDirector playableRSwing;
    public PlayableDirector playableLSwing;
    public override void Attack(BattleModel target)
    {
        int attackNum = Random.Range(0, 2);
        if (attackNum == 0)
        {    
            Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
            actionTarget = target;
            battleC.enemyIndex++;
            StartCoroutine(AttackTimer());
        }
        if (attackNum == 1)
        {
            Debug.Log("Starting " + modelName + " Swinging Attack");
            battleC.enemyIndex++;
            StartCoroutine(SwingTimer());
        }
    }

    public override void GetHit(BattleModel modelSource)
    {
        audioSource.PlayOneShot(actionSounds[2]);
        base.GetHit(modelSource);
    }

    public override void Die(BattleModel damSource)
    {
        audioSource.PlayOneShot(actionSounds[1]);
        base.Die(damSource);
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

    IEnumerator SwingTimer()
    {
        Transform heroSpawn0 = battleC.activeRoom.playerSpawnPoints[0].transform;
        Transform heroSpawn1 = battleC.activeRoom.playerSpawnPoints[1].transform;
        Transform heroSpawn2 = battleC.activeRoom.playerSpawnPoints[2].transform;

        Vector3 returnPos = transform.position;


        BattleModel target0 = null;
        BattleModel target1 = null;

        int chance = Random.Range(0, 2);

        if (chance == 0) // attacks 0 and 2
        {
            target0 = battleC.heroParty[0];
            target1 = battleC.heroParty[2];


            playableLSwing.Play();   

            yield return new WaitForSeconds(2.5f);

            target0.GetHit(this);
            target1.GetHit(this);

            target0.impactFX.StandardImpact();
            target1.impactFX.StandardImpact();

            target0.TakeDamage(target0.maxH / 3, this);
            target1.TakeDamage(target1.maxH / 3, this);

            yield return new WaitForSeconds((float)playableLSwing.duration - 2.5f);
            playableLSwing.Stop();
            transform.position = returnPos;

        }
        
        if (chance == 1) // attacks 0 and 1;
        {
            target0 = battleC.heroParty[0];
            target1 = battleC.heroParty[1];


            playableRSwing.Play();

            yield return new WaitForSeconds(2.5f);

            target0.GetHit(this);
            target1.GetHit(this);

            target0.impactFX.StandardImpact();
            target1.impactFX.StandardImpact();

            target0.TakeDamage(target0.maxH / 3, this);
            target1.TakeDamage(target1.maxH / 3, this);

            yield return new WaitForSeconds((float)playableRSwing.duration - 2.5f);
            playableRSwing.Stop();
            transform.position = returnPos;
        }



        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator AttackTimer()
    {
        if (!dead)
        {
            float xFL = strikeDistance + actionTarget.hitbuffer;
            Vector3 attackPosition = actionTarget.transform.position + (actionTarget.transform.forward * xFL);
            Vector3 returnPos = transform.position;
            transform.position = attackPosition;
            transform.LookAt(actionTarget.transform);
            anim.SetTrigger("attack0");
            yield return new WaitForSeconds(strikeTimer);
            float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
            float defX = defAdjusted * actionTarget.def;

            float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;

            int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
            if (damageAmount < 0)
            {
                damageAmount = 0;
            }
            Debug.Log(modelName + " attacking " + actionTarget.modelName + " for " + damageAmount + " damage");
            actionTarget.TakeDamage(damageAmount, this);
            transform.position = returnPos;
            transform.LookAt(battleC.activeRoom.playerSpawnPoints[0].transform);
        }
        afterAction.Invoke();
        afterAction = null;
    }

}
