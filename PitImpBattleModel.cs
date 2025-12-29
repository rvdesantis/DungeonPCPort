using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitImpBattleModel : EnemyBattleModel
{
    public ParticleSystem shadowFlash;
    public bool shadow;
    public List<GameObject> bodyList;
    public AudioClip shadowPoof;


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
            if (shadow)
            {
                ExitShadow();
            }
            SelectRandomTarget();


            if (actionType == ActionType.melee)
            {
                audioSource.PlayOneShot(actionSounds[2]);
                Attack(actionTarget);
            }

            if (actionType == ActionType.spell)
            {
                selectedSpell = masterSpells[0]; // for testing
                Cast(actionTarget, selectedSpell);
            }
        }
    }

    public override void GetHit(BattleModel modelSource)
    {
        if (!dead)
        {
            anim.SetTrigger("hit");
            audioSource.PlayOneShot(actionSounds[1]);
            StartCoroutine(ShadowDelay());
        }
    }

    public override void Die(BattleModel damSource)
    {
        ExitShadow();
        audioSource.PlayOneShot(actionSounds[3]);
        base.Die(damSource);
    }

    IEnumerator ShadowDelay()
    {
        yield return new WaitForSeconds(1);
        EnterShadow();
    }


    public void EnterShadow()
    {
        shadow = true;
        statusC.shadow = true;
        shadowFlash.gameObject.SetActive(true);
        shadowFlash.Play();
        foreach (GameObject obj in bodyList)
        {
            obj.SetActive(false);
        }
        audioSource.PlayOneShot(shadowPoof);
    }

    public void ExitShadow()
    {
        shadow = false;
        statusC.shadow = false;
        shadowFlash.gameObject.SetActive(true);
        shadowFlash.Play();
        foreach (GameObject obj in bodyList)
        {
            obj.SetActive(true);
        }
        audioSource.PlayOneShot(shadowPoof);
    }
}
