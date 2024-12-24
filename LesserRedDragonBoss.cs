using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LesserRedDragonBoss : EnemyBattleModel
{
    public bool flying;
    public ParticleSystem dragonBreath;
    public ParticleSystem fireExplotion;
    public PlayableDirector fireAllPlayable;
    public PlayableDirector fireblastPlayable;
    public int attackSelection;
    public GameObject fireDragonObj;
    public CinemachineVirtualCamera flyHitCam;

    private void Start()
    {
        PartyController party = FindObjectOfType<PartyController>();
        party.AssignCamBrain(fireAllPlayable, 3);

        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
    }

    public void LandShake()
    {
        Debug.Log("Shaking Cam ", battleC.bCamController.activeCam.gameObject);
        battleC.bCamController.TriggerShake(.25f, 2, 2);
        audioSource.PlayOneShot(actionSounds[5]);
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

            if (attackSelection == 0) // high damage to all - Melee Attack
            {
                if (actionType == ActionType.melee)
                {
                    Attack(actionTarget); // triggers flying fire attack to all (Melee)
                }
            }

            if (attackSelection == 1) // trigger Fly
            {
                anim.SetTrigger("fly");
                anim.SetBool("flying", true);
                verticalHitBuffer = -7.25f;
                flying = true;
                afterAction.Invoke();
                afterAction = null;
            }
            if (attackSelection == 2)
            {
                anim.SetTrigger("land");             
                verticalHitBuffer = 0;
                flying = false;
                afterAction.Invoke();
                afterAction = null;
                IEnumerator Timer()
                {
                    yield return new WaitForSeconds(.25f);
                    anim.SetBool("flying", false);
                    yield return new WaitForSeconds(2f);
                    Vector3 fixedPosition = battleC.activeRoom.enemySpawnPoints[0].transform.position;
                    transform.position = fixedPosition;

                } StartCoroutine(Timer());
            }
            if (attackSelection == 3) // cast spell, single target
            {
                SelectRandomTarget();
                Cast(actionTarget, activeSpells[0]);
            }
            if (attackSelection == 4) // melee, single target
            {
                StartCoroutine(EnemyAttackTimer());
     
            }

            attackSelection++;

            if (attackSelection == 5)
            {
                attackSelection = 0;
            }
        }
    }

    public override void Cast(BattleModel target, Spell spell)
    {
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        selectedSpell = spell;
        spell.targetTransform = target.transform;
        StartCoroutine(CastTimer());
    }

    IEnumerator EnemyAttackTimer()
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
            anim.SetTrigger("attack0");
            audioSource.PlayOneShot(actionSounds[0]);
            if (attCam != null)
            {
                attCam.m_Priority = 20;
            }
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
            actionTarget.TakeDamage(damageAmount, this);

            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    IEnumerator CastTimer()
    {
        battleC.party.AssignCamBrain(fireblastPlayable, 3);
        fireblastPlayable.Play();
        GameObject fireDragonFX = Instantiate(fireDragonObj, actionTarget.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(5.5f);     
        actionTarget.GetHit(this);
        yield return new WaitForSeconds(2.5f);
        actionTarget.TakeDamage(selectedSpell.baseDamage, this);
        afterAction.Invoke();
        afterAction = null;
    }

    public override void Attack(BattleModel target)
    {  
        StartCoroutine(FlyingAttackTimer());
    }

    public void FirePositionZero()
    {
        ParticleSystem expl0 = Instantiate(fireExplotion, battleC.heroParty[0].transform.position, Quaternion.identity);
        expl0.Play();
        battleC.heroParty[0].GetHit(this);
    }
    public void FirePositionOne()
    {
        ParticleSystem expl1 = Instantiate(fireExplotion, battleC.heroParty[1].transform.position, Quaternion.identity);
        expl1.Play();
        battleC.heroParty[1].GetHit(this);
    }
    public void FirePositionTwo()
    {
        ParticleSystem expl2 = Instantiate(fireExplotion, battleC.heroParty[2].transform.position, Quaternion.identity);
        expl2.Play();
        battleC.heroParty[2].GetHit(this);
    }

    public override void GetHit(BattleModel modelSource)
    {
        if (!flying)
        {
            anim.SetTrigger("hit");
        }
        if (flying)
        {
            if (modelSource.actionType != ActionType.melee)
            {
                flyHitCam.m_Priority = 20;
                anim.SetTrigger("flyHit");
                IEnumerator HitCamTimer()
                {
                    yield return new WaitForSeconds(3);
                    flyHitCam.m_Priority = -10;
                }
                StartCoroutine(HitCamTimer());
            }          
        }

    }

    public override void Die(BattleModel damSource)
    {
        if (flying)
        {
            Debug.Log(modelName + " has Died", gameObject);
            anim.SetTrigger("flyDead");
            dead = true;
            statusC.ResetAll();
        }
        if (!flying)
        {
            Debug.Log(modelName + " has Died", gameObject);
            anim.SetTrigger("dead");
            dead = true;
            statusC.ResetAll();
            audioSource.PlayOneShot(actionSounds[1]);
            audioSource.PlayOneShot(actionSounds[2]);
        }

    }


    IEnumerator FlyingAttackTimer()
    {
        if (!dead)
        {
            foreach (BattleModel hero in battleC.heroParty)
            {
                int x = battleC.heroParty.IndexOf(hero);
                if (hero.dead)
                {
                    UnassignBindingAtPosition(x);
                }
            }
            Vector3 returnPos = transform.position;
            float timer = (float)fireAllPlayable.duration;
            fireAllPlayable.Play();
            yield return new WaitForSeconds(timer + .1f);

            float powerAdjusted = powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;

            foreach (BattleModel hero in battleC.heroParty)
            {
                float defAdjusted = hero.defBonusPercent / 100f + 1f;
                float defX = defAdjusted * hero.def;
                int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
                if (damageAmount < 0)
                {
                    damageAmount = 0;
                }
                Debug.Log(modelName + " attacking " + hero.modelName + " for " + damageAmount + " damage");
                hero.TakeDamage(damageAmount, this);
                hero.GetHit(this);
            }
            transform.position = returnPos;
        }
        afterAction.Invoke();
        afterAction = null;
    }

    public override void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        DamageMSS damCan = Instantiate(battleC.damageCanvas, hitTarget.transform.position, Quaternion.identity);
        damCan.activeCam = battleC.bCamController.activeCam;
        

        if (damSource.actionType == ActionType.melee)
        {
            if (!flying)
            {
                if (statusC.DEFboost > 0)
                {
                    damage = damage - statusC.DEFboost;
                    if (damage < 0)
                    {
                        damage = 0;
                    }
                    statusC.ActivateDEF(false);
                }
                damCan.ShowDamage(damage);

                health = health - damage;
                if (health <= 0)
                {
                    health = 0;
                    Die(damSource);
                }
            }
            if (flying)
            {
                damage = 0;
                DamageMSS damCanv = Instantiate(battleC.damageCanvas, damSource.transform.position, damSource.transform.rotation);
                damCanv.transform.LookAt(battleC.activeRoom.mainCam.transform);
                damCanv.activeCam = battleC.bCamController.activeCam;
                damCanv.damTXT.text = "MISS";
                damCanv.damTXT.color = Color.white;
                IEnumerator CanvasTimer()
                {
                    yield return new WaitForSeconds(2);
                    Destroy(damCanv.gameObject);
                } StartCoroutine(CanvasTimer());
            }           
        }
        if (damSource.actionType == ActionType.spell || damSource.actionType == ActionType.ranged || damSource.actionType == ActionType.item)
        {
            if (statusC.DEFboost > 0)
            {
                damage = damage - statusC.DEFboost;
                if (damage < 0)
                {
                    damage = 0;
                }
                statusC.ActivateDEF(false);
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


    void UnassignBindingAtPosition(int bindingIndex)
    {
        var timelineAsset = fireAllPlayable.playableAsset as TimelineAsset;
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
        fireAllPlayable.SetGenericBinding(track, null);
        Debug.Log($"Binding at position {bindingIndex} unassigned.");
    }

}
