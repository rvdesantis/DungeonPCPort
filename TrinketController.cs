using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinketController : MonoBehaviour
{
    public SceneController controller;
    public DistanceController distance;
    public InventoryController inventory;

    public List<DunItem> activeDunTrinkets;
    public List<DunItem> activeBattleTrinkets;
    public List<DunItem> masterDunTrinkets;
    public List<DunItem> masterBattleTrinkets;

    public List<BattleTrinket> masterBattleTs;
    public ChaosOrbTrinket staticChaosOrbTrinket; // not in master list since C Orbs are a Key Item, and not in the master Battle Trinket List


}
