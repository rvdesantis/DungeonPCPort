using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class BossCube : Cube
{
    public SceneController controller;
    public HallStarterCube bossHallStarter;
    public PlayableDirector hallwayEnter;
    public DunModel bossModel;
    public bool rangeTrigger;

    public GameObject doorObj;
    public bool doorBool;
    public ConfirmUI confirmUI;
    public bool bossTrigger;
    public PlayableDirector bossStartPlayable;

    public Transform afterBattleSpawn;
    public FinalChest finalChest;

    IEnumerator RangeTimer()
    {
        hallwayEnter.Play();
        yield return new WaitForSeconds((float)hallwayEnter.duration);
        bossModel.anim.SetTrigger("idleBreak"); // for lesser red dragon
    }

    public virtual void StartBossBattle()
    {
        BattleController battleC = FindObjectOfType<BattleController>();

        Debug.Log("Starting Boss Battle (TEST)");
    }

    public void OpenDoorConfirm()
    {
        Debug.Log("Starting Door Confirm");
        controller.playerController.enabled = false;
        controller.distance.chests.Add(finalChest);
        confirmUI.gameObject.SetActive(true);
        confirmUI.targetAction = null;
        confirmUI.targetAction = ConfirmBossBattle;
        confirmUI.ConfirmMessageUI("Enter Boss Room?", false, false, false, false, false, true);
    }

    public virtual void ConfirmBossBattle()
    {
        StartCoroutine(BossRoomStartTimer());
    }

    IEnumerator BossRoomStartTimer()
    {
        Debug.Log("Starting Boss Battle");


        PartyController party = FindObjectOfType<PartyController>();


        controller.activePlayable = bossStartPlayable;
        controller.endAction += StartBossBattle;

        party.AssignCamBrain(bossStartPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(bossStartPlayable);
            model.transform.position = bossStartPlayable.transform.position;
            model.transform.parent = bossStartPlayable.transform;
            model.gameObject.SetActive(true);
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
        }
        bossModel.AssignToDirector(bossStartPlayable, 4);
        bossTrigger = true;
        bossStartPlayable.Play();
        yield return new WaitForSeconds((float)bossStartPlayable.duration);
        StartBossBattle();
    }

    private void Update()
    {
        if (!rangeTrigger && positioner.activeSelf)
        {
            if (controller == null)
            {
                controller = FindObjectOfType<SceneController>();
            }
            if (confirmUI == null)
            {
                confirmUI = controller.uiController.confirmUI;
            }
            Vector3 playerPosition = controller.playerController.transform.position;
       
            if (Vector3.Distance(playerPosition, bossHallStarter.generatedHallway[2].transform.position) < 5 && controller.playerController.controller.enabled)
            {
                rangeTrigger = true;
                StartCoroutine(RangeTimer());
            }
        }
        if (rangeTrigger)
        {
            Vector3 playerPosition = controller.playerController.transform.position;
            if (Vector3.Distance(playerPosition, doorObj.transform.position) < 5 && !doorBool)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    doorBool = true;
                    OpenDoorConfirm();
                }
            }
            if (Vector3.Distance(playerPosition, doorObj.transform.position) >25 && doorBool && !bossTrigger)
            {
                doorBool = false;
            }
        }
    }
}
