
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
    public int mana;
    public int maxM;

    public int def;
    public int power;
    public int XP;
    public float powerBonusPercent;
    public float defBonusPercent;
    public float spellBonusPercent;
    public float critChance;

    public StatusController statusC;
    public Spell selectedSpell;
    public DunItem selectedItem;

    public List<Spell> activeSpells;
    public List<Spell> masterSpells;
    public GameObject spellSpawnPoint;
    public float strikeDistance;
    public float strikeTimer;
    public float hitbuffer;
    public Transform hitTarget;
    public ImpactFXController impactFX; 
    public CinemachineVirtualCamera attCam;

    public enum ActionType { melee, ranged, spell, item}
    public ActionType actionType;
    public BattleModel actionTarget;
    public int gold;
    public bool dead;
    public PlayableDirector battleStartPlayable;
    public Action afterAction;
    public bool skip;

    public List<CinemachineVirtualCamera> actionCams; // 0 - attack, 1 - cast, 2, item 
    public Vector3 spawnPoint;

    public List<AudioClip> actionSounds;

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
            if (actionTarget.dead)
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
            battleC.heroIndex++;
            afterAction.Invoke();
            afterAction = null;
        }
    }

    public virtual void TriggerTargetHit() // set on attack animation, or triggered in spell script
    {
        actionTarget.anim.SetTrigger("hit");
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

    public virtual void TakeDamage(int damage, BattleModel damSource)
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }


        DamageMSS damCan = Instantiate(battleC.damageCanvas, hitTarget.transform.position, Quaternion.identity);   
        damCan.activeCam = battleC.bCamController.activeCam;     

        damCan.ShowDamage(damage, gameObject);

  

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
        Vector3 returnPos = transform.position;

        transform.position = attackPosition;
        transform.LookAt(actionTarget.transform);
        if (attCam != null)
        {
            attCam.m_Priority = 20;
        }

        yield return new WaitForSeconds(.75f);

        int crit = UnityEngine.Random.Range(0, 100);
        if (crit >= critChance )
        {
            anim.SetTrigger("attack0");
        }
        if (crit < critChance)
        {
            Debug.Log(modelName + " CRIT HIT!");
            anim.SetTrigger("attack1");
        }

        yield return new WaitForSeconds(strikeTimer);

        if (attCam != null)
        {
            attCam.m_Priority = -1;
        }

        float defAdjusted = (float)target.defBonusPercent / 100f + 1f;
        float defX = defAdjusted * target.def;

        float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
        float powerX = powerAdjusted * power;

        int damageAmount = Mathf.RoundToInt(powerX) - Mathf.RoundToInt(defX);
        if (damageAmount < 0)
        {
            damageAmount = 0;
        }     

        Debug.Log(modelName + " attacking " + target.modelName + " for " + damageAmount + " damage");
        target.TakeDamage(damageAmount, this);

        transform.position = returnPos;
        
        yield return new WaitForSeconds(1.5f);
        battleC.heroIndex++;
        afterAction.Invoke();
        afterAction = null;
    }

    public virtual void Die(BattleModel damSource)
    {
        Debug.Log(modelName + " has Died", gameObject);
        anim.SetTrigger("dead");
        dead = true;
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

    public virtual void UseItem(BattleModel target, DunItem item)
    {
        anim.SetTrigger("item");
        StartCoroutine(ItemTimer(target, item));
    }

    IEnumerator ItemTimer(BattleModel target, DunItem item)
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
}
