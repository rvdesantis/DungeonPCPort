using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveBattlePotion : BattleItem
{
    public ParticleSystem reviveFX;
    public AudioSource audioSource;
    public AudioClip healSFX;

    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        ParticleSystem reviveFFX = Instantiate(reviveFX, battleTarget.transform.position, Quaternion.identity);
        reviveFFX.gameObject.SetActive(true);
        reviveFFX.Play();
        audioSource.PlayOneShot(healSFX);

        if (battleTarget != null)
        {
            battleTarget.dead = false;
            battleTarget.gameObject.SetActive(false);
            battleTarget.gameObject.SetActive(true);
            battleTarget.health = 0;
            battleTarget.Heal(battleTarget.maxH / 2);
            battleTarget.anim.SetTrigger("revive");
            battleTarget.skip = true;
            battleTarget.transform.LookAt(battleTarget.battleC.activeRoom.playerSpawnPoints[0].transform);
            IEnumerator RevivePositionTimer()
            {
                yield return new WaitForSeconds(2);
                battleTarget.transform.position = battleTarget.spawnPoint;
                battleTarget.transform.localRotation = new Quaternion(0, 0, 0, 0);
            } StartCoroutine(RevivePositionTimer());
        }
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
}
