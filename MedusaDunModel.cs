using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Playables;

public class MedusaDunModel : DunNPC
{
    public BattleController battleC;
    public Transform hallPointA;
    public Transform hallPointB;
    public float speed = 3f; 
    public Transform targetPoint;
    public PlayableDirector breakNTurnA;
    public PlayableDirector breakNTurnB;
    public bool battleTrigger;
    
    public float desiredHairWeight = 1.0f;                  // Weight of hair animations
    public float hairWeightChangeSpeed = 5f;				// Change per second


    void Start()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
    }

    public void SetDesiredHairWeight(float newValue)
    {
        desiredHairWeight = newValue;
    }

    void MoveTowardsTarget()
    {
        if (targetPoint != null)
        {
            anim.SetFloat("locomotion", .75f);
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);          
            if (Vector3.Distance(transform.position,targetPoint.position) < 1)
            {        
                SwitchTarget();
            }
        }
    }

    IEnumerator LaunchBattle()
    {
        PlayerController player = battleC.sceneController.playerController;
        breakNTurnA.Stop();
        breakNTurnB.Stop();
        player.controller.enabled = false;
        transform.LookAt(player.transform);
        anim.SetTrigger("taunt");
        yield return new WaitForSeconds(3);
        battleC.SetBattle(23);
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    void SwitchTarget()
    {       
        StartCoroutine(SwitchTimer());
    }

    IEnumerator SwitchTimer()
    {      
        Transform savePoint = targetPoint;
        transform.parent = savePoint;
        targetPoint = null;
        
        if (savePoint == hallPointA)
        {
            breakNTurnA.Play();
            float turnTime = (float)breakNTurnA.duration;
            yield return new WaitForSeconds(turnTime);
            targetPoint = hallPointB;
        }
        if (savePoint == hallPointB)
        {
            breakNTurnB.Play();
            float turnTime = (float)breakNTurnB.duration;
            yield return new WaitForSeconds(turnTime);
            targetPoint = hallPointA;
        }
        transform.LookAt(targetPoint);
    }

    void Update()
    {
        if (anim.GetLayerWeight(1) != desiredHairWeight)
        {
            anim.SetLayerWeight(1, Mathf.MoveTowards(anim.GetLayerWeight(1), desiredHairWeight, hairWeightChangeSpeed * Time.deltaTime));
        }

        if (targetPoint != null)
        {
            MoveTowardsTarget();
        }

        if (inRange && !battleTrigger)
        {
            battleTrigger = true;
            remove = true;
            targetPoint = null;
            StartCoroutine(LaunchBattle());
        }
    }
}
