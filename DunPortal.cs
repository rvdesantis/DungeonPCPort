using System.Collections;
using UnityEngine;

public class DunPortal : MonoBehaviour
{    
    public GameObject transportPosition;
    public DunPortal connectedPortal;
    public SceneController sceneController;
    public bool inRange;
    public int jumpCount;
    public bool closeOnJump;
    public bool swapOnJump;
    public bool assigned;
    public FakeWall destoryWall;
    public MeshRenderer portalViewMeshRenderer;
    public Material returnViewMaterial;
    public RenderTexture returnViewTexture;
    public AudioClip returnSound;

    public IEnumerator Transport()
    {
        DunUIController uiController = FindAnyObjectByType<DunUIController>();
        if (!uiController.uiActive)
        {
            jumpCount++;
            connectedPortal.connectedPortal = this;
            connectedPortal.sceneController = sceneController;
            connectedPortal.gameObject.SetActive(true);
            uiController.rangeImage.gameObject.SetActive(false);

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
            uiController.interactUI.activeObj = null;
            uiController.interactParent.SetActive(false);
            yield return new WaitForSeconds(.25f);
            connectedPortal.gameObject.SetActive(false);
            if (closeOnJump)
            {
                gameObject.SetActive(false);
            }
            if (swapOnJump)
            {
                SwapOnJump();
            }
        }
       
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
        assigned = true;
        Material[] materials = portalViewMeshRenderer.materials;
        materials[0] = connectorPortal.returnViewMaterial;
        portalViewMeshRenderer.materials = materials;
    }

    public void SwapOnJump()
    {
        if (sceneController == null)
        {
            sceneController = FindAnyObjectByType<SceneController>();
        }
        connectedPortal.gameObject.SetActive(true);
        connectedPortal.ConnectPortals(this);
        sceneController.distance.portals.Remove(this);
        sceneController.distance.portals.Add(connectedPortal);
        connectedPortal.swapOnJump = true;       
    }
}
