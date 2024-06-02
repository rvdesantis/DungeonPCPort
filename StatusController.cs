using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    public BattleModel attachedModel;
    public bool boost;
    public int boostAmount;
    public bool STR;
    public bool DEF;
    public bool burn;
    public bool freeze;
    public bool poison;
    public bool shock;
    public bool dark;
    public List<ParticleSystem> statusCircleFX; // boost - 0, STR - 1, DEF - 2, BURN - 3, FREEZE - 4, POISON - 5, SHOCK - 6, Dark - 7   

    public void ActivateBoost(bool activate)
    {
        if (activate)
        {
            boost = true;
            statusCircleFX[0].gameObject.SetActive(true);
            statusCircleFX[0].Play();
        }
        if (!activate)
        {
            boost = false;
            statusCircleFX[0].Stop();
            statusCircleFX[0].gameObject.SetActive(false);
            boostAmount = 0;
        }

    }

}
