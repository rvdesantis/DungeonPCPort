using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePhaseUI : MonoBehaviour
{
    public BattleController battleC;
    public BattleUIController battleUI;
    public Image mainIcon;
    public List<Sprite> iconSprites;
    public List<Image> circleFills;
    public List<AudioClip> audioClips;
    public List<Animator> ringAnims;

    public void UpdateIcon()
    {
        if (battleC.phase == BattleController.BattlePhase.start)
        {
            if (mainIcon.sprite != iconSprites[0])
            {
                mainIcon.sprite = iconSprites[0];

                foreach (Animator anim in ringAnims)
                {
                    anim.SetBool("highLight", false);
                }
                ringAnims[0].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.select)
        {
            if (ringAnims[6].GetBool("highLight") == true)
            {
                ringAnims[6].SetBool("highLight", false);
            }
            if (mainIcon.sprite != iconSprites[1])
            {
                mainIcon.sprite = iconSprites[1];
                battleUI.audioSource.PlayOneShot(audioClips[0]);

                ringAnims[0].SetBool("highLight", false);
                ringAnims[1].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.preHero)
        {
            if (ringAnims[6].GetBool("highLight") == true)
            {
                ringAnims[6].SetBool("highLight", false);
            }
            if (mainIcon.sprite != iconSprites[2])
            {
                mainIcon.sprite = iconSprites[2];
                battleUI.audioSource.PlayOneShot(audioClips[0]);
                battleUI.audioSource.PlayOneShot(audioClips[1]);
                ringAnims[3].SetBool("highLight", false);
                ringAnims[4].SetBool("highLight", false);
                ringAnims[5].SetBool("highLight", false);
                ringAnims[6].SetBool("highLight", false);
                ringAnims[0].SetBool("highLight", false);
                ringAnims[1].SetBool("highLight", false);

                ringAnims[2].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.hero)
        {
            if (mainIcon.sprite != iconSprites[2])
            {
                mainIcon.sprite = iconSprites[2];                
            }
        }
        if (battleC.phase == BattleController.BattlePhase.afterHero)
        {
            if (mainIcon.sprite != iconSprites[3])
            {
                mainIcon.sprite = iconSprites[3];
                battleUI.audioSource.PlayOneShot(audioClips[0]);
                ringAnims[2].SetBool("highLight", false);
                ringAnims[3].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.preEnemy)
        {
            if (mainIcon.sprite != iconSprites[4])
            {
                mainIcon.sprite = iconSprites[4];
                battleUI.audioSource.PlayOneShot(audioClips[2]);

                ringAnims[3].SetBool("highLight", false);
                ringAnims[4].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.Enemy)
        {
            if (mainIcon.sprite != iconSprites[4])
            {
                mainIcon.sprite = iconSprites[4];               
            }
        }
        if (battleC.phase == BattleController.BattlePhase.afterEnemy)
        {
            if (mainIcon.sprite != iconSprites[5])
            {
                mainIcon.sprite = iconSprites[5];
                battleUI.audioSource.PlayOneShot(audioClips[0]);
                ringAnims[4].SetBool("highLight", false);
                ringAnims[5].SetBool("highLight", true);
            }
        }
        if (battleC.phase == BattleController.BattlePhase.endPhase)
        {
            if (mainIcon.sprite != iconSprites[6])
            {
                mainIcon.sprite = iconSprites[6];
                ringAnims[5].SetBool("highLight", false);
                ringAnims[6].SetBool("highLight", true);
            }
        }
    }

    

    private void Update()
    {
        UpdateIcon();
    }

}
