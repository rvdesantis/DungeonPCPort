using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DemonessBattle : EnemyBattleModel
{
    public DemonMinionBattle demonMinion;
    public PlayableDirector attackAllPlayable;



    public override void StartAction()
    {
        Debug.Log("Demoness Start Action");
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
        if (!skip && !dead)
        {
            if (DeadDemonCheck()) // both minions are living
            {
                Debug.Log("Demon's Living, Selecting Attack", gameObject);
                int attackNum = Random.Range(0, 2);
                if (attackNum == 0)
                {
                    SelectRandomTarget();
                    actionType = ActionType.melee;
                    Attack(actionTarget);
                    return;
                }
                if (attackNum == 1)
                {
                    actionType = ActionType.melee;
                    Debug.Log("Triggering Multi-Attack", gameObject);
                    StartCoroutine(MultiAttackTimer());
                    return;
                }
              
            }
            if (!DeadDemonCheck())
            {
                Debug.Log("Demon's Dead, Selecting Summon", gameObject);
                actionType = ActionType.spell;
                if (battleC.enemyParty[1].dead)
                {
                    StartCoroutine(SummonOneTimer());
                    return;
                }
                if (battleC.enemyParty[2].dead)
                {
                    StartCoroutine(SummonTwoTimer());
                    return;
                }
            }
        }
    }

    IEnumerator MultiAttackTimer()
    {
        attackAllPlayable.Play();

        yield return new WaitForSeconds(1.5f);
        foreach (BattleModel hero in battleC.heroParty)
        {
            if (!hero.dead)
            {
                int damage = hero.health / 2;
                hero.TakeDamage(damage, this);
                hero.TimelineHit();
                hero.impactFX.StandardImpact();
            }
        }

        yield return new WaitForSeconds((float)attackAllPlayable.duration - 1.4f);

        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
    }


    public void SummonDemon(int pos)
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
        }
        if (pos == 1)
        {
            Debug.Log("Summoning Demon (Position 1)");
            Transform spawnPoint1 = battleC.activeRoom.enemySpawnPoints[1].transform;
            DemonMinionBattle demon1 = Instantiate(demonMinion, spawnPoint1);
            demon1.spawnPoint = spawnPoint1.position;
            demon1.Spawn();

            if (battleC.enemyParty[1].GetComponent<DemonMinionBattle>() != null && battleC.enemyParty[1].dead) // to check for placeholders
            {
                Debug.Log("Replacing Dead Demon Position 1");
                BattleModel oldDemon = battleC.enemyParty[1];
                demon1.XP = demon1.XP + oldDemon.XP; // to stack Demon XP
                Destroy(oldDemon.gameObject);
                demon1.skip = true;
            }
            battleC.enemyParty[1] = demon1;
            demon1.afterAction = battleC.enemyParty[2].StartAction;
            afterAction = null;
            afterAction = battleC.enemyParty[1].StartAction;




        }
        if (pos == 2)
        {
            Debug.Log("Summoning Demon (Position 2)");
            Transform spawnPoint2 = battleC.activeRoom.enemySpawnPoints[2].transform;
            DemonMinionBattle demon2 = Instantiate(demonMinion, spawnPoint2);
            demon2.spawnPoint = spawnPoint2.position;
            demon2.Spawn();

            if (battleC.enemyParty[2].GetComponent<DemonMinionBattle>() != null && battleC.enemyParty[2].dead)
            {
                Debug.Log("Replacing Dead Demon Position 2");
                BattleModel oldDemon = battleC.enemyParty[2];
                demon2.XP = demon2.XP + oldDemon.XP; // to stack Demon XP
                Destroy(oldDemon.gameObject);
                demon2.skip = true;
            }

            battleC.enemyParty[2] = demon2;
            battleC.enemyParty[1].afterAction = demon2.StartAction;
            demon2.afterAction = battleC.StartPostEnemyTimer;       
        }
    }

    public void SpawnOne() 
    {
        SummonDemon(1);
    }

    public void SpawnTwo() 
    {
        SummonDemon(2);
    }

    public bool DeadDemonCheck()
    {
        bool living = true;

        if (battleC.enemyParty[1].dead || battleC.enemyParty[2].dead)
        {
            living = false;
        }

        return living;
    }

    IEnumerator SummonOneTimer()
    {
        anim.SetTrigger("taunt");
        battleC.enemyIndex++;

        yield return new WaitForSeconds(1.5f);
        SpawnOne();
        yield return new WaitForSeconds(1.5f);

        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator SummonTwoTimer()
    {
        anim.SetTrigger("taunt");
        battleC.enemyIndex++;

        yield return new WaitForSeconds(1.5f);
        SpawnTwo();
        yield return new WaitForSeconds(1.5f);

        afterAction.Invoke();
        afterAction = null;
    }
}


