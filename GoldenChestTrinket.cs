using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GoldenChestTrinket : DunTrinket
{
    public InventoryController inventory;
    public SceneController controller;
    private bool watching;
    public PlayableDirector intro;
    public static bool goldenChestActive = false;

    public override void SetTrinket()
    {
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
            goldenChestActive = true;

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
