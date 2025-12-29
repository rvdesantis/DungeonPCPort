using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrinketItem : DunItem
{
    public AudioClip activateClip;

    public override void PickUp()
    {
        InventoryController inventory = FindAnyObjectByType<InventoryController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunTrinket randomT = null;

        List<DunTrinket> availTrinkets = new List<DunTrinket>();
        AudioSource audioSource = controller.uiController.uiAudioSource;

        if (itemType == ItemType.dungeon)
        {

            foreach (DunTrinket dunTrink in inventory.trinketC.masterDunTrinkets)
            {
                bool activeT = false;
                if (inventory.trinketC.activeDunTrinkets.Count > 0)
                {
                    foreach (DunTrinket aTrunk in inventory.trinketC.activeDunTrinkets)
                    {
                        if (aTrunk == dunTrink)
                        {
                            activeT = true;
                        }
                    }
                }
                if (!activeT)
                {
                    availTrinkets.Add(dunTrink);
                }
            }

            if (availTrinkets.Count > 0)
            {
                randomT = availTrinkets[Random.Range(0, availTrinkets.Count)];
                inventory.trinketC.activeDunTrinkets.Add(randomT);
                randomT.gameObject.SetActive(true);
                randomT.SetTrinket();
                inventory.ReduceGold(itemPrice);
                audioSource.PlayOneShot(activateClip);

            }
        }
        if (itemType == ItemType.battle)
        {
            foreach (DunTrinket dunTrink in inventory.trinketC.masterBattleTrinkets)
            {
                bool activeT = false;
                if (inventory.trinketC.activeBattleTrinkets.Count > 0)
                {
                    foreach (DunTrinket aTrunk in inventory.trinketC.activeBattleTrinkets)
                    {
                        if (aTrunk == dunTrink)
                        {
                            activeT = true;
                        }
                    }
                }
                if (!activeT)
                {
                    availTrinkets.Add(dunTrink);
                    inventory.ReduceGold(itemPrice);
                    audioSource.PlayOneShot(activateClip);
                }
            }

            if (availTrinkets.Count > 0)
            {
                randomT = availTrinkets[Random.Range(0, availTrinkets.Count)];
                inventory.trinketC.activeBattleTrinkets.Add(randomT);
                randomT.gameObject.SetActive(true);
                randomT.SetTrinket();
                inventory.ReduceGold(itemPrice);
                audioSource.PlayOneShot(activateClip);
            }
        }
    }
}
