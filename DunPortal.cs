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

    public FakeWall destoryWall;
    public MeshRenderer portalViewMeshRenderer;
    public Material returnViewMaterial;
    public RenderTexture returnViewTexture;
    public AudioClip returnSound;

    public IEnumerator Transport()
    {
        sceneController.uiController.compassObj.SetActive(false);
        jumpCount++;
        connectedPortal.connectedPortal = this;
        connectedPortal.sceneController = sceneController;
        connectedPortal.gameObject.SetActive(true);


        PlayerController player = sceneController.playerController;
        player.controller.enabled = false;
        player.transform.position = connectedPortal.transportPosition.transform.position;
        player.transform.rotation = connectedPortal.transportPosition.transform.rotation;
        player.controller.enabled = true;
        player.audioSource.PlayOneShot(connectedPortal.returnSound);
        if (connectedPortal.destoryWall != null)
        { 
            connectedPortal.destoryWall.inRange = true;
            connectedPortal.destoryWall.WallBreak();
        }
        yield return new WaitForSeconds(.25f);

        connectedPortal.gameObject.SetActive(false);
        if (closeOnJump)
        {
            gameObject.SetActive(false);
        }
        sceneController.uiController.compassObj.SetActive(true);
    }


    private void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                StartCoroutine(Transport());
            }
        }
    }


    public void ConnectPortals(DunPortal connectorPortal)
    {
        connectedPortal = connectorPortal;
        connectorPortal.connectedPortal = this;

        Material[] materials = portalViewMeshRenderer.materials;
        materials[0] = connectorPortal.returnViewMaterial;
        portalViewMeshRenderer.materials = materials;
    }
}
