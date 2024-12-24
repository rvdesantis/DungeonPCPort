using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailKeeperBattleModel : EnemyBattleModel
{
    public int attackCount;

    public void ClubSlamShake()
    {
        Debug.Log("Shaking Cam ", battleC.bCamController.activeCam.gameObject);
        battleC.bCamController.TriggerShake(.25f, 4, 2);
        audioSource.PlayOneShot(actionSounds[5]);
    }

    public override void Attack(BattleModel target)
    {
        base.Attack(target);
    }
    public void DoubleAttack(BattleModel target)
    {
        strikeTimer = 3f;
        Debug.Log("Starting " + modelName + "Double Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(DoubleAttackTimer());
    }

    public void TrippleAttack(BattleModel target)
    {
        strikeTimer = 5f;
        Debug.Log("Starting " + modelName + "Tripple Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(TrippleAttackTimer());
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
            SelectRandomTarget();   
            if (attackCount == 0)
            {
                Attack(actionTarget);
            }
            if (attackCount == 1)
            {
                DoubleAttack(actionTarget);
            }
            if (attackCount == 2)
            {
                TrippleAttack(actionTarget);
            }
        }
        attackCount++;
        if (attackCount > 2)
        {
            attackCount = 0;
            strikeTimer = 1.5f;
        }
    }

    IEnumerator DoubleAttackTimer()
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
            anim.SetTrigger("doubleAttack");
            if (attCam != null)
            {
                attCam.m_Priority = 20;
            }

            audioSource.PlayOneShot(actionSounds[1]);
            yield return new WaitForSeconds(1);
            audioSource.PlayOneShot(actionSounds[2]);
            yield return new WaitForSeconds(strikeTimer - 1);

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
            int extraDamage0 = damageAmount / 3;
            actionTarget.TakeDamage(damageAmount + extraDamage0, this);

            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator TrippleAttackTimer()
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
            anim.SetTrigger("trippleAttack");
            if (attCam != null)
            {
                attCam.m_Priority = 20;
            }
            audioSource.PlayOneShot(actionSounds[1]);
            yield return new WaitForSeconds(1);
            audioSource.PlayOneShot(actionSounds[2]);
            yield return new WaitForSeconds(.75f);
            audioSource.PlayOneShot(actionSounds[0]);
            yield return new WaitForSeconds(strikeTimer);

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
            int extraDamage0 = damageAmount / 3;
            int extraDamage1 = damageAmount / 2;
            actionTarget.TakeDamage(damageAmount + extraDamage0 + extraDamage1, this);

            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    public override void GetHit(BattleModel modelSource)
    {
        base.GetHit(modelSource);
        audioSource.PlayOneShot(actionSounds[4]);
    }


}
