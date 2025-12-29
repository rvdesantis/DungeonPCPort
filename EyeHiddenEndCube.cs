using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;

public class EyeHiddenEndCube : HiddenEndCube
{
    public GameObject churchParent;
    public EyeElevatorNPC eyeNPC;
    public Transform churchEnterPos;
    public Transform dunReturnPos;
    public SceneController controller;
    public PlayableDirector eyeElevatorOnboard;
    public PlayableDirector eyeFloatUp;
    public PlayableDirector eyeFloatDown;

    public GameObject fallBoxCollider;

    public DunItem chaosOrb;
    public GameObject eyeStatue;
    public List<GameObject> eyeCultMembers;
    public PlayableDirector cultIdle;
    public bool combatLaunch;


    public override void SecretSetUp()
    {
        if (distanceC == null)
        {
            distanceC = FindAnyObjectByType<DistanceController>();
        }
        controller = FindAnyObjectByType<SceneController>();

        base.SecretSetUp();

        distanceC.npcS.Add(eyeNPC);
        
    }

    public void ElevatorUp()
    {
        churchParent.gameObject.SetActive(true);
        fallBoxCollider.SetActive(false);
       
        foreach (DunModel model in controller.party.activeParty)
        {
            model.gameObject.SetActive(true);
            model.transform.position = eyeElevatorOnboard.transform.position;
            model.transform.parent = eyeElevatorOnboard.transform;
            model.AssignToDirector(eyeElevatorOnboard);
            
        }
        controller.party.AssignCamBrain(eyeElevatorOnboard, 3);
        controller.party.AssignCamBrain(eyeFloatUp, 3);
        controller.party.AssignCamBrain(eyeFloatDown, 3);
        StartCoroutine(UpTimer());
        
    }

    public void PickUpStatue()
    {

    }

    public void ElevaatorDown()
    {
        StartCoroutine(DownTimer());
    }

    IEnumerator UpTimer()
    {
   
        PlayerController player = FindAnyObjectByType<PlayerController>();
        player.controller.enabled = false;
        eyeElevatorOnboard.Play();
        yield return new WaitForSeconds((float)eyeElevatorOnboard.duration);
        eyeFloatUp.Play();
        foreach (DunModel model in controller.party.activeParty)
        {
            model.gameObject.SetActive(false);
     

        }
        float timer = (float)eyeFloatUp.duration;
        yield return new WaitForSeconds(timer);
        if (!combatLaunch)
        {
            foreach (GameObject model in eyeCultMembers)
            {
                model.SetActive(true);
            }
            cultIdle.Play();
        }
        player.transform.position = churchEnterPos.position;
        player.controller.enabled = true;
        eyeNPC.upPosition = true; 
    }

    IEnumerator DownTimer()
    {
        PlayerController player = controller.playerController;
        player.controller.enabled = false;
        eyeFloatDown.Play();
        float timer = (float)eyeFloatDown.duration;
        yield return new WaitForSeconds(timer);
        foreach (GameObject model in eyeCultMembers)
        {
            model.SetActive(false);
        }

        cultIdle.Stop();
        churchParent.gameObject.SetActive(false);
        fallBoxCollider.SetActive(true);
        player.transform.position = dunReturnPos.position;
        player.controller.enabled = true;
        eyeNPC.eyeIdle.Play();
        eyeNPC.upPosition = false;
    }
    
}
