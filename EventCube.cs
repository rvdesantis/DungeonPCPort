using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCube : MonoBehaviour
{
    public SanctuaryCube sanct;
    public GameObject treasureParent;
    public DunChest treasureChest;
    public DunPortal enterPortalSMRoom;
    public DunPortal exitPortalSMRoom;

    public void SetTreasureRoom(DunPortal fromPortal, bool closeOnJump = true)
    {
        DistanceController distance = FindObjectOfType<DistanceController>();
        treasureParent.SetActive(true);
    
        enterPortalSMRoom.ConnectPortals(fromPortal);

        if (!closeOnJump)
        {
            fromPortal.closeOnJump = false;
        }

        exitPortalSMRoom.ConnectPortals(sanct.returnPortal);

        distance.chests.Add(treasureChest);
        distance.portals.Add(exitPortalSMRoom);
    }

    public void ResetSmallRoom()
    {
        treasureParent.SetActive(false);
    }
}
