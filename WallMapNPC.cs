using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMapNPC : DunNPC
{
    public InventoryController inventoryC;
    public MapItem mapItem;


    private void Start()
    {
        if (inventoryC == null)
        {
            SceneController controller = FindObjectOfType<SceneController>();
            inventoryC = controller.inventory;
        }
    }


    public override void NPCTrigger()
    {
        if (inRange && !opened)
        {
            if (fakeWall == null)
            {
                opened = true;
                if (audioSource != null)
                {
                    if (audioClips.Count > 0)
                    {
                        audioSource.PlayOneShot(audioClips[0]);
                        mapItem.PickUp();
                    }
                }
            }           
        }
    } // triggered from DistanceController


    private void FixedUpdate()
    {
        if (inventoryC.mapstatus == InventoryController.MapInventoryStatus.secret)
        {
            remove = true;
            opened = true;
        }
    }
}
