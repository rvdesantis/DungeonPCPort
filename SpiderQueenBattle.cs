using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class SpiderQueenBattle : EnemyBattleModel
{
    public SpiderEggsBattle spiderEggs;
    public PlayableDirector eggsHatchPlayable;



    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
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
            if (dead)
            {
                foreach (GameObject bodyPart in bodyObjects)
                {
                    bodyPart.SetActive(false);
                }
            }
            battleC.enemyIndex++;
            afterAction.Invoke();
            afterAction = null;
            return;
        }
        if (!skip && !dead)
        {
            SelectRandomTarget();

            int actionNum = Random.Range(0, 2);
            if (!DeadCheck())
            {
                Attack(actionTarget);
            }

            if (DeadCheck())
            {
                HatchEggs();
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
            actionTarget.statusC.AddPoison(10, this);
            actionTarget.impactFX.ElementalImpact(2);
            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    public void HatchEggs()
    {
        StartCoroutine(HatchTimer());
    }

    public bool DeadCheck()
    {
        bool dead = false;
        int deadCount = 0;

        foreach (BattleModel enemy in battleC.enemyParty)
        {
            if (enemy.dead)
            {
                deadCount++;
            }
        }
        if (deadCount == 2)
        {
            dead = true;
        }


        return dead;
    }

    IEnumerator HatchTimer()
    {

        eggsHatchPlayable.Play();
        yield return new WaitForSeconds(2.5f);
        int x = 1;
        SpiderEggsBattle egg = Instantiate(spiderEggs, battleC.enemyParty[x].spawnPoint, battleC.enemyParty[x].transform.rotation);
        egg.spawnPoint = spawnPoint;
        egg.XP = egg.XP + battleC.enemyParty[x].XP;
        battleC.enemyParty[x] = egg;
        Debug.Log("Spawning Eggs at position " + x);

        afterAction = egg.StartAction;

        yield return new WaitForSeconds(4);
        x = 2;
        SpiderEggsBattle egg2 = Instantiate(spiderEggs, battleC.enemyParty[x].spawnPoint, battleC.enemyParty[x].transform.rotation);
        egg2.spawnPoint = battleC.enemyParty[x].spawnPoint;
        egg2.XP = egg2.XP + battleC.enemyParty[x].XP;
        battleC.enemyParty[x] = egg2;
        Debug.Log("Spawning Eggs at position " + x);
        egg.afterAction = egg2.StartAction;
        egg2.afterAction = battleC.StartPostEnemyTimer;
        float timer = (float)eggsHatchPlayable.duration;

        yield return new WaitForSeconds(3.5f);
        afterAction.Invoke();
        afterAction = null;
    }

}
