using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TemplarBattleScript : EnemyBattleModel
{
    public enum Templar { Warrior, Holy, MiniBoss}
    public Templar templarType;
    public DunModel afterTarget;
    public PlayableDirector introPlayable;
    public CinemachineVirtualCamera introVCam;
    public ParticleSystem healVFX; // used for Holy Templar
    public ParticleSystem lightStrikeVFX;

    private void Start()
    {
        if (templarType == Templar.MiniBoss)
        {
            introPlayable.Play();
            introVCam.gameObject.SetActive(true);
     
            IEnumerator CamTimer()
            {
                introVCam.m_Priority = 20;
                yield return new WaitForSeconds(3);
                introVCam.m_Priority = -5;
            }

            StartCoroutine(CamTimer());
        }
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
            afterTarget = battleC.enemyParty[1];            
        }
        if (battleC.enemyIndex == 1)
        {
            afterAction = battleC.enemyParty[2].StartAction;
            afterTarget = battleC.enemyParty[2];
        }
        if (battleC.enemyIndex == 2)
        {
            afterAction = battleC.StartPostEnemyTimer;
            afterTarget = battleC.heroParty[0];
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
            if (templarType == Templar.MiniBoss)
            {
                StartCoroutine(MiniBossTurnTimer());
            }
            if (templarType == Templar.Warrior)
            {
                SelectRandomTarget();
                Attack(actionTarget);
            }
            if (templarType == Templar.Holy)
            {
                bool healthCheck = false;
                foreach (BattleModel enemy in battleC.enemyParty)
                {
                    if (enemy.health < enemy.maxH/2 && !enemy.dead)
                    {
                        Debug.Log("Heal Needed, Cleric Healing Party", gameObject);
                        healthCheck = true;
                    }
                }
                if (healthCheck)
                {
                    CastHeal();
                }
                if (!healthCheck)
                {
                    SelectRandomTarget();
                    CastAttack();
                }                
            }
        }
    }

    public void CastShield() // attached to anim on Templar MiniBoss
    {
        statusC.DEFboost = 20;
        statusC.ActivateDEF(true);
        if (statusC.DEFboost == 0 && templarType != Templar.Holy)
        {
            anim.SetBool("block", true);
        }            
    }

    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        base.TakeDamage(damage, damSource, crit);
        if (statusC.DEFboost == 0 && templarType != Templar.Holy)
        {
            anim.SetBool("block", false);
        }
    }

    public override void GetHit(BattleModel modelSource)
    {
        base.GetHit(modelSource);
        audioSource.PlayOneShot(actionSounds[1]);
    }

    public override void Die(BattleModel damSource)
    {
        base.Die(damSource);
        audioSource.PlayOneShot(actionSounds[2]);
    }

    public void CastHeal()
    {
        StartCoroutine(HealTimer());
        anim.SetTrigger("spell0");
    }

    public void CastAttack()
    {        
        StartCoroutine(HolySpellTimer());
        anim.SetTrigger("attack0");
    }

    IEnumerator HolySpellTimer()
    {
        Debug.Log("Casting Spell Attack Timer");
        ParticleSystem lightPillar = Instantiate(lightStrikeVFX, actionTarget.transform);
        audioSource.PlayOneShot(actionSounds[4]);
        yield return new WaitForSeconds(2);
        float sDamage = 30;
        if (spellBonusPercent != 0)
        {
            sDamage = sDamage * (1 + (spellBonusPercent / 100));
        }
        int dam = ((int)sDamage);

        bool voidMageCheck = false;
        VoidBattleModel voidMage = null; 

        if (actionTarget.GetComponent<VoidBattleModel>() != null)
        {
            voidMageCheck = true;
            voidMage = actionTarget.GetComponent<VoidBattleModel>();
        }     
        if (!voidMageCheck)
        {
            actionTarget.TakeDamage(dam, this);
        }
        if (voidMageCheck)
        {
            dam = maxH * 2;
            actionTarget.TakeDamage(dam, this, true);
        }
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;

    }

    IEnumerator HealTimer()
    {
        Debug.Log("Casting Heal Timer");
        ParticleSystem healFX = null;
        DamageMSS healTXT = null;
        List<DamageMSS> usedCanvas = new List<DamageMSS>();

        audioSource.PlayOneShot(actionSounds[3]);
        foreach (BattleModel enemy in battleC.enemyParty)
        {
            if (!enemy.dead)
            {
                healFX = Instantiate(healVFX, enemy.transform);
                healFX.gameObject.SetActive(true);
                healFX.Play();

                enemy.Heal(25);
                healTXT = Instantiate(battleC.damageCanvas, enemy.transform);
                healTXT.gameObject.SetActive(true);
                healTXT.damTXT.color = Color.green;
                healTXT.damTXT.text = "25";
                usedCanvas.Add(healTXT);
            }
        }
        yield return new WaitForSeconds(2.5f);


        foreach (DamageMSS can in usedCanvas)
        {
            can.gameObject.SetActive(false);
            Destroy(can.gameObject);
        }
        healFX.gameObject.SetActive(false);
        Destroy(healFX.gameObject);
        battleC.enemyIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator MiniBossTurnTimer()
    {
        // cast shield, then attack or cast.
        anim.SetTrigger("special"); // activates CastShield()
        yield return new WaitForSeconds(1.5f);
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
