using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GelCubeBattleScript : EnemyBattleModel
{
    public Transform gulpTransform;
    public BattleModel trappedPlayer;
    public bool trapped;
    public Quaternion startRot;
    public SkinnedMeshRenderer skinnedMesh;
    public Material normalMat;
    public List<Material> distortionMats; // fire, dark, poison, thunder, frozen

    public List<Spell> capturedSpells;



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
            if (trappedPlayer != null)
            {
                actionTarget = trappedPlayer;
                actionType = ActionType.melee;
                Attack(actionTarget);
                return;
            }
            if (trappedPlayer == null && capturedSpells.Count == 0)
            {
                SelectRandomTarget();   
                actionType = ActionType.melee;
                Attack(actionTarget);
                return;
            }
            if (trappedPlayer == null && capturedSpells.Count >0)
            {
                SelectRandomTarget();
                actionType = ActionType.spell;
                selectedSpell = capturedSpells[0]; 
                Cast(actionTarget, selectedSpell);          
                return;
            }
        }
    }

    public override void Cast(BattleModel target, Spell spell)
    {
        Debug.Log("Starting " + modelName + " Cast, Targeting " + target.modelName);
        battleC.enemyIndex++;
        StartCoroutine(CastTimer(target, selectedSpell));
    }

    IEnumerator CastTimer(BattleModel target, Spell spell)
    {
        transform.LookAt(target.transform);
        anim.SetTrigger("attack0");
        audioSource.PlayOneShot(actionSounds[1]);
        Spell newSpell = Instantiate(spell, target.transform);
        newSpell.targetTransform = target.transform;
        newSpell.CastSpell(newSpell.transform.position, target.transform.position);

        Material[] jellyMats = skinnedMesh.materials;
        jellyMats[0] = normalMat;
        skinnedMesh.materials = jellyMats;

        yield return new WaitForSeconds(newSpell.castTime);
        TriggerTargetHit();
        float damageAdjusted = (spellBonusPercent / 100f) + 1f;
        float damX = damageAdjusted * spell.baseDamage;
        int roundedDamage = Mathf.RoundToInt(damX);

        Debug.Log(modelName + " casting " + spell.spellName + " at " + target.modelName + " for " + roundedDamage + " damage");
        yield return new WaitForSeconds(1);
        target.TakeDamage(roundedDamage, this);
        afterAction.Invoke();
        afterAction = null;

        capturedSpells.RemoveAt(0);
        if (capturedSpells.Count > 0)
        {
            UpdateMat(capturedSpells[0]);
        }
    }

    public void UpdateMat(Spell spellSource)
    {
        Material[] jellyMats = skinnedMesh.materials;

        if (spellSource.spellType == Spell.SpellType.fire)
        {
            jellyMats[0] = distortionMats[0];
        }
        if (spellSource.spellType == Spell.SpellType.offense)
        {
            jellyMats[0] = normalMat;
        }
        if (spellSource.spellType == Spell.SpellType.voidMag)
        {
            jellyMats[0] = distortionMats[1];
        }
        if (spellSource.spellType == Spell.SpellType.nature)
        {
            jellyMats[0] = distortionMats[2];
        }
        if (spellSource.spellType == Spell.SpellType.thunder)
        {
            jellyMats[0] = distortionMats[3];
        }
        if (spellSource.spellType == Spell.SpellType.ice)
        {
            jellyMats[0] = distortionMats[4];
        }
        skinnedMesh.materials = jellyMats;
    }

    public override void GetHit(BattleModel modelSource)
    {
        audioSource.PlayOneShot(actionSounds[3]);
        base.GetHit(modelSource);
        if (modelSource.actionType == ActionType.spell)
        {
            Spell usedSpell = modelSource.selectedSpell;
            capturedSpells.Add(usedSpell);

            Material[] jellyMats = skinnedMesh.materials;

            if (usedSpell.spellType == Spell.SpellType.fire)
            {
                jellyMats[0] = distortionMats[0];
            }
            if (usedSpell.spellType == Spell.SpellType.offense)
            {
                jellyMats[0] = normalMat;
            }
            if (usedSpell.spellType == Spell.SpellType.voidMag)
            {
                jellyMats[0] = distortionMats[1];
            }
            if (usedSpell.spellType == Spell.SpellType.nature)
            {
                jellyMats[0] = distortionMats[2];
            }
            if (usedSpell.spellType == Spell.SpellType.thunder)
            {
                jellyMats[0] = distortionMats[3];
            }
            if (usedSpell.spellType == Spell.SpellType.ice)
            {
                jellyMats[0] = distortionMats[4];
            }
            skinnedMesh.materials = jellyMats;
        }
    }

    public override void IntroPlayable()
    {    
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        PartyController party = battleC.party;
        party.AssignCamBrain(battleStartPlayable, 3);

        foreach (BattleModel hero in battleC.heroParty)
        {
            hero.AssignBattleDirector(battleStartPlayable, 0, false, true);
        }
        actionTarget = battleC.heroParty[0];
        StartCoroutine(BattleIntroTimer());
    }

    IEnumerator BattleIntroTimer()
    {
        battleStartPlayable.Play();
        yield return new WaitForSeconds((float)battleStartPlayable.duration);
        afterAction.Invoke();
        afterAction = null;
        Gulp(actionTarget);
    }

    public override void Attack(BattleModel target)
    {
        Debug.Log("Starting " + modelName + " Attack, Targeting " + target.modelName);
        actionTarget = target;
        battleC.enemyIndex++;
        StartCoroutine(EnemyAttackTimer());
    }

    public override void Die(BattleModel damSource)
    {
        audioSource.PlayOneShot(actionSounds[2]);
        base.Die(damSource);

    }

    IEnumerator EnemyAttackTimer()
    {
        if (!dead)
        {
            int pos = battleC.heroParty.IndexOf(actionTarget);
            Vector3 attackPosition = battleC.activeRoom.enemySpawnPoints[pos].transform.position;
            Vector3 returnPos = transform.position;
            transform.position = attackPosition;

            Quaternion camRot = battleC.activeRoom.mainCam.transform.rotation;

            battleC.activeRoom.mainCam.LookAt = transform;

            
            anim.SetTrigger("attack1");
            audioSource.PlayOneShot(actionSounds[0]);
         
            yield return new WaitForSeconds(strikeTimer / 2);

            if (trappedPlayer!=null)
            {
                trappedPlayer.transform.position = actionTarget.spawnPoint;                
                trappedPlayer.transform.parent = battleC.activeRoom.playerSpawnPoints[pos].transform;
                trappedPlayer.transform.rotation = battleC.activeRoom.playerSpawnPoints[pos].transform.rotation;
                TriggerTargetHit();
                trappedPlayer = null;
            }
            battleC.activeRoom.mainCam.LookAt = null;
            battleC.activeRoom.mainCam.transform.rotation = camRot;

            yield return new WaitForSeconds(strikeTimer / 2);

            float defAdjusted = (float)actionTarget.defBonusPercent / 100f + 1f;
            float defX = defAdjusted * actionTarget.def;

            float powerAdjusted = (float)powerBonusPercent / 100f + 1f;
            float powerX = powerAdjusted * power;

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

    public void Gulp(BattleModel target)
    {
        target.transform.position = gulpTransform.position;
        target.transform.parent = gulpTransform;
        trappedPlayer = target;
    }

    private void Update()
    {
        if (trappedPlayer != null)
        {
            trappedPlayer.transform.position = gulpTransform.transform.position;       
        }
    }
}
