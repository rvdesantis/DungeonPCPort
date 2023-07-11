using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DunPortal : MonoBehaviour
{    
    public GameObject transportPosition;
    public DunPortal connectedPortal;
    public SceneController sceneController;
    public bool inRange;
    public int jumpCount;
    public bool closeOnJump;    

    public IEnumerator Transport()
    {
        connectedPortal.connectedPortal = this;
        connectedPortal.sceneController = sceneController;
        connectedPortal.gameObject.SetActive(true);

        PlayerController player = sceneController.playerController;
        player.controller.enabled = false;
        player.transform.position = connectedPortal.transportPosition.transform.position;
        player.transform.rotation = connectedPortal.transportPosition.transform.rotation;
        player.controller.enabled = true;

        yield return new WaitForSeconds(1);
        connectedPortal.inRange = false;
        connectedPortal.gameObject.SetActive(false);
        jumpCount++;
        connectedPortal.jumpCount++;
        if (closeOnJump)
        {
            gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(Transport());
            }
        }
    }

}
