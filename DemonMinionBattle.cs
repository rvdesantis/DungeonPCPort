using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Playables;

public class DemonMinionBattle : EnemyBattleModel
{
    public PlayableDirector spawnPlayable;
    public GameObject spawnVFX;

    public DemonMinionBattle partnerDemon;
    public List<PlayableDirector> combos;

    public void Spawn()
    {
        StartCoroutine(VFXTimer());
    }

    public override void Attack(BattleModel target)
    {
        base.Attack(target);
        audioSource.PlayOneShot(actionSounds[0]);
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
    IEnumerator VFXTimer()
    {
        GameObject newVFX = Instantiate(spawnVFX, transform.position, transform.rotation);
        newVFX.SetActive(true);
        yield return new WaitForSeconds(.05f);
        spawnPlayable.Play();
        yield return new WaitForSeconds(3);
        Destroy(newVFX);
    }

    public override void StartAction()
    {
        Debug.Log("Starting Demon Start Action", gameObject);
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }

       
        if (battleC.enemyIndex == 1)
        {
            Debug.Log("Assigning After Action", gameObject);
            afterAction = null;
            afterAction = battleC.enemyParty[2].StartAction;
        }
        if (battleC.enemyIndex == 2)
        {
            Debug.Log("Assigning After Action", gameObject);
            afterAction = null;
            afterAction = battleC.StartPostEnemyTimer;
        }

        if (skip || dead || DeadEnemiesCheck())
        {
            Debug.Log("Demon Skip or Dead", gameObject);
            skip = false;
            battleC.enemyIndex++;
            afterAction.Invoke();
            afterAction = null;
            return;
        }
        if (!skip && !dead)
        {            
            int x = battleC.enemyParty.IndexOf(this);
            

            if (x == 1)
            {
                Debug.Log("Demon in position 1, checking for combo");
                if (DemonTeamUpChecker())
                {
                    if (!partnerDemon.skip && !partnerDemon.dead)
                    {
                        Debug.Log("Triggering Combo", gameObject);
                        SelectRandomTarget();
                        int z = battleC.heroParty.IndexOf(actionTarget);

                        afterAction = null;
                        afterAction = battleC.StartPostEnemyTimer;
                        StartCoroutine(ComboTimer(z));
                        return;
                    }
                    else
                    SelectRandomTarget();
                    Attack(actionTarget);
                    return;
                }
                if (!DemonTeamUpChecker())
                {
                    SelectRandomTarget();
                    Attack(actionTarget);
                }
            }            
            if (x != 1)
            {
                SelectRandomTarget();
                Attack(actionTarget);
            }

        }
    }

    public bool DemonTeamUpChecker()
    {
        bool combo = false;
        Debug.Log("Checking for Partner Demon", gameObject);
        if (partnerDemon == null)
        {
            foreach (BattleModel enemy in battleC.enemyParty)
            {
                if (enemy != this)
                {
                    if (enemy.GetComponent<DemonMinionBattle>() != null)
                    {
                        partnerDemon = enemy.GetComponent<DemonMinionBattle>();
                        Debug.Log("Partner Demon Added", enemy.gameObject);
                        foreach (PlayableDirector dir in combos)
                        {
                            battleC.comboC.AssignComboPlayable(enemy, dir, 1);
                        }
                        break;
                    }
                }
            }
        }
        if (partnerDemon != null)
        {
            if (!partnerDemon.skip && !partnerDemon.dead && !skip && !dead)
            {
                combo = true;
            }
        }


        return combo;
    }

    IEnumerator ComboTimer(int comboNum) // after action set prior to ensure enemy isn't skipped out of position
    {
        float timer = 0f;
        PlayableDirector activeD = null;
        if (comboNum == 0)
        {
            timer = (float)combos[0].duration;
            activeD = combos[0];
        }
        if (comboNum == 1)
        {
            timer = (float)combos[1].duration;
            activeD = combos[1];
        }
        if (comboNum == 2)
        {
            timer = (float)combos[2].duration;
            activeD = combos[2];
        }
        activeD.Play();
        yield return new WaitForSeconds(1.75f);

        actionTarget.GetHit(this);
        actionTarget.impactFX.StandardImpact();

        yield return new WaitForSeconds(1.25f);

        actionTarget.GetHit(this);
        actionTarget.impactFX.StandardImpact();

        int totalDEF = Mathf.RoundToInt(actionTarget.def * (1 + (actionTarget.defBonusPercent / 100)));
        int damageAmount = (power * 2) - totalDEF;
        Debug.Log("Combo Targeting " + actionTarget.modelName + ", Power " + damageAmount +", vs " + totalDEF + " DEF");
        actionTarget.TakeDamage(damageAmount, this);
        yield return new WaitForSeconds(timer -1.8f);
        transform.position = spawnPoint;
        partnerDemon.transform.position = partnerDemon.spawnPoint;
        afterAction.Invoke();
        afterAction = null;

    }
}
