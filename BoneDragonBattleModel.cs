using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BoneDragonBattleModel : EnemyBattleModel
{
    public ParticleSystem frostFX;
    public PlayableDirector freezeAllPlayable;


    public override void Cast(BattleModel target, Spell spell = null)
    {
        battleC.enemyIndex++;
        Debug.Log("Bone Dragon Spell Select Start");
        int spellNum = Random.Range(0, 2);
        if (spellNum == 0)
        {
            spell = masterSpells[0];
        }
        if (spellNum == 1)
        {
            spell = null;
        }
        if (spell == masterSpells[0])
        {
            selectedSpell = masterSpells[0];
            Debug.Log("Bone Dragon Spell 0");
            StartCoroutine(CastTimer());
        }
        if (spell == null)
        {
            Debug.Log("Bone Dragon Spell 1");
            StartCoroutine(FreezeTimer());
        }
    }

    public IEnumerator CastTimer()
    {
        transform.LookAt(actionTarget.transform);
        anim.SetTrigger("spell0");
        audioSource.PlayOneShot(actionSounds[1]);

        yield return new WaitForSeconds(1.5f); 
        actionTarget.TakeDamage(selectedSpell.baseDamage, this);
        actionTarget.GetHit(this);
        actionTarget.impactFX.ElementalImpact(3);

        yield return new WaitForSeconds(1);
        afterAction.Invoke();
        afterAction = null;

        transform.position = spawnPoint;
        transform.LookAt(battleC.activeRoom.playerSpawnPoints[0].transform);
    }

    public override void GetHit(BattleModel modelSource)
    {
        base.GetHit(modelSource);
        audioSource.PlayOneShot(actionSounds[2]);
    }

    public override void Die(BattleModel damSource)
    {
        base.Die(damSource);
        audioSource.PlayOneShot(actionSounds[3]);
    }
    public IEnumerator FreezeTimer()
    {
        bool bool0 = false;
        bool bool1 = false;
        bool bool2 = false;

        int freeze0 = Random.Range(0, 2);
        int freeze1 = Random.Range(0, 2);
        int freeze2 = Random.Range(0, 2);

        if (freeze0 == 1)
        {
            bool0 = true;
        }
        if (freeze1 == 1)
        {
            bool1 = true;
        }
        if (freeze2 == 1)
        {
            bool2 = true;
        }
        freezeAllPlayable.Play();

        yield return new WaitForSeconds(4);
        if (bool0) // right her set first
        {
            battleC.heroParty[2].GetHit(this);
            battleC.heroParty[2].impactFX.ElementalImpact(3);
            battleC.heroParty[2].statusC.freeze = true;
            battleC.heroParty[2].skip = true;
            battleC.heroParty[2].anim.SetBool("injured", true);
        }
        if (bool1) // middle her set 2nd
        {
            battleC.heroParty[0].GetHit(this);
            battleC.heroParty[0].impactFX.ElementalImpact(3);
            battleC.heroParty[0].statusC.freeze = true;
            battleC.heroParty[0].skip = true;
            battleC.heroParty[0].anim.SetBool("injured", true);
        }
        if (bool2)
        {
            battleC.heroParty[1].GetHit(this);
            battleC.heroParty[1].impactFX.ElementalImpact(3);
            battleC.heroParty[1].statusC.freeze = true;
            battleC.heroParty[1].skip = true;
            battleC.heroParty[1].anim.SetBool("injured", true);
        }

        float time = (float)freezeAllPlayable.duration;
        yield return new WaitForSeconds(time - 4); 
        afterAction.Invoke();
        afterAction = null;

        transform.position = spawnPoint;
        transform.LookAt(battleC.activeRoom.playerSpawnPoints[0].transform);
    }

    public void StartBreath()
    {
        frostFX.gameObject.SetActive(true);
        frostFX.Play();
    }

    public void StopBreath()
    {
        frostFX.Stop();
        frostFX.gameObject.SetActive(false);
    }

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
            int action = Random.Range(0, 2);
            if (action == 0)
            {
                Debug.Log("Bone Dragon Attack");
                Attack(actionTarget);
            }
            if (action == 1)
            {
                Debug.Log("Bone Dragon Cast Start");                
                Cast(actionTarget);       
            }
        }
    }

}
