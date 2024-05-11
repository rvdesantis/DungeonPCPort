using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBattleItem : DunItem
{
    public ParticleSystem healFX;
    public AudioSource audioSource;
    public AudioClip healSFX;

    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        healFX.gameObject.SetActive(true);
        healFX.Play();
        audioSource.PlayOneShot(healSFX);

        if (battleTarget != null)
        {
            battleTarget.Heal((int)itemEffect);
        }
    }
}
