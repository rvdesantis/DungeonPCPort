using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushWallDunTrap : DunTrap
{
    public MushroomDunNPC mushNPC;


    public override void SetSideTrap(SideExtenderCube sideCube)
    {
        base.SetSideTrap(sideCube);
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        mushNPC.gameObject.SetActive(true);
        distance.npcS.Add(mushNPC); // trap triggers from NPC, not DunTrap script
    }
}
