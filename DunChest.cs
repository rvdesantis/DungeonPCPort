using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DunChest : MonoBehaviour
{
    public DunItem chestItem;
    public enum AnimType { animator, playable}
    public AnimType animType;
    public Animator anim;
    public PlayableDirector openPlayable;
    public bool opened;
    public bool inRange;
    public bool cursed;
    public bool locked;
    public AudioSource audioSource;
    public List<AudioClip> audioClips; // 0 - open, 1 - locked, 2 - treasure sound (coin, etc)
    public FakeWall fakeWall;
    public bool fixedTreasure; 



    public IEnumerator OpenSequence()
    {
        DunUIController uiController = FindObjectOfType<DunUIController>();
        InventoryController inventory = FindFirstObjectByType<InventoryController>();

        float waitTime = 0;
        if (!fixedTreasure)
        {
            chestItem = inventory.randomALLItem.DunItemShuffle();      

            if (chestItem.itemType == DunItem.ItemType.gold)
            {
                chestItem.itemCount = Random.Range(50, 250);
            }
            if (chestItem.itemType == DunItem.ItemType.XP)
            {
                chestItem.itemCount = Random.Range(50, 100);
            }
            if (chestItem.itemType != DunItem.ItemType.gold && chestItem.itemType != DunItem.ItemType.XP)
            {
                chestItem.itemCount = 1;
            }
        }
        opened = true; 
        if (animType == AnimType.animator)
        {
            waitTime = .5f;
            if (anim != null)
            {
                anim.SetTrigger("openLid");
            }
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClips[0]);
            }
        }
        if (animType == AnimType.playable)
        {
            waitTime = (float)openPlayable.duration;
            if (openPlayable != null)
            {
                openPlayable.Play();
            }
        }
        yield return new WaitForSeconds(waitTime);
        if (audioClips.Count >= 3)
        {
            if (audioClips[2] != null)
            {
                audioSource.PlayOneShot(audioClips[2]);
            }
        }
        uiController.ToggleKeyUI(gameObject, false);
        uiController.pickUpUI.gameObject.SetActive(true);
        uiController.pickUpUI.OpenImage(chestItem);
        uiController.pickUpUI.afterAction = chestItem.PickUp;
    }

    public virtual void OpenChest()
    {
        if (inRange && !opened && !locked)
        {      
            if (fakeWall == null)
            {
                StartCoroutine(OpenSequence());
            }
            if (fakeWall != null)
            {
                if (fakeWall.wallBroken)
                {
                    StartCoroutine(OpenSequence());
                }
            }
        }
        if (inRange && !opened && locked)
        {
            audioSource.PlayOneShot(audioClips[1]);
        }
    }

   
}
