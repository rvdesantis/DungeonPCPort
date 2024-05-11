using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleModel : BattleModel
{

    public override void IntroPlayable()
    {
        if (battleStartPlayable != null)
        {
            if (battleC == null)
            {
                battleC = FindObjectOfType<BattleController>();
            }
            PartyController party = battleC.party;
            party.AssignCamBrain(battleStartPlayable, 3);

            foreach (BattleModel hero in battleC.heroParty)
            {
                hero.AssignBattleDirector(battleStartPlayable, 0, false, true);
            }
            StartCoroutine(BattleIntroTimer());
        }
        if (battleStartPlayable == null)
        {
            afterAction.Invoke();
            afterAction = null;
        }
       
    }

    IEnumerator BattleIntroTimer()
    {
        battleStartPlayable.Play();
        yield return new WaitForSeconds((float)battleStartPlayable.duration);
        afterAction.Invoke();
        afterAction = null;
    }

    public void SelectRandomTarget()
    {
        List<BattleModel> available = new List<BattleModel>();
        foreach (BattleModel model in battleC.heroParty)
        {
            if (!model.dead)
            {
                available.Add(model);
            }
        }

        actionTarget = available[Random.Range(0, available.Count)];

    }

    public override bool DeadEnemiesCheck()
    {
        bool allDead = false;
        int deadEnemies = 0;

        foreach (BattleModel en in battleC.heroParty)
        {
            if (en.dead)
            {
                deadEnemies++;
            }
        }

        if (deadEnemies == 3)
        {
            allDead = true;
        }

        return allDead;
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

           
            if (actionType == ActionType.melee)
            {
                Attack(actionTarget);
            }

            if (actionType == ActionType.spell)
            {
                selectedSpell = masterSpells[0]; // for testing
                Cast(actionTarget, selectedSpell);
            }
        }
    }

    public override void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(EnemyAttackTimer());
    }

    public override void Cast(BattleModel target, Spell spell)
    {
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
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

            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    public override void Die(BattleModel damSource)
    {
        Debug.Log(modelName + " has Died", gameObject);
        anim.SetTrigger("dead");
        dead = true;
    }
}
