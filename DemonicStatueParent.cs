using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class DemonicStatueParent : RoomPropParent
{
    public StatueWeeper statueWeeper;
    public bool statueReady;
    public PlayableDirector doorOpenPlayable;

    public DunSwitch doorSwitch;
    public ArmorStandNPC armorStand;

    private void Start()
    {
        if (distanceController == null)
        {
            distanceController = FindObjectOfType<DistanceController>();
        }
        distanceController.switches.Add(doorSwitch);
    }



    public void TriggerCheck()
    {
        statueWeeper.ConnectStatue();
        statueWeeper.activated = true;
        statueWeeper.trackPlayer = true;
        Debug.Log("Statue Weeper now tracking player", statueWeeper.gameObject);
    }

    public override void EnvFill()
    {
        base.EnvFill();
        if (roomParent.roomType == CubeRoom.RoomType.battle || roomParent.roomType == CubeRoom.RoomType.quest)
        {
            statueWeeper.gameObject.SetActive(true);
            statueReady = true;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (!statueWeeper.activated && statueReady)
        {
            if (roomParent.inRoom)
            {
                TriggerCheck();
            }
        }
    }
}
