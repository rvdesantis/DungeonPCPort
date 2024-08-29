using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TemplarBattleScript : EnemyBattleModel
{
    public enum Templar { Warrior, Holy, MiniBoss}
    public Templar templarType;
    public PlayableDirector introPlayable;
    public CinemachineVirtualCamera introVCam;

    private void Start()
    {
        introPlayable.Play();
        introVCam.gameObject.SetActive(true);
        introVCam.m_Priority = 20;
    }

    IEnumerator CamTimer()
    {
        yield return new WaitForSeconds(4);
        introVCam.m_Priority = -20;
        introVCam.gameObject.SetActive(false);
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
            if (templarType == Templar.MiniBoss)
            {
                StartCoroutine(MiniBossTurnTimer());
            }
            if (templarType == Templar.Warrior)
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
            if (templarType == Templar.Holy)
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

    public void CastShield() // attached to anim
    {
        statusC.DEFboost = 20;
        statusC.ActivateDEF(true);
        anim.SetBool("block", true);
    }

    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        base.TakeDamage(damage, damSource, crit);
        if (statusC.DEFboost == 0)
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
