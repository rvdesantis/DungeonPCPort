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

    public void MapCheck()
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        if (inventory.mapstatus != InventoryController.MapInventoryStatus.secret)
        {
            chestItem = inventory.mapItemPrefab;
        }
    }

    public IEnumerator OpenSequence()
    {
        opened = true;
        if (animType == AnimType.animator)
        {
            if (anim != null)
            {
                anim.SetTrigger("openLid");
            }
            if (audioSource != null)
            {
                audioSource.PlayOneShot(audioClips[0]);
            }
            yield return new WaitForSeconds(.5f);
            if (chestItem != null)
            {
                chestItem.PickUp();
            }
            if (audioClips.Count >= 3)
            {
                if (audioClips[2] != null)
                {
                    audioSource.PlayOneShot(audioClips[2]);
                }
            }
        }
        if (animType == AnimType.playable)
        {
            if (openPlayable != null)
            {
                openPlayable.Play();
                yield return new WaitForSeconds((float)openPlayable.duration);
                if (chestItem != null)
                {
                    chestItem.PickUp();
                }
            }
        }

        DunUIController uiController = FindObjectOfType<DunUIController>();
        uiController.ToggleKeyUI(gameObject, false);
        uiController.pickUpUI.gameObject.SetActive(true);
        uiController.pickUpUI.OpenImage(chestItem);
    }

    public virtual void OpenChest()
    {
        if (inRange && !opened && !locked)
        {
            MapCheck();
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
