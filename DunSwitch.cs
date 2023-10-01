using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DunSwitch : MonoBehaviour
{
    public bool switchOn;
    public bool inRange;
    public bool locked;
    public bool flipping;
    public enum AnimType { animator, playable}
    public AnimType animType;
    public Animator switchAnim;
    public PlayableDirector switchPlayable;

    public IEnumerator FlipTimer()
    {
        yield return new WaitForSeconds(.5f);
        flipping = false;
    }

    public virtual void FlipSwitch()
    {
        if (!locked && !flipping)
        {
            if (!switchOn)
            {
                if (animType == AnimType.animator)
                {
                    switchOn = true;
                    flipping = true;
                    switchAnim.SetTrigger("switchOn");
                    StartCoroutine(FlipTimer());
                }
                if (animType == AnimType.playable)
                {

                }
                return;
            }
            if (switchOn)
            {
                if (animType == AnimType.animator)
                {
                    switchAnim.SetTrigger("switchOff");
                    switchOn = false;
                    flipping = true;
                    StartCoroutine(FlipTimer());
                }
                if (animType == AnimType.playable)
                {

                }
            }
        }
    }

    public virtual void Update()
    {
        if (inRange && !flipping)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                FlipSwitch();
            }
        }
    }
}
