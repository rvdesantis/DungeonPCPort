using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBattleItem : BattleItem
{
    public ParticleSystem healFX;
    public AudioSource audioSource;
    public AudioClip healSFX;

    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        healFX.gameObject.SetActive(true);
        healFX.Play();
        audioSource.PlayOneShot(healSFX);

        if (battleTarget != null)
        {
            battleTarget.Heal((int)itemEffect);
            if (battleTarget.statusC.poison)
            {
                battleTarget.statusC.poisonAmount = 0;
                battleTarget.statusC.poisonModel = null;
                battleTarget.statusC.statusCircleFX[5].gameObject.SetActive(false);
                battleTarget.statusC.statusCircleFX[5].Stop();
                battleTarget.statusC.poison = false;
            }
        }
    }

    public override void PickUp()
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        inventory.dungeonItems[0].itemCount++;
        Debug.Log(itemCount + " " + itemName + " picked up");

        DunUIController uiController = FindObjectOfType<DunUIController>();
        uiController.rangeImage.gameObject.SetActive(false);
        uiController.customImage.gameObject.SetActive(false);
        uiController.ToggleKeyUI(gameObject, false);
        uiController.pickUpUI.gameObject.SetActive(true);
        uiController.pickUpUI.OpenImage(this);
    }
}
