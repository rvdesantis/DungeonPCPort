using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueWeeperBattle : EnemyBattleModel
{
    public ParticleSystem voidTrapVFX;
    public ParticleSystem voidAuraVFX;
    public ParticleSystem activeTrap;
    public ParticleSystem trapImpact;
    public AudioClip trapSFX;
    public bool trap;
    public bool aura;


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

        if (aura)
        {
            aura = false;
            voidAuraVFX.Stop();
            voidAuraVFX.gameObject.SetActive(false);
            anim.SetTrigger("Spell1End");

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
;
            int actionNum = Random.Range(0, 3);

            if (actionNum == 0)
            {
                actionType = ActionType.melee;
                SelectRandomTarget();
                Attack(actionTarget);
            }
            if (actionNum == 1 && !trap)
            {
                CastTrap();
                return;
            }
            if (actionNum == 1 && trap)
            {
                actionType = ActionType.melee;
                SelectRandomTarget();
                Attack(actionTarget);
            }
            if (actionNum == 2 && !aura)
            {
                CastAura();
                return;
            }
            if (actionNum == 2 && aura)
            {
                actionType = ActionType.melee;
                SelectRandomTarget();
                Attack(actionTarget);
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
            anim.SetTrigger("attack0");
            yield return new WaitForSeconds(strikeTimer);

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
            actionTarget.statusC.AddDark(2, this);
            actionTarget.statusC.darkModel = this;
            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    public void CastTrap()
    {
        trap = true;
        StartCoroutine(TrapTimer());
    }

    IEnumerator TrapTimer()
    {
        yield return new WaitForSeconds(.5f);
        anim.SetTrigger("spell0");
        yield return new WaitForSeconds(2.5f);
        ParticleSystem newTrap = Instantiate(voidTrapVFX, voidTrapVFX.transform.position, voidTrapVFX.transform.rotation);
        newTrap.gameObject.SetActive(true);
        newTrap.Play();
        activeTrap = newTrap;
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public void CastAura()
    {
        aura = true;
        StartCoroutine(AuraTimer());
    }

    IEnumerator AuraTimer()
    {
        yield return new WaitForSeconds(.5f);
        anim.SetTrigger("spell1");
        yield return new WaitForSeconds(1.5f);
        voidAuraVFX.gameObject.SetActive(true);
        voidAuraVFX.Play();
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public void TrapDamage(BattleModel modelSource)
    {
        float totalBoost = 1 + (spellBonusPercent / 100);
        float totalDam = activeSpells[1].baseDamage * totalBoost;
        int dam = Mathf.RoundToInt(totalDam);
        modelSource.TakeDamage(dam, this);
    }

    public override void GetHit(BattleModel modelSource)
    {
        anim.SetTrigger("hit");
        base.GetHit(modelSource);
        if (trap)
        {
            if (modelSource.actionType == ActionType.melee)
            {
                actionType = ActionType.spell;
                selectedSpell = activeSpells[0];
                trap = false;
                ParticleSystem trapFX = Instantiate(trapImpact, activeTrap.transform.position, activeTrap.transform.rotation);
                trapFX.Play();
                audioSource.PlayOneShot(trapSFX);
                activeTrap.gameObject.SetActive(false);
                Destroy(activeTrap.gameObject);
                TrapDamage(modelSource);
            }
        }
        if (aura)
        {
            if (modelSource.actionType == ActionType.melee)
            {
                modelSource.impactFX.ElementalImpact(1);
                modelSource.statusC.AddDark(2, this);               
            }
        }
    }
}
