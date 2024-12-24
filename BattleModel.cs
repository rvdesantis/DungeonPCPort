
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class BattleModel : DunModel
{
    public BattleController battleC;
    public int health;
    public int maxH;

    public int def;
    public int power;
    public int XP;
    public float powerBonusPercent;
    public float defBonusPercent;
    public float spellBonusPercent;
    public float critChance;

    public StatusController statusC;
    public Spell selectedSpell;
    public BattleItem selectedItem;

    public List<Spell> activeSpells;
    public List<Spell> masterSpells;
    public GameObject spellSpawnPoint;
    public float strikeDistance;
    public float strikeTimer;
    public float hitbuffer;
    public float verticalHitBuffer = 0;
    public Transform hitTarget;
    public ImpactFXController impactFX; 
    public CinemachineVirtualCamera attCam;

    public enum ActionType { melee, ranged, spell, item}
    public ActionType actionType;
    public BattleModel actionTarget;
    public int gold;
    public bool dead;
    public bool pHolder;
    public PlayableDirector battleStartPlayable;
    public Action afterAction;
    public bool skip;

    public List<CinemachineVirtualCamera> actionCams; // 0 - attack, 1 - cast, 2, item 
    public Vector3 spawnPoint;

    public List<AudioClip> actionSounds;
    public List<GameObject> weaponAuras;

    

    public virtual void IntroPlayable()
    {
        if (battleStartPlayable != null)
        {

        }
        afterAction.Invoke();
        afterAction = null;
    }

    public virtual void IntroAction()
    {

    }

    public virtual bool DeadEnemiesCheck()
    {
        bool allDead = false;
        int deadEnemies = 0;

        foreach (BattleModel en in battleC.enemyParty)
        {
            if (en.dead)
            {
                deadEnemies++;
            }
        }
        
        if (deadEnemies == 3)
        {
            allDead = true;
        }

        return allDead;

    }

    public virtual void StartAction()
    {
        Debug.Log("Starting " + modelName + " Action");
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        if (battleC.heroIndex == 0) // works for Hero side
        {
            afterAction = null;
            afterAction = battleC.heroParty[1].StartAction;
            Debug.Log(modelName + " afterAction set to action " + battleC.heroParty[1]);
        }
        if (battleC.heroIndex == 1)
        {
            afterAction = null;
            afterAction = battleC.heroParty[2].StartAction;
            Debug.Log(modelName + " afterAction set to action " + battleC.heroParty[2]);
        }
        if (battleC.heroIndex == 2)
        {
            afterAction = null;
            afterAction = battleC.StartPostHeroTimer;
            Debug.Log(modelName + " afterAction set to StartPostHeroTimer(BC)");
        }
        if (!skip && !dead && !DeadEnemiesCheck())
        {
            if (actionTarget == null)
            {
                actionTarget = battleC.enemyParty[0];               
            }
            if (actionTarget.dead && actionType != ActionType.item)
            {
                foreach (BattleModel enemy in battleC.enemyParty)
                {
                    if (!enemy.dead)
                    {
                        actionTarget = enemy;
                        break;
                    }
                }
            }
            if (actionType == ActionType.melee)
            {
                Attack(actionTarget);
            }
            if (actionType == ActionType.spell)
            {                
                Cast(actionTarget, selectedSpell);
            }
            if (actionType == ActionType.item)
            {
                UseItem(actionTarget, selectedItem);
            }
        }
        if (skip || dead || DeadEnemiesCheck())
        {
            skip = false;
            anim.SetBool("injured", false);
            battleC.heroIndex++;
            afterAction.Invoke();
            afterAction = null;
        }
    }
    public void TimelineHit()
    {
        anim.SetTrigger("hit");
    }
    public virtual void GetHit(BattleModel modelSource)
    {
        if (!dead)
        {
            anim.SetTrigger("hit");
        }
    }
    public virtual void TriggerTargetHit() // set on attack animation, or triggered in spell script
    {
        actionTarget.GetHit(this);
        if (actionType == ActionType.melee)
        {
            actionTarget.impactFX.StandardImpact();
            if (actionSounds.Count > 0)
            {
                if (actionSounds[0] != null)
                {
                    audioSource.PlayOneShot(actionSounds[0]);
                }
            }
            statusC.statusCircleFX[0].Stop();
        }
        if (actionType == ActionType.spell)
        {
            if (selectedSpell.spellType == Spell.SpellType.fire)
            {
                actionTarget.impactFX.ElementalImpact(0);
            }
            if (selectedSpell.spellType == Spell.SpellType.voidMag)
            {
                actionTarget.impactFX.ElementalImpact(1);
            }
            if (selectedSpell.spellType == Spell.SpellType.nature)
            {
                actionTarget.impactFX.ElementalImpact(2);
            }
            if (selectedSpell.spellType == Spell.SpellType.ice)
            {
                actionTarget.impactFX.ElementalImpact(3);
            }
        }
    }

    public virtual void TakeDamage(int damage, BattleModel damSource, bool crit = false)
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        DamageMSS damCan = Instantiate(battleC.damageCanvas, hitTarget.transform.position, Quaternion.identity);   
        damCan.activeCam = battleC.bCamController.activeCam;

        if (damSource.actionType == ActionType.melee)
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
        }

        if (crit)
        {
            damCan.ShowDamage(damage, true);
        }
        if (!crit)
        {
            damCan.ShowDamage(damage);
        }
        // check for DEF Boost


        health = health - damage;
        if (health <= 0)
        {
            health = 0;
            Die(damSource);
        }
    }



    public virtual void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;        
        StartCoroutine(AttackTimer(target));
    }

    public IEnumerator AttackTimer(BattleModel target)
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

        if (attCam != null)
        {
            attCam.m_Priority = 20;
        }

        yield return new WaitForSeconds(.75f);

        anim.SetTrigger("attack0");

        yield return new WaitForSeconds(strikeTimer);

        if (attCam != null)
        {
            attCam.m_Priority = -1;
        }

        float defAdjusted = (float)target.defBonusPercent / 100f + 1f;
        float defX = defAdjusted * target.def;

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

        Debug.Log(modelName + " attacking " + target.modelName + " for " + damageAmount + " damage");
        if (powerStatusBoost > 0)
        {
            target.TakeDamage(damageAmount, this, true);
        }
        if (powerStatusBoost == 0)
        {
            target.TakeDamage(damageAmount, this);
        }

        transform.position = returnPos;
        
        yield return new WaitForSeconds(1.5f);
        statusC.ActivateBoost(false);
        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public virtual void Die(BattleModel damSource)
    {
        Debug.Log(modelName + " has Died", gameObject);
        anim.SetTrigger("dead");
        dead = true;
        statusC.ResetAll();
    }

    public virtual void Raise()
    {


        dead = false;
    }

    public virtual void Heal(int amount)
    {
        health = health + amount;
        if (health > maxH)
        {
            health = maxH;
        }
    }

    public virtual void Cast(BattleModel target, Spell spell)
    {
        actionTarget = target;
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        StartCoroutine(CastTimer(target, selectedSpell));
    }

     IEnumerator CastTimer(BattleModel target, Spell spell)
    {
        transform.LookAt(target.transform);
        anim.SetTrigger("spell0");
        yield return new WaitForSeconds(1);
        float damageAdjusted = (float)spellBonusPercent / 100f + 1f;
        int roundedDamage = Mathf.RoundToInt(damageAdjusted);

        Debug.Log(modelName + " casting " + spell.spellName + " at " + target.modelName + " for " + roundedDamage + " damage");
        target.TakeDamage(roundedDamage, this);

        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public virtual void UseItem(BattleModel target, BattleItem item)
    {
        anim.SetTrigger("item");
        Debug.Log(modelName + " using item, targeting " + target.modelName + " using " + item.itemName, gameObject);
        StartCoroutine(ItemTimer(target, item));
    }

    IEnumerator ItemTimer(BattleModel target, BattleItem item)
    {
        Debug.Log(modelName + " used " + item.itemName + " targeting " + target.modelName);
        DunItem usedItem = Instantiate(item, target.transform.transform.position, target.transform.rotation);
        if (!usedItem.gameObject.activeSelf)
        {
            usedItem.gameObject.SetActive(true);
        }
        usedItem.battleTarget = target;
        usedItem.UseItem(null, target);

        yield return new WaitForSeconds(2);
        item.itemCount--;
        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }
    public virtual void ActivateWeapon() // for use in animations in the unsheath anim
    {
        if (activeWeapon != null)
        {
            activeWeapon.gameObject.SetActive(true);
        }
    }

    public void AssignBattleDirector(PlayableDirector dir, int pos = 0, bool activeTorch = false, bool weapon = false)
    {
        BattleController battleC = FindObjectOfType<BattleController>();
        int posNum = 0;
        if (pos == 0)
        {
            posNum = battleC.heroParty.IndexOf(this);
        }
        if (pos > 2)
        {
            posNum = pos;
        }


        Debug.Log("Assigning " + modelName + " to position " + posNum);

        PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
        dir.SetGenericBinding(playableBinding.sourceObject, anim);

        if (activeTorch)
        {
            activeWeapon.SetActive(false);
            if (torch != null)
            {
                torch.SetActive(true);
            }
        }

        if (weapon)
        {
            if (torch != null)
            {
                torch.SetActive(false);
            }            
            activeWeapon.SetActive(true);
        }
    }

    public void WeaponAuraOff()
    {
        foreach (GameObject auras in weaponAuras)
        {
            auras.SetActive(false);
        }
    }
    public void WeaponAuraFire()
    {
        weaponAuras[0].SetActive(true);
    }

    public void WeaponAuraDark()
    {
        weaponAuras[1].SetActive(true);
    }

    public void WeaponAuraIce()
    {
        weaponAuras[3].SetActive(true);
    }

    public void WeaponAuraPoison()
    {
        weaponAuras[2].SetActive(true);
    }

    public void WeaponAuraLightning()
    {
        weaponAuras[4].SetActive(true);
    }

    public void ResetWeaponAuras()
    {
        foreach (GameObject obj in weaponAuras)
        {
            obj.SetActive(false);
        }
    }

    public void TimeLineBlankScript() // 
    {

    }

    public void TimelineFireHit()
    {
        actionTarget.GetHit(this);
        actionTarget.impactFX.ElementalImpact(0);
    }
}
