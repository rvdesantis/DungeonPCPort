using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ArmorStandNPC : BlacksmithNPC
{
    public bool triggered;
    public bool assembled;
    public PlayableDirector startPlayable;
    public PlayableDirector idlePlayable;
    public PlayableDirector collapsePlayable;
    public DemonicStatueParent statueRoomParent;

    public override void NPCTrigger()
    {
        DunUIController uicontroller = FindObjectOfType<DunUIController>();
        BlacksmithUI blackUI = uicontroller.blackSmithUI;
        if (uiObject = null)
        {           
            uiObject = blackUI.gameObject;
        }
        if (!triggered)
        {
            triggered = true;
            Debug.Log("assembling Armor", gameObject);
            StartCoroutine(Assemble());
        }
        if (assembled)
        {
            blackUI.OpenSmithUI(false, true, multiplier, this);
        }
    }

    IEnumerator Assemble()
    {   
        startPlayable.Play();
        yield return new WaitForSeconds((float)startPlayable.duration);
        idlePlayable.Play();
        assembled = true;
    }

}
