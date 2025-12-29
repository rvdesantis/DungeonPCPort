using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class LesserEntBattleModel : EnemyBattleModel
{
    public PlayableDirector teamWorkPlayable;
    public bool TeamChecker()
    {
        bool trigger = false;
        if (this == battleC.enemyParty[1] && !skip && !dead)
        {
            BattleModel secondEnt = battleC.enemyParty[2];
            if (!secondEnt.skip && !secondEnt.dead)
            {
                Debug.Log("TriggerCombo");
                secondEnt.AssignBattleDirector(teamWorkPlayable, 5);
                foreach (BattleModel model in battleC.heroParty)
                {
                    if (!model.dead)
                    {
                        model.AssignBattleDirector(teamWorkPlayable);
                    }
                }
                trigger = true;
            }
        }
        return trigger;
    }

    public override void StartAction()
    {
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
            if (TeamChecker())
            {
                Debug.Log("Starting Team Attack");
                StartCoroutine(TeamAttack());
            }
            else
            {
                SelectRandomTarget();
                Attack(actionTarget);
            }
        }
    }

    IEnumerator TeamAttack()
    {
        float teamTimer = (float)teamWorkPlayable.duration;
        
        teamWorkPlayable.Play();
        yield return new WaitForSeconds(teamTimer + .1f);
        foreach (BattleModel tree in battleC.enemyParty)
        {
            tree.transform.position = tree.spawnPoint;
        }
        foreach (BattleModel hero in battleC.heroParty)
        {
            if (!hero.dead)
            {
                int damage = Mathf.RoundToInt(power * 1.1f);
                hero.TakeDamage(damage - hero.def, this);
                hero.impactFX.StandardImpact();
                hero.GetHit(this);
            }
        }
        battleC.StartPostEnemyTimer();
    }

    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        if (battleC == null)
        {
            battleC = FindAnyObjectByType<BattleController>();
        }


        DamageMSS damCan = Instantiate(battleC.damageCanvas, hitTarget.transform.position, Quaternion.identity);
        damCan.activeCam = battleC.bCamController.activeCam;
        if (damSource.actionType == ActionType.spell)
        {
            if (damSource.selectedSpell.spellType == Spell.SpellType.fire)
            {
                damage = Mathf.RoundToInt(damage * 1.5f);
                damCan.ShowDamage(damage, true);
                health = health - damage;
                if (health <= 0)
                {
                    health = 0;
                    Die(damSource);
                }
                return;
            }
        }
        damCan.ShowDamage(damage);
        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            Die(damSource);
        }
    }
}
