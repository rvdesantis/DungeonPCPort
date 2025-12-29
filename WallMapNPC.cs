using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class WallMapNPC : DunNPC
{
    public InventoryController inventoryC;
    public MapItem mapItem;


    private void Start()
    {
        if (inventoryC == null)
        {
            SceneController controller = FindAnyObjectByType<SceneController>();
            inventoryC = controller.inventory;
        }
    }


    public override void NPCTrigger()
    {
        if (inRange && !opened)
        {
            DunUIController uiController = FindAnyObjectByType<DunUIController>();
            opened = true;
            audioSource.PlayOneShot(audioClips[0]);   ;
            uiController.ToggleKeyUI(gameObject, false);
            uiController.pickUpUI.gameObject.SetActive(true);
            uiController.pickUpUI.OpenImage(mapItem);
            uiController.pickUpUI.afterAction = mapItem.PickUp;
        }
    } // triggered from DistanceController


    private void FixedUpdate()
    {
        if (inventoryC.mapstatus == InventoryController.MapInventoryStatus.secret && !remove)
        {
            remove = true;
            opened = true;
        }
    }
}
