using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserRedDragonBoss : EnemyBattleModel
{
    bool flying;

    public override void Attack(BattleModel target)
    {
        if (!flying)
        {
            base.Attack(target);
        }
        if (flying)
        {
            if (!anim.GetBool("flying"))
            {
                anim.SetBool("flying", true);
            }
            StartCoroutine(FlyingAttackTimer());
        }

    }


    IEnumerator FlyingAttackTimer()
    {
        if (!dead)
        {
            float xFL = (strikeDistance + actionTarget.hitbuffer);
            Vector3 attackPosition = (actionTarget.transform.position + new Vector3(+xFL, 0, 0));
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

            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }
}
