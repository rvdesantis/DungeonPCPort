using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    public BattleModel attachedModel;
    public bool boost;
    public bool burn;
    public bool freeze;
    public bool poison;
    public bool shock;
    public bool dark;
    public List<ParticleSystem> statusCircleFX;

}
