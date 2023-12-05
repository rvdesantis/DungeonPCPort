using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float powerBonusInt;

    public float defBonusPercent;
    public float defBonusInt;

    public float spellBonusPercent;
    public float spellBonusInt;

    public List<Spell> activeSpells;
    public List<Spell> masterSpells;
}
