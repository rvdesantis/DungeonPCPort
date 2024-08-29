using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrinket : MonoBehaviour
{
    public BattleController battleC;
    public InventoryController inventory;
    public BattlePhaseUI phase;

    public float battleDelay;
    public int effectAmount;
    public enum BattlePhase { start, select,preEnemy, }
    public BattlePhase combatPhase;

    public virtual void ActiveBattleTrinket()
    {

    }


}
