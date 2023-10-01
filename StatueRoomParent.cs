using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueRoomParent : RoomPropParent
{
    public Transform crystalParent;
    public Transform floorGrate;
    public Transform roofGrate;

    public float rotationSpeed = 10f;

    public DunSwitch startSwitch;
    public List<DunSwitch> wallSwitches;
    public DunSwitch swapSwitchN;
    public DunSwitch swapSwitchS;

    public DunItem chaosOrb;
    public List<GameObject> wallCovers;


    private void Start()
    {
        RoomSetUp();
        StartCoroutine(RotateObjects());
    }

    public void RoomSetUp() // does not add wall switches to distance controller until activated, only start switch.
    {
        foreach (GameObject wallCov in roomParent.wallCovers)
        {
            if (wallCov.activeSelf)
            {
                int x = roomParent.wallCovers.IndexOf(wallCov);
                wallCovers[x].SetActive(true);
            }
        }
        // add a check for switch room activation, set to default for testing

        foreach(DunSwitch sw in wallSwitches)
        {
            sw.locked = true;
            sw.switchAnim.SetTrigger("switchOn");
        }
        swapSwitchN.locked = true;
        swapSwitchN.switchAnim.SetTrigger("switchOn");
        swapSwitchS.locked = true;
        swapSwitchS.switchAnim.SetTrigger("switchOn");

        DistanceController distance = FindObjectOfType<DistanceController>();
        distance.switches.Add(startSwitch);
    }

    private IEnumerator RotateObjects()
    {
        while (true)
        {
            crystalParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            floorGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            roofGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
