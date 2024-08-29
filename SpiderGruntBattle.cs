using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderGruntBattle : EnemyBattleModel
{
    public SpiderEggsBattle spiderEggs;



    public override void Die(BattleModel damSource)
    {
        base.Die(damSource);
        StartCoroutine(DieTimer());
    }

    IEnumerator DieTimer()
    {
        yield return new WaitForSeconds(2);
        int x = battleC.enemyParty.IndexOf(this);
        SpiderEggsBattle egg = Instantiate(spiderEggs, spawnPoint, transform.rotation);
        egg.spawnPoint = spawnPoint;
        egg.XP = egg.XP + XP;
        battleC.enemyParty[x] = egg;
        Debug.Log("Spawning Eggs at position " + x);
        gameObject.SetActive(false);
    }

    public override void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(EnemyAttackTimer());
    }

    IEnumerator EnemyAttackTimer()
    {
        if (!dead)
        {
            float xFL = strikeDistance + actionTarget.hitbuffer;
            Vector3 attackPosition = actionTarget.transform.position + (actionTarget.transform.forward * xFL);
            Vector3 returnPos = transform.position;
            transform.position = attackPosition;

            int crit = Random.Range(0, 100);
            if (crit >= critChance)
            {
                anim.SetTrigger("attack0");
            }
            if (crit < critChance)
            {
                Debug.Log(modelName + " CRIT HIT!");
                anim.SetTrigger("attack1");
            }
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
            actionTarget.statusC.AddPoison(5, this);
            actionTarget.impactFX.ElementalImpact(2);
            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }
}
