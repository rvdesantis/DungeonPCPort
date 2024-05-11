using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenEndCube : DeadEndCube
{
    public DunChest chest;
    public DunPortal secretPortal;
    public DunNPC npc;
    public enum SecretSize { end, room, massive}
    public SecretSize secretSize;

    void Start()
    {
        if (distanceC == null)
        {
            distanceC = FindObjectOfType<DistanceController>();
        }
        if (fakeWall != null)
        {
            if (fakeWall.gameObject.activeSelf)
            {
                distanceC.fakeWalls.Add(fakeWall);
            }
        }  
        if (chest !=null)
        {
            distanceC.chests.Add(chest);
        }
        if (npc !=null)
        {
            distanceC.npcS.Add(npc);
        }
    }

}
