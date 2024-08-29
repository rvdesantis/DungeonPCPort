using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GhostNightBattle : EnemyBattleModel
{
    public GameObject metalHead;
    public ParticleSystem headSmokeVFX;
    public bool cracked;
    public ParticleSystem cast1VFX;
    public bool comboTriggered;

    public PlayableDirector counterPlayable;
    public PlayableDirector spellPlayable;
    public List<BattleModel> activeTargets;
    public int castCount;

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
            int action = Random.Range(0, 2);
            if (action == 0)
            {
                SelectRandomTarget();
                Attack(actionTarget);
            }

            if (action == 1)
            {
                actionType = ActionType.melee;
                RandomAssign();
                StartCoroutine(CastCombo());
            }
        }
    }

    IEnumerator CastCombo()
    {
        float timer = (float) spellPlayable.duration;
        spellPlayable.Play();
        yield return new WaitForSeconds(timer + .1f);

        foreach(BattleModel hero in activeTargets)
        {
            if (hero != null)
            {
                if (!hero.dead)
                {
                    hero.transform.position = hero.spawnPoint;
                    float defAdjusted = (float)hero.defBonusPercent / 100f + 1f;
                    float defX = defAdjusted * hero.def;

                    float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
                    float powerX = powerAdjusted * power;

                    int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
                    if (damageAmount < 0)
                    {
                        damageAmount = 0;
                    }

                    Debug.Log(modelName + " attacking " + hero.modelName + " for " + damageAmount + " damage");
                    hero.TakeDamage(damageAmount, this);
                }
            }
        }
        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }


    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        base.TakeDamage(damage, damSource, crit);
        if (!cracked)
        {
            if (health <= maxH/2)
            {
                cracked = true;
                metalHead.SetActive(false);
                headSmokeVFX.gameObject.SetActive(true);
                headSmokeVFX.Play();
            }
        }
    }

    public void RandomAssign()
    {

        int x = Random.Range(0, 2);
        int y = Random.Range(0, 2);
        int z = Random.Range(0, 2);

        if (x == 0)
        {
            if (!battleC.heroParty[0].dead)
            {
                battleC.heroParty[0].AssignBattleDirector(spellPlayable);
                activeTargets[0] = battleC.heroParty[0];
            }            
        }
        if (y == 0)
        {
            if (!battleC.heroParty[1].dead)
            {
                battleC.heroParty[1].AssignBattleDirector(spellPlayable);
                activeTargets[1] = battleC.heroParty[1];
            }                
        }
        if (z == 0)
        {
            if (!battleC.heroParty[2].dead)
            {
                battleC.heroParty[2].AssignBattleDirector(spellPlayable);
                activeTargets[2] = battleC.heroParty[2];
            }  
        }

        if (x == 1 || battleC.heroParty[0].dead)
        {
            UnassignBindingAtPosition(0);
            activeTargets[0] = null;
            DamageMSS damCan = Instantiate(battleC.damageCanvas, battleC.heroParty[0].transform.position, Quaternion.identity);
            damCan.activeCam = battleC.bCamController.activeCam;
            damCan.damTXT.text = "MISS";
            damCan.damTXT.color = Color.white;
        }
        if (y == 1 || battleC.heroParty[1].dead)
        {
            UnassignBindingAtPosition(1);
            activeTargets[1] = null;
            DamageMSS damCan = Instantiate(battleC.damageCanvas, battleC.heroParty[1].transform.position, Quaternion.identity);
            damCan.activeCam = battleC.bCamController.activeCam;
            damCan.damTXT.text = "MISS";
            damCan.damTXT.color = Color.white;
        }
        if (z == 1 || battleC.heroParty[2].dead)
        {
            UnassignBindingAtPosition(2);
            activeTargets[2] = null;
            DamageMSS damCan = Instantiate(battleC.damageCanvas, battleC.heroParty[2].transform.position, Quaternion.identity);
            damCan.activeCam = battleC.bCamController.activeCam;
            damCan.damTXT.text = "MISS";
            damCan.damTXT.color = Color.white;
        }

        battleC.party.AssignCamBrain(spellPlayable, 3);
    }

    void UnassignBindingAtPosition(int bindingIndex)
    {
        var timelineAsset = spellPlayable.playableAsset as TimelineAsset;
        if (timelineAsset == null)
        {
            Debug.LogError("PlayableAsset is not a TimelineAsset.");
            return;
        }

        // Get the track at the specified position
        var track = timelineAsset.GetOutputTrack(bindingIndex);
        if (track == null)
        {
            Debug.LogError($"No track found at position {bindingIndex}.");
            return;
        }

        // Unassign the binding
        spellPlayable.SetGenericBinding(track, null);
        Debug.Log($"Binding at position {bindingIndex} unassigned.");
    }




}
