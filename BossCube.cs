using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class BossCube : Cube
{
    public SceneController controller;
    public HallStarterCube bossHallStarter;
    public PlayableDirector hallwayEnter;
    public DunModel bossModel;
    public bool rangeTrigger;

    IEnumerator RangeTimer()
    {
        hallwayEnter.Play();
        yield return new WaitForSeconds((float)hallwayEnter.duration);
        bossModel.anim.SetTrigger("idleBreak"); // for lesser red dragon
    }

    private void Update()
    {
        if (!rangeTrigger)
        {
            if (controller == null)
            {
                controller = FindObjectOfType<SceneController>();
            }
            Vector3 playerPosition = controller.playerController.transform.position;
       
            if (Vector3.Distance(playerPosition, bossHallStarter.generatedHallway[2].transform.position) < 5)
            {
                rangeTrigger = true;
                StartCoroutine(RangeTimer());
            }
        }
    }
}
