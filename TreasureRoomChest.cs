using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoomChest : DunChest
{
    public DunPortal roomExitPortal;



    public override void OpenChest()
    {
        base.OpenChest();
        roomExitPortal.gameObject.SetActive(true);
    }
}
