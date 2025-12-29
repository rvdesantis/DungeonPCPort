using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansBestFriend : MonoBehaviour
{
    public FakeWall activeFakeWall;
    public PlayerController player;
    public SkinnedMeshRenderer skinnedMesh;
    public Animator anim;
    public bool lie;
    public bool howl;
    public bool idle;
    public bool hide;
    public bool triggered;

    public ParticleSystem dogBuff;
    public AudioSource audioSource;
    public List<AudioClip> clips;

    public void AssignCub(FakeWall fake)
    {
        if (fake.hideType == FakeWall.HideType.room)
        {
            lie = true;
        }
        if (fake.hideType == FakeWall.HideType.treasure)
        {
            howl = true;
        }
        if (fake.hideType == FakeWall.HideType.monster)
        {
            hide = true;
        }

        transform.LookAt(fake.transform);
    }

    public void TrackPlayer()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > 50)
        {
            if (skinnedMesh.enabled)
            {
                skinnedMesh.enabled = false;
                idle = false;
            }
        }
        if (distance > 25 && distance < 50 && !idle)
        {
            idle = true;
            anim.SetTrigger("idle");
            skinnedMesh.enabled = true;
        }
        if (distance < 10)
        {
            idle = false;
            
            if (lie && !triggered)
            {
                anim.SetTrigger("lie");
            }
            if (howl && !triggered)
            {
                anim.SetTrigger("howl");
                audioSource.PlayOneShot(clips[0]);
            }
            if (hide && !triggered)
            {
                anim.SetTrigger("hide");
                audioSource.PlayOneShot(clips[1]);
            }

            triggered = true;

            if (activeFakeWall.wallBroken)
            {
                ParticleSystem buff = Instantiate(dogBuff, transform.position, transform.rotation);
                buff.gameObject.SetActive(true);
                buff.Play();
                StartCoroutine(TimedDeactivation());
            }
        }
    }

    IEnumerator TimedDeactivation()
    {
        yield return new WaitForSeconds(1);
        Deactivate();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        TrackPlayer();
    }

}
