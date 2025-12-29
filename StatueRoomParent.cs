using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;


public class StatueRoomParent : RoomPropParent
{
    public PlayerController player;
    public Transform crystalParent;
    public Transform floorGrate;
    public Transform roofGrate;
    public Transform returnPosition;

    public Transform crystalParent2;
    public Transform floorGrate2;
    public Transform roofGrate2;

    public float rotationSpeed = 10f;

    public DunSwitch startSwitch;
    public List<DunSwitch> wallSwitches;
    public DunSwitch swapSwitchN;
    public DunSwitch swapSwitchS;
    public List<GameObject> gavityObjects;

    public DunItem chaosOrb;
    public List<GameObject> wallCovers;

    public PlayableDirector crystalSwitchPlayable;
    public PlayableDirector fallChaosPlayable;
    public PlayableDirector swapPlayableOne;
    public PlayableDirector swapPlayableTwo;

    public bool gravityOff;
    public bool skippedTrigger;

    public GameObject rotationRoomParent;
    public bool leftRotation;
    public Transform startPoint;
    public Transform swapPointTwo;

    private void Start()
    {
        RoomSetUp();
        StartCoroutine(RotateObjects());
    }

    public void EndStart()
    {
        rotationRoomParent.gameObject.SetActive(true);

        SceneController controller = FindAnyObjectByType<SceneController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        PartyController party = FindAnyObjectByType<PartyController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();       
        controller.activePlayable = null;
        controller.endAction = null;
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.transform.position = startPoint.position;
        player.controller.enabled = true; 

        uiController.compassObj.SetActive(true);
        uiController.rangeImage.gameObject.SetActive(false);
        if (uiController.interactUI.activeObj == startSwitch.gameObject)
        {
            uiController.ToggleKeyUI(startSwitch.gameObject, false);
        }
        MusicController music = FindAnyObjectByType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);

        foreach (DunSwitch sw in wallSwitches)
        {
            controller.distance.switches.Add(sw);
        }
        controller.distance.switches.Add(swapSwitchN);
        controller.distance.switches.Add(swapSwitchS);

        foreach (GameObject obj in gavityObjects)
        {
            obj.transform.parent = null;
        }

    }
    IEnumerator StartSwitch()
    {
        Debug.Log("Room ENV Enter Trigger");
        PartyController party = FindAnyObjectByType<PartyController>();
        PlayerController player = FindAnyObjectByType<PlayerController>();
        MonsterController monsters = FindAnyObjectByType<MonsterController>();
        SceneController controller = FindAnyObjectByType<SceneController>();
        DunUIController uiController = FindAnyObjectByType<DunUIController>();

        controller.activePlayable = crystalSwitchPlayable;
        controller.endAction = EndStart;
        party.AssignCamBrain(crystalSwitchPlayable, 3);
        party.AssignCamBrain(fallChaosPlayable, 3);
        foreach (DunModel model in party.activeParty)
        {
            model.AssignToDirector(crystalSwitchPlayable);
            model.gameObject.SetActive(true);
            if (model.torch != null)
            {
                model.torch.SetActive(false);
            }
            if (model.activeWeapon != null)
            {
                model.activeWeapon.SetActive(false);
            }
            model.transform.position = crystalSwitchPlayable.transform.position;
            model.transform.parent = crystalSwitchPlayable.transform;
        }
        party.activeParty[0].torch.SetActive(false);
        float clipTime = (float)crystalSwitchPlayable.duration;
        player.controller.enabled = false;
        uiController.compassObj.SetActive(false);
        crystalSwitchPlayable.Play();
        player.transform.position = startPoint.position;
        yield return new WaitForSeconds(clipTime);

       if (controller.activePlayable != null)
        {
            EndStart();
        }
    }

    public void CrystalSwitch()
    {
        StartCoroutine(StartSwitch());
    }


    public void RoomSetUp() // does not add wall switches to distance controller until activated, only start switch.
    {
        foreach (GameObject wallCov in roomParent.wallCovers)
        {
            if (wallCov.activeSelf)
            {
                int x = roomParent.wallCovers.IndexOf(wallCov);
                wallCovers[x].SetActive(true);
            }
        }
        DistanceController distance = FindAnyObjectByType<DistanceController>();
        distance.switches.Add(startSwitch);

        player = FindAnyObjectByType<PlayerController>();
    }

    private IEnumerator RotateObjects()
    {
        while (true)
        {
            crystalParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            floorGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            roofGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            if (rotationRoomParent.activeSelf)
            {
                crystalParent2.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                floorGrate2.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                roofGrate2.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }

    public void TriggerSwapSwitchOne()
    {
        Debug.Log("Starting Swap 1 Playable");
        swapPlayableOne.Play();
    }

    public void TriggerSwapSwitchTwo()
    {
        Debug.Log("Starting Swap 2 Playable");
        swapPlayableTwo.Play();
    }

    public void SwitchStatusCheck()
    {
        int switchCount = wallSwitches.Count;
        int switchCounter = 0;

        foreach (DunSwitch dunSwitch in wallSwitches)
        {
            if (dunSwitch.switchOn && dunSwitch.locked)
            {
                switchCounter++;
            }
        }

        if (switchCounter == switchCount)
        {
            Debug.Log("Switches Complete");
            StartCoroutine(EndPuzzle());
        }
    }
    IEnumerator EndPuzzle()
    {
        SceneController controller = FindAnyObjectByType<SceneController>();

        player.controller.enabled = false;
        yield return new WaitForSeconds(2);
        player.transform.position = returnPosition.position;
        fallChaosPlayable.Play();
        yield return new WaitForSeconds(.5f);
        rotationRoomParent.SetActive(false);
        controller.distance.dunItems.Add(chaosOrb);
        yield return new WaitForSeconds((float)fallChaosPlayable.duration);
        player.controller.enabled = true;  
    }

    private void Update()
    {
        if (startSwitch.switchOn && !gravityOff)
        {
            gravityOff = true;
            startSwitch.locked = true;
            CrystalSwitch();
        }
        if (rotationRoomParent.activeSelf)
        {
            SwitchStatusCheck();
        }
    }
}
