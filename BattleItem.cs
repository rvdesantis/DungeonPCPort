using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleItem : DunItem
{
    public enum BattleTarget { heroes, enemies, all, dead}
    public BattleTarget itemTarget;
}
