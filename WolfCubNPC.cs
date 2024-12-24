using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WolfCubNPC : DunNPC
{
    public PlayableDirector greeting;
    public DunItem cubTrinket;



    public override void NPCTrigger()
    {
        if (inRange && !opened)
        {
            StartCoroutine(GreetingTimer());
        }
    } 

    IEnumerator GreetingTimer()
    {
        opened = true;
        idlePlayableLoop.Stop();
        float timer = (float)greeting.duration;
        greeting.Play();
        yield return new WaitForSeconds(timer);
        greeting.Stop();
        idlePlayableLoop.Play();
        cubTrinket.PickUp();
    }
}
