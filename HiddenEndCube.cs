using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenEndCube : DeadEndCube
{
    public DunChest chest;
    void Start()
    {
        if (distanceC == null)
        {
            distanceC = FindObjectOfType<DistanceController>();
            distanceC.fakeWalls.Add(fakeWall);
        }
        if (chest !=null)
        {
            distanceC.chests.Add(chest);
        }
    }

}
