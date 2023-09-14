using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DunSwitch : MonoBehaviour
{
    public bool switchOn;
    public bool inRange;
    public bool locked;
    public enum AnimType { animator, playable}
    public AnimType animType;
    public Animator switchAnim;
    public PlayableDirector switchPlayable;

    public virtual void FlipSwitch()
    {
        if (!locked)
        {
            if (!switchOn)
            {
                if (animType == AnimType.animator)
                {

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

                }
                if (animType == AnimType.playable)
                {

                }
            }
        }
    }

    public virtual void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                FlipSwitch();
            }
        }
    }
}
