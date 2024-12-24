using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ReviveDunItem : DunItem
{
    public PartyController party;
    
    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        party = FindObjectOfType<PartyController>();
        MessagePanelUI messageUI = FindObjectOfType<DunUIController>().messagePanelUI;
       

        int deadCount = 0;
        foreach (int health in party.combatHealthTracker)
        {
            if (health == 0)
            {
                deadCount++;
            }
        }
        if (deadCount == 0)
        {
            string message = "No Dead Heroes, cannot use Revive";
            messageUI.gameObject.SetActive(true);
            messageUI.OpenMessage(message);
            IEnumerator MessageTimer()
            {
                yield return new WaitForSeconds(3);
                messageUI.gameObject.SetActive(false);
            }StartCoroutine(MessageTimer());
        }
        if (deadCount > 0)
        {
            StartCoroutine(UseTimer());
        }       
    }

    IEnumerator UseTimer()
    {
        ConfirmUI confirmUI = FindObjectOfType<DunUIController>().confirmUI;
        PlayerController player = FindObjectOfType<PlayerController>();
        yield return new WaitForSeconds(.35f);
        Debug.Log("Revive - Checking Party for 0 health");
        foreach (int health in party.combatHealthTracker)
        {
            int x = party.combatHealthTracker.IndexOf(health);
            Debug.Log("Checking " + party.activeParty[x].modelName + " ");

            int y = health;
            Debug.Log(party.activeParty[x].modelName + " healh set to " + y);
            if (y == 0)
            {
                Debug.Log("Setting up Confirm UI");

                string confirmMes = "Use Revive to return defeated heroes to your Party?";
                confirmUI.gameObject.SetActive(true);
                confirmUI.ConfirmItem(confirmMes, this);
                break;
            }
        }
        player.vfxLIST[1].gameObject.SetActive(true);
        player.vfxLIST[1].Play();
    }

    public override void PickUp()
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        bool inList = false;
        foreach (DunItem dunItem in inventory.dungeonItems)
        {
            if (dunItem == this)
            {
                dunItem.itemCount = dunItem.itemCount + 1;
                inList = true;
                inventory.masterBattleItems[2].itemCount = dunItem.itemCount;
                gameObject.SetActive(false);
                break;
            }
        }

        if (!inList)
        {
       
            foreach (DunItem battleItem in inventory.masterDungeonItems)
            {
                if (battleItem.itemName == itemName)
                {
                    inventory.dungeonItems.Add(battleItem);
                    BattleItem battleComponent = battleItem.GetComponent<BattleItem>();
                    inventory.battleItems.Add(inventory.masterBattleItems[2]);
                    battleItem.itemCount = 1;
                    inventory.masterBattleItems[2].itemCount = 1;
                }
            }
            gameObject.SetActive(false);
        }
    }


    public override void ConfirmItem()
    {
        Debug.Log("Using Revive (DunItem)");
        PartyController party = FindObjectOfType<PartyController>();
        int pos = 0;
        foreach (int health in party.combatHealthTracker)
        {
            if (health == 0)
            {
                pos = party.combatHealthTracker.IndexOf(health);     
                int healAmount = Mathf.RoundToInt(party.combatParty[pos].maxH / 2);
                party.combatHealthTracker[pos] = healAmount;
                Debug.Log("Setting " + party.activeParty[pos].modelName + " health to " + healAmount);
            }
        }
    }

}
