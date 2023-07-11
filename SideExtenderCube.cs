using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideExtenderCube : Cube
{
    public enum SideType { enemy, chest, NPC, other, empty}
    public SideType sideType;
    public FakeWall lFakeWall;
    public FakeWall rFakeWall;
    public GameObject lSmall;
    public GameObject rSmall;
    public GameObject lSpawn;
    public GameObject rSpawn;

    public DunChest lChest;
    public DunChest rChest;

    public void SetWalls(bool left = true)
    {
        DistanceController distanceC = FindObjectOfType<DistanceController>();
        if (left)
        {
            foreach(GameObject wall in lWalls)
            {
                wall.SetActive(false);
            }
            lFakeWall.gameObject.SetActive(true);
            lSmall.SetActive(true);
            distanceC.fakeWalls.Add(lFakeWall);
            if (sideType == SideType.chest)
            {
                distanceC.chests.Add(lChest);
            }
        }
        else
        {
            foreach (GameObject wall in rWalls)
            {
                wall.SetActive(false);
            }
            rFakeWall.gameObject.SetActive(true);
            rSmall.SetActive(true);
            distanceC.fakeWalls.Add(rFakeWall);
            if (sideType == SideType.chest)
            {
                distanceC.chests.Add(rChest);
            }
        }
    }
    public void FillCube()
    {

    }
    // Start is called before the first frame update

}
