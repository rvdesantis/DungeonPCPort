using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MushroomDunNPC : DunNPC
{
    public PlayableDirector mushGrowPlayable;
    public bool growthTrig;
    public bool grown;
    public Button attackBT;
    public Button fMageAttackBT;
    public Button exitBT;
    public BoxCollider boxC;


    public override void NPCTrigger()
    {
        if (!grown && !engaged)
        {
            StartCoroutine(SlowGrow());
        }
        if (grown && !engaged)
        {
            OpenUI();
        }
    }

    public override void Confirm() // launch mush battle
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        BattleController battleC = uiController.controller.battleController;

        uiController.uiActive = false;
        uiController.controller.playerController.controller.enabled = true;

        uiObject.gameObject.SetActive(false);
        uiController.interactParent.SetActive(false);
        remove = true;
        battleC.SetBattle(19);
        gameObject.SetActive(false);
    }

    public void FireConfirm() // launch mush battle
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        BattleController battleC = uiController.controller.battleController;

        uiController.uiActive = false;
        uiController.controller.playerController.controller.enabled = true;
        uiObject.gameObject.SetActive(false);
        uiController.interactParent.SetActive(false);
        remove = true;
        battleC.SetBattle(20);
        gameObject.SetActive(false);
    }

    public bool FMageChecker()
    {
        bool inParty = false;
        PartyController party = FindObjectOfType<PartyController>();
        foreach (DunModel hero in party.activeParty)
        {
            if (hero.modelName == party.masterParty[0].modelName)
            {
                inParty = true;
                break;
            }
        }

        return inParty;
    }

    public override void OpenUI()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        engaged = true;
        uiController.interactParent.SetActive(false);
        uiController.uiActive = true;
        uiController.controller.playerController.controller.enabled = false;
        uiObject.SetActive(true);
        if (FMageChecker())
        {
            fMageAttackBT.gameObject.SetActive(true);
        }        
        attackBT.Select();
    }

    IEnumerator SlowGrow()
    {
        mushGrowPlayable.Play();
        boxC.enabled = true;
        yield return new WaitForSeconds(5);
        grown = true;

    }

    public void ExitBTAction()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        uiController.controller.playerController.controller.enabled = true;
        uiController.uiActive = false;
        uiController.interactParent.SetActive(false);

        uiObject.gameObject.SetActive(false);      
        IEnumerator EnabledTimer()
        {
            yield return new WaitForSeconds(1);
            engaged = false;
        }
        StartCoroutine(EnabledTimer());

    }


    private void Update()
    {
        if (inRange)
        {
            if (!growthTrig)
            {
                growthTrig = true;
                NPCTrigger();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && uiObject.gameObject.activeSelf)
        {
            exitBT.Select();
            ExitBTAction();
        }
    }
}
