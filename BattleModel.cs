using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class BattleModel : DunModel
{
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

    public List<Spell> activeSpells;
    public List<Spell> masterSpells;

    public enum AttackType { melee, ranged, spell, item}
    public AttackType attackType;
    public int gold;
    public bool dead;

    public virtual void TakeDamage(int damage, BattleModel damSource)
    {

    }

    public virtual void Attack(BattleModel target)
    {

    }

    public virtual void Die(BattleModel damSource)
    {


        dead = true;
    }

    public virtual void Raise()
    {


        dead = false;
    }

    public virtual void Heal(int amount)
    {

    }

    public virtual void Cast(BattleModel target)
    {

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
            torch.SetActive(true);
        }

        if (weapon)
        {
            torch.SetActive(false);
            activeWeapon.SetActive(true);
        }
    }
}
