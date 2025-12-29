using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberBugBattle : EnemyBattleModel
{
    public bool airborn;
    public bool explode;
    public GameObject spitParticle;
    public GameObject explodeParticle;
    public Vector3 explodeRotation = new Vector3(-90, 0, 0);


    public override void StartAction()
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
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
            SelectRandomTarget();

            if (HealthCheck())
            {
                SelfDestruct(actionTarget);
            }
            if (!HealthCheck())
            {
                Attack(actionTarget);
            }            
        }
    }

    private bool HealthCheck()
    {
        bool healthLow = false;
        if (health <= maxH/4)
        {
            healthLow = true;
        }

        return healthLow;
    }

    public void SelfDestruct(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Self Destruct");
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(SelfDestructTimer());
    }

    public override void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(EnemyAttackTimer());
    }

    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        base.TakeDamage(damage, damSource, crit);
        if (HealthCheck() && !airborn)
        {
            airborn = true;
            anim.SetTrigger("goAir");
        }
    }

    IEnumerator EnemyAttackTimer()
    {
        if (!dead)
        {
            float xFL = strikeDistance + actionTarget.hitbuffer;
            Vector3 attackPosition = actionTarget.transform.position + (actionTarget.transform.forward * xFL);
            if (actionTarget.verticalHitBuffer != 0)
            {
                attackPosition = attackPosition + new Vector3(0, actionTarget.verticalHitBuffer, 0);
            }
            Vector3 returnPos = transform.position;
            transform.position = attackPosition;
            if (actionTarget.verticalHitBuffer == 0)
            {
                transform.LookAt(actionTarget.transform);
            }
            anim.SetTrigger("attack0");
            audioSource.PlayOneShot(actionSounds[0]);
            if (attCam != null)
            {
                attCam.m_Priority = 20;
            }
            yield return new WaitForSeconds(strikeTimer);
            TriggerTargetHit();

            if (attCam != null)
            {
                attCam.m_Priority = -10;
            }

            float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
            float defX = defAdjusted * actionTarget.def;

            float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;
            float powerStatusBoost = 0f;
            if (statusC.boost)
            {
                powerStatusBoost = statusC.boostAmount;
            }
            powerX = powerX + powerStatusBoost;
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

    IEnumerator SelfDestructTimer()
    {
        if (!dead)
        {
            explode = true;
            float xFL = strikeDistance + actionTarget.hitbuffer;
            Vector3 attackPosition = battleC.playerSpawnPoints[0].transform.position + (actionTarget.transform.forward * xFL);
            if (actionTarget.verticalHitBuffer != 0)
            {
                attackPosition = attackPosition + new Vector3(0, actionTarget.verticalHitBuffer, 0);
            }
            Vector3 returnPos = transform.position;
            transform.position = attackPosition;
            if (actionTarget.verticalHitBuffer == 0)
            {
                transform.LookAt(battleC.playerSpawnPoints[0].transform);
            }
            anim.SetTrigger("goGround");
            yield return new WaitForSeconds(1);
            anim.SetTrigger("explode");       
            if (attCam != null)
            {
                attCam.m_Priority = 20;
            }
            audioSource.PlayOneShot(actionSounds[3]);
            yield return new WaitForSeconds(3.5f);

            if (attCam != null)
            {
                attCam.m_Priority = -10;
            }

            float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
            float defX = defAdjusted * actionTarget.def;

            float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;
            float powerStatusBoost = 0f;
            if (statusC.boost)
            {
                powerStatusBoost = statusC.boostAmount;
            }
            powerX = powerX + powerStatusBoost;
            int damageAmount = (Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX)) * 2;
            if (damageAmount < 0)
            {
                damageAmount = 0;
            }

            Debug.Log(modelName + " attacking " + actionTarget.modelName + " for " + damageAmount + " damage");
            foreach (BattleModel hero in battleC.heroParty)
            {
                hero.TakeDamage(damageAmount, this);
                hero.GetHit(this);
            }

            TakeDamage(health, this); 

        }
        afterAction.Invoke();
        afterAction = null;
    }
    public void ExplodeBug()
    {

        GameObject newParticle = Instantiate(explodeParticle, transform.position, Quaternion.identity);
        newParticle.transform.eulerAngles = explodeRotation;
        Destroy(newParticle, 20.0f);
    }

    public override void GetHit(BattleModel modelSource)
    {
        if (!dead)
        {
            if (!airborn)
            {
                anim.SetTrigger("hit");
                audioSource.PlayOneShot(actionSounds[1]);
            }
            if (airborn && !explode)
            {
                anim.SetTrigger("flyGotHit");
                audioSource.PlayOneShot(actionSounds[5]);
            }
                
        }
    }

    public override void Die(BattleModel damSource)
    {
        Debug.Log(modelName + " has Died", gameObject);
        if (!airborn)
        {
            anim.SetTrigger("dead");
            audioSource.PlayOneShot(actionSounds[4]);
        }
        if (airborn && !explode)
        {
            anim.SetTrigger("flyDead");
            audioSource.PlayOneShot(actionSounds[6]);
        }
 
        dead = true;
        statusC.ResetAll();
    }
}
