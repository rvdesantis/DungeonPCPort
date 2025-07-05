using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplarMonkLeaderNPC : DunNPC
{
    public HiddenEndCube hiddenLibrary;
    public bool vMagic;
    public DunUIController uiController;
    public BattleController bController;
    public PartyController party;
    public DunModel vMage;
    public bool orbInInventory;
    public MessagePanelUI messagePanel;
    public bool isToggling;
    public string dialog;

    // Start is called before the first frame update
    void Start()
    {
        if (party == null)
        {
            party = FindObjectOfType<PartyController>();
        }
        if (uiController == null)
        {
            uiController = FindObjectOfType<DunUIController>();
        }
        foreach (DunModel hero in party.activeParty)
        {
            if (hero.modelName == vMage.modelName)
            {
                vMagic = true;
            }
        }
    }

    public override void NPCTrigger()
    {
        MonsterController monster = FindObjectOfType<MonsterController>();
       
        
        if (!vMagic)
        {
            Debug.Log("vMage not in party, attack not triggered.  Opening UI");
            OpenUI();
        }
    }

    public override void OpenUI()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<DunUIController>();
        }
        if (uiController != null)
        {  
            if (!vMagic && orbInInventory)
            {
                uiController.confirmUI.gameObject.SetActive(true);
                string message = "For a donation of 250G to the Templar order, we can provide you this strange Orb";
                message = message + "\n\n Give 250 Gold? ";

                uiController.confirmUI.message.text = message;
                uiController.confirmUI.targetAction = BuyOrb;
                uiController.confirmUI.noBT.Select();
                uiController.controller.playerController.controller.enabled = false;
            }
            if (!vMagic && !orbInInventory)
            {
                if (!isToggling)
                {
                    dialog = "Templar Monk Leader:\nThank you for your donation, you are truely a friend of the Templar Order.";
                    isToggling = true;
                    if (messagePanel == null)
                    {
                        messagePanel = FindObjectOfType<DunUIController>().messagePanelUI;
                    }
                    StartCoroutine(MessageTimer());
                }
            }
            if (vMagic && orbInInventory)
            {
                uiController.confirmUI.gameObject.SetActive(true);
                string message = "Void Mages are not welcome here.  Leave Immediately";
                message = message + "\n\n Attack Void Monks?";

                uiController.confirmUI.message.text = message;
                uiController.confirmUI.targetAction = SelectAttack;
                uiController.confirmUI.noBT.Select();
                uiController.controller.playerController.controller.enabled = false;
            }
        }
    }

    public void BuyOrb()
    {
        InventoryController inventory = uiController.controller.inventory;
        inventory.ReduceGold(250);
        DunItem orb = hiddenLibrary.items[0];
        orb.PickUp();
        orbInInventory = false;
        uiController.controller.playerController.controller.enabled = true;
        uiController.controller.distance.dunItems.Remove(orb);
    }

    public void SelectAttack()
    {
        StartCoroutine(SelectAttackTimer());
    }

    IEnumerator SelectAttackTimer()
    {
        yield return new WaitForSeconds(.5f);
        if (bController == null)
        {
            bController = FindObjectOfType<BattleController>();
        }
        bController.afterBattleAction = null;  // set after action
        bController.SetBattle(21);
    }

    IEnumerator MessageTimer()
    {
        messagePanel.OpenMessage(dialog);
        yield return new WaitForSeconds(3);
        if (messagePanel.currentString == dialog)
        {
            messagePanel.gameObject.SetActive(false);
        }
        isToggling = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
