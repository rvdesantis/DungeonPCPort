using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TrapGhostTrinket : DunTrinket
{
    public SceneController controller;
    public TrapGhost ghost;
    private bool watching;
    public List<TrapGhost> activeGhosts;

    public PlayableDirector intro;
    
    public override void SetTrinket()
    {
        gameObject.SetActive(true);
        DistanceController distance = FindObjectOfType<DistanceController>();

        if (controller.dunFull)
        {
            foreach (FakeFloor fFloor in distance.fakeFloors)
            {
                TrapGhost newGhost = Instantiate(ghost, fFloor.spawnPoint);
                newGhost.activeFakeFloor = fFloor;
                newGhost.idleLoopDirector.Play();
                newGhost.player = controller.playerController;
                activeGhosts.Add(newGhost);

            }
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
            float timer = (float)intro.duration;
            uiController.RemoteToggleTimer(timer);

            intro.Play();
            if (uiController.shopUI.gameObject.activeSelf)
            {
                uiController.shopUI.CloseUI();
            }
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
