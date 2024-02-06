using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenForge : HiddenEndCube
{
    public DunNPC forgeNPC;

    private void Start()
    {
        if (distanceC == null)
        {
            distanceC = FindObjectOfType<DistanceController>();
        }
        distanceC.npcS.Add(forgeNPC);
        distanceC.fakeWalls.Add(fakeWall);
    }
}
