using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class StatueWeeper : DunModel
{
    public Vector3 spawnPosition;
    public Quaternion spawnRot;
    public bool activated;
    public bool trackPlayer;
    public bool stalking;
    public PlayerController player;
    public SceneBuilder builder;
    public GameObject stand;
    public HallStarterCube currentCube;
    public HallStarterCube lastCube;

    public GameObject blackmist;
    public GameObject spell;
    public List<AudioClip> audioClips;
    public Light spotlight;
    public bool battle;
    public BoxCollider baseCollider;
    public PlayableDirector encounterPlayable;
    public CinemachineVirtualCamera encounterCam;

    public void ConnectStatue()
    {
        if (builder == null)
        {
            builder = FindObjectOfType<SceneBuilder>();
        }
        if (player == null)
        {
            player = builder.playerController;
        }
    }

    public void RandomStatue()
    {
        int x = Random.Range(0, 3);
        if (x == 0)
        {
            anim.SetTrigger("statue1");
        }
        if (x == 1)
        {
            anim.SetTrigger("statue2");
        }
        if (x == 2)
        {
            anim.SetTrigger("statue3");
        }
    }


    public void TrackPlayer(Vector3 playerPosition)
    {
        playerPosition = player.transform.position;
        foreach (HallStarterCube starter in builder.createdStarters)
        {
            if (Vector3.Distance(playerPosition, starter.transform.position) < 6)
            {
                if (currentCube != null)
                {
                    if (starter != currentCube)
                    {
                        if (lastCube != currentCube && lastCube != starter)
                        {
                            HallStarterCube jumpCube = null;
                            if (lastCube != null)
                            {
                                jumpCube = lastCube;
                            }
                            lastCube = currentCube;
                            currentCube = starter;
                            if (jumpCube != null)
                            {
                                if (jumpCube != lastCube && jumpCube != currentCube)
                                {
                                    // check distance
                                    if (Vector3.Distance(playerPosition, transform.position) > 20)
                                    { 
                                        transform.position = jumpCube.floorPoint.transform.position;
                                        transform.rotation = jumpCube.floorPoint.transform.rotation;
                                        StartCoroutine(Jump());
                                    }
                                }
                            }
                        }
                    }
                }
                if (currentCube == null)
                {
                    currentCube = starter;
                }
            }
        }
    }

    IEnumerator Crumble()
    {
        baseCollider.enabled = false;
        encounterCam.gameObject.SetActive(false);
        encounterCam.m_Priority = -1;
        activated = false;
        stalking = false;
        trackPlayer = false;
        anim.SetTrigger("death");
        audioSource.PlayOneShot(audioClips[1]);
        yield return new WaitForSeconds(7);
        gameObject.SetActive(false);        
    }

    public void StartBattle()
    {
        StartCoroutine(BattleTimer());
    }

    IEnumerator BattleTimer()
    {
        SceneController controller = FindObjectOfType<SceneController>();
        BattleController battleC = FindObjectOfType<BattleController>();
        controller.playerController.controller.enabled = false;

        encounterCam.gameObject.SetActive(true);
        encounterCam.m_Priority = 10;
        anim.SetTrigger("cast1");

        yield return new WaitForSeconds(5);
        battleC.afterBattleAction = KillStatue;
        battleC.SetBattle(14);

    }

    public void KillStatue()
    {
        StartCoroutine(Crumble());
    }

    IEnumerator Jump()
    {
        blackmist.gameObject.SetActive(true);
        if (!spotlight.gameObject.activeSelf)
        {
            spotlight.gameObject.SetActive(true);
        }
        Vector3 lookTarget = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(lookTarget);
        if (Vector3.Distance(transform.position, player.transform.position) < 75)
        {
            audioSource.PlayOneShot(audioClips[0]);
        }
        RandomStatue();
        stalking = true;
        yield return new WaitForSeconds(2);
        blackmist.SetActive(false);
    }

    private void Update()
    {        
        if (activated)
        {
            if (trackPlayer)
            {
                if (player == null)
                {
                    player = FindObjectOfType<PlayerController>();
                }
                Vector3 playerPosition = player.transform.position;
                Vector3 lookPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
                TrackPlayer(playerPosition);
                if (stalking)
                {
                    if (player.enabled)
                    {
                        if (Vector3.Distance(playerPosition, transform.position) < 5 && !battle)
                        {
                            battle = true;
                            StartBattle();
                        }
                    }
                }
            }
        }
    }

}
