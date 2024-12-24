using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class MimicTrinket : DunTrinket
{
    public SceneController controller;
    private bool watching;
    public PlayableDirector intro;

    public override void SetTrinket()
    {
        DistanceController distance = FindObjectOfType<DistanceController>();
        foreach (DunChest chest in distance.chests)
        {
            if (chest.GetComponent<DunMimic>() != null)
            {
                chest.GetComponent<DunMimic>().mimicIndicator.SetActive(true);
            }
        }
        gameObject.SetActive(true);
        if (controller.dunFull)
        {
            StartCoroutine(PlayableTimer());
        }
        if (!controller.dunFull)
        {
            watching = true;
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
            intro.Play();
            if (uiController.shopUI.gameObject.activeSelf)
            {
                uiController.shopUI.CloseUI();
            }
            float timer = (float)intro.duration;
            uiController.RemoteToggleTimer(timer);
            yield return new WaitForSeconds(timer);
            player.controller.enabled = true;
        }
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
