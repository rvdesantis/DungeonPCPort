using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;

public class StatueRoomParent : RoomPropParent
{
    public Transform crystalParent;
    public Transform floorGrate;
    public Transform roofGrate;

    public float rotationSpeed = 10f;

    public DunSwitch startSwitch;
    public List<DunSwitch> wallSwitches;
    public DunSwitch swapSwitchN;
    public DunSwitch swapSwitchS;

    public DunItem chaosOrb;
    public List<GameObject> wallCovers;

    public PlayableDirector crystalSwitchPlayable;
    public PlayableDirector fallChaosPlayable;
    public bool gravityOff;
    public bool skippedTrigger;





    private void Start()
    {
        RoomSetUp();
        StartCoroutine(RotateObjects());
    }

    public void EndStart()
    {
        SceneController controller = FindObjectOfType<SceneController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        PartyController party = FindObjectOfType<PartyController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        controller.activePlayable = null;
        controller.endAction = null;
        foreach (DunModel model in party.activeParty)
        {
            model.gameObject.SetActive(false);
        }
        player.controller.enabled = true;
        uiController.compassObj.SetActive(true);
        uiController.rangeImage.gameObject.SetActive(false);
        if (uiController.interactUI.activeObj == startSwitch.gameObject)
        {
            uiController.ToggleKeyUI(startSwitch.gameObject, false);
        }
        MusicController music = FindObjectOfType<MusicController>();
        music.CrossfadeToNextClip(music.dungeonMusicClips[Random.Range(0, music.dungeonMusicClips.Count)]);
    }
    IEnumerator StartSwitch()
    {
        Debug.Log("Room ENV Enter Trigger");
        PartyController party = FindObjectOfType<PartyController>();
        PlayerController player = FindObjectOfType<PlayerController>();
        MonsterController monsters = FindObjectOfType<MonsterController>();
        SceneController controller = FindObjectOfType<SceneController>();
        DunUIController uiController = FindObjectOfType<DunUIController>();

        controller.activePlayable = crystalSwitchPlayable;
        controller.endAction = EndStart;
        party.AssignCamBrain(crystalSwitchPlayable, 3);
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
        // add a check for switch room activation, set to default for testing

        foreach(DunSwitch sw in wallSwitches)
        {
            sw.locked = true;
            sw.switchAnim.SetTrigger("switchOn");
        }
        swapSwitchN.locked = true;
        swapSwitchN.switchAnim.SetTrigger("switchOn");
        swapSwitchS.locked = true;
        swapSwitchS.switchAnim.SetTrigger("switchOn");

        DistanceController distance = FindObjectOfType<DistanceController>();
        distance.switches.Add(startSwitch);
    }

    private IEnumerator RotateObjects()
    {
        while (true)
        {
            crystalParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            floorGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            roofGrate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void Update()
    {
        if (startSwitch.switchOn && !gravityOff)
        {
            gravityOff = true;
            startSwitch.locked = true;
            CrystalSwitch();
        }
    }
}
