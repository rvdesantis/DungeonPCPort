using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedScorpBattle : EnemyBattleModel
{

    private void Start()
    {
        audioSource.PlayOneShot(actionSounds[2]);
    }
    public override void Attack(BattleModel target)
    {
        audioSource.PlayOneShot(actionSounds[0]);
        base.Attack(target);
    }

    public override void GetHit(BattleModel modelSource)
    {
        base.GetHit(modelSource);
        audioSource.PlayOneShot(actionSounds[1]);
    }

    public override void Die(BattleModel damSource)
    {
        audioSource.PlayOneShot(actionSounds[3]);
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
            int action = Random.Range(0, 2);

            if (action == 0)
            {
                Attack(actionTarget);
            }

            if (action == 1)
            {
                StingAttack();
            }
        }
    }



    public void StingAttack()
    {
        StartCoroutine(StingTimer());
    }

    IEnumerator StingTimer()
    {
        float xFL = strikeDistance + actionTarget.hitbuffer;
        Vector3 attackPosition = actionTarget.transform.position + (actionTarget.transform.forward * xFL);
        Vector3 returnPos = transform.position;
        transform.position = attackPosition;
        anim.SetTrigger("attack1");
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
        yield return new WaitForSeconds(1);
        actionTarget.statusC.AddPoison(10, this);
        actionTarget.impactFX.ElementalImpact(2);
        afterAction.Invoke();
        afterAction = null;
    }
}
