using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WolfBattleModel : EnemyBattleModel
{
    public bool pup;
    public List<BattleModel> wolves;
    public SanctuaryCube sanctuary;
    public PlayableDirector eatPlayable;
    public PlayableDirector pupRetire;

    private void Start()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        if (pup)
        {
            sanctuary = battleC.sceneController.sanctuary.GetComponent<SanctuaryCube>();
        }
        
    }

    public override void GetHit(BattleModel modelSource)
    {
        audioSource.PlayOneShot(actionSounds[1]);
        base.GetHit(modelSource);
    }


    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        Debug.Log("StartAction() started for enemy " + battleC.enemyIndex);
        if (battleC.enemyIndex == 0) // works for Enemy side
        {
            afterAction = battleC.enemyParty[1].StartAction;
        }
        if (battleC.enemyIndex == 1)
        {
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (battleC.enemyIndex == 2)
        {
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

            if (!pup)
            {
                if (health > (maxH / 4))
                {
                    SelectRandomTarget();
                    if (actionType == ActionType.melee)
                    {
                        Attack(actionTarget);
                    }
                }
                if (health <= (maxH/ 4))
                {
                    Eat();
                }
            }
            if (pup)
            {
                if (LivingCheck() == false)
                {
                    Surrender();
                }
                if (LivingCheck())
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
            
        }
    }

    public void Surrender()
    {
        actionCams[0].gameObject.SetActive(true);
        actionCams[0].m_Priority = 20;
        pupRetire.Play();
        pHolder = true;
        dead = true;
        StartCoroutine(SurrenderTimer());
       
    }

    IEnumerator SurrenderTimer()
    {
        yield return new WaitForSeconds(3);
        if (sanctuary == null)
        {
            if (battleC.sceneController != null)
            {
                sanctuary = battleC.sceneController.sanctuary.GetComponent<SanctuaryCube>();
            }
        }
        actionCams[0].m_Priority = -1;
        actionCams[0].gameObject.SetActive(false);
        afterAction.Invoke();
        afterAction = null;
        sanctuary.randomProps[0].gameObject.SetActive(true);
        WolfCubNPC wolfCub = sanctuary.randomProps[0].GetComponent<WolfCubNPC>();
        DistanceController distance = battleC.sceneController.distance;
        distance.npcS.Add(wolfCub);
    }

    public void Eat()
    {
        actionCams[0].gameObject.SetActive(true);
        actionCams[0].m_Priority = 20;
        StartCoroutine(EatTimer());
    }

    IEnumerator EatTimer()
    {
        float eatTimer = (float)eatPlayable.duration;
        eatPlayable.Play();
        yield return new WaitForSeconds(eatTimer);
        Heal(maxH / 2);
        actionCams[0].gameObject.SetActive(false);
        actionCams[0].m_Priority = -10;
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public bool LivingCheck()
    {
        bool alive = true;
        int deadCount = 0;
        foreach (BattleModel wolf in wolves)
        {
            if (wolf.dead)
            {
                deadCount++;
            }
        }

        if (deadCount == 2)
        {
            alive = false;
        }


        return alive;
    }


    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        base.TakeDamage(damage, damSource, crit);

        if (health <= maxH / 4)
        {
            if (anim.GetBool("aggressive") == false)
            {
                anim.SetBool("aggressive", true);
                statusC.boostAmount = statusC.boostAmount + 10;
                statusC.ActivateBoost(true);
                return;
            }
        }       
    }

}
