using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplarMonkLeaderBattle : EnemyBattleModel
{
    public ParticleSystem lightStrikeVFX;



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
            CastAttack();
        }
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


}
