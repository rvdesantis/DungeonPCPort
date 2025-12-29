using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TrapGhost : MonoBehaviour
{
    public FakeFloor activeFakeFloor;
    public PlayerController player;
    public SkinnedMeshRenderer skinnedMesh;
    public bool idle;
    public bool triggered;

    public PlayableDirector idleLoopDirector;
    public PlayableDirector fallDirector;





    public void TrackPlayer()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < 10)
        {
            idle = false;
            triggered = true;
            StartCoroutine(TimedDeactivation());
        }
    }

    IEnumerator TimedDeactivation()
    {
        fallDirector.Play();
        yield return new WaitForSeconds((float)fallDirector.duration);
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
