using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MansBestTrinket : DunTrinket
{
    public SceneController controller;
    public MansBestFriend puppy;
    public PlayableDirector activationPlayable;
    public ParticleSystem dogBuff;

    public bool watching;


    public override void SetTrinket()
    {
        gameObject.SetActive(true);
        DistanceController distance = FindObjectOfType<DistanceController>();

        if (controller.dunFull)
        {
            foreach (FakeWall fWall in distance.fakeWalls)
            {
                MansBestFriend newPuppy = Instantiate(puppy, fWall.frontSpawnTransform);
                newPuppy.activeFakeWall = fWall;
                newPuppy.player = FindObjectOfType<PlayerController>();
                newPuppy.dogBuff = dogBuff;
                newPuppy.AssignCub(fWall);
            }
            StartCoroutine(PlayableTimer());
        }
        if (!controller.dunFull)
        {
            watching = true;
        }      
    }

    IEnumerator PlayableTimer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        if (uiController.shopUI.gameObject.activeSelf)
        {
            uiController.shopUI.CloseUI();
            uiController.CloseDunInventory();
            uiController.confirmUI.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(.25f);
        if (uiController.shopUI.gameObject.activeSelf)
        {
            uiController.shopUI.CloseUI();
        }
        activationPlayable.Play();
        float timer = (float)activationPlayable.duration;
        uiController.RemoteToggleTimer(timer);
        yield return new WaitForSeconds(timer);
        player.controller.enabled = true;
    }

    public override void ActivateTrinket()
    {

    }

    private void FixedUpdate()
    {
        if (watching)
        {
            if (controller.dunFull)
            {
                watching = false;
                SetTrinket();
            }
        }
    }
}
