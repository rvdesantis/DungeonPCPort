using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalystPowderBattleItem : BattleItem
{
    public ParticleSystem FX;
    public AudioSource audioSource;
    public AudioClip SFX;

    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        FX.gameObject.SetActive(true);
        FX.Play();
        audioSource.PlayOneShot(SFX);
    }
}
