using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public SceneBuilder sceneBuilder;
    public GameObject mapParentObject;
    public DunUIController uiController;
    public Camera mapCam;
    public PlayerController playerController;
    public List<Sprite> iconMasterList; // 0 - Quest, 1 - Battle, 2 - Chest, 3 - Shop, 4 - NPC, 5 - Portal, 6 - Health
    public List<SpriteRenderer> roomIcons;
    public List<SpriteRenderer> endIcons;
    public List<SpriteRenderer> hallIcons;
    public List<SpriteRenderer> turnIcons;
    public List<SpriteRenderer> bossRoomIcons;
    public bool buildFinished;
    public bool isToggling;

    public List<string> layerNames; // map - 6 / hiddenmap - 7

    public float moveSpeed;
    public GameObject camObject;
    public List<GameObject> boundries;

    public List<Image> mapInputImages;
    public List<Image> mapJoyImages;

    private void Start()
    {
        
    }

    public void JoyStickSwap()
    {
        foreach (Image image in mapInputImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (Image image in mapJoyImages)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void ShowMapOutLine()
    {
        foreach (Cube room in sceneBuilder.createdRooms)
        {
            LayerOnMap(room, 6, false);
        } 
        foreach (Cube hall in sceneBuilder.createdHallCubes)
        {
            LayerOnMap(hall, 6, false);
        }
        foreach (Cube turn in sceneBuilder.createdTurns)
        {
            LayerOnMap(turn, 6, false);
        }
        foreach (Cube end in sceneBuilder.createdDeadEnds)
        {
            LayerOnMap(end, 6, false);
        }
        foreach (Cube trap in sceneBuilder.createdHallSideCubes)
        {
            LayerOnMap(trap, 6, false);
        }
        foreach (Cube trap in sceneBuilder.createdTrapHalls)
        {
            LayerOnMap(trap, 6, false);
        }

        InventoryController inventory = FindAnyObjectByType<InventoryController>();

        if (inventory.mapstatus == InventoryController.MapInventoryStatus.sketched)
        {
            inventory.mapstatus = InventoryController.MapInventoryStatus.outlined;
        }
    }

    public void ShowFullMap(bool secrets = false)
    {
        foreach (Cube room in sceneBuilder.createdRooms)
        {
            LayerOnMap(room, 6, true);
        }
        foreach (Cube hall in sceneBuilder.createdHallCubes)
        {
            LayerOnMap(hall, 6, true);
        }
        foreach (Cube turn in sceneBuilder.createdTurns)
        {
            LayerOnMap(turn, 6, true);
        }
        foreach (Cube end in sceneBuilder.createdDeadEnds)
        {
            LayerOnMap(end, 6, true);
        }
        foreach (Cube trap in sceneBuilder.createdHallSideCubes)
        {
            LayerOnMap(trap, 6, false);
        }
        foreach (Cube trap in sceneBuilder.createdTrapHalls)
        {
            LayerOnMap(trap, 6, false);
        }
        foreach (Cube boss in sceneBuilder.createdBossRooms)
        {
            LayerOnMap(boss, 6, true);
        }

        InventoryController inventory = FindAnyObjectByType<InventoryController>();

        if (inventory.mapstatus != InventoryController.MapInventoryStatus.secret)
        {
            inventory.mapstatus = InventoryController.MapInventoryStatus.full;
        }

        if (secrets)
        {
            foreach (Cube end in sceneBuilder.createdSecretEnds)
            {
                LayerOnMap(end, 6, true);
                inventory.mapstatus = InventoryController.MapInventoryStatus.secret;
            }
            foreach (Cube trap in sceneBuilder.createdHallSideCubes)
            {
                LayerOnMap(trap, 6, true);
            }
            foreach (Cube trap in sceneBuilder.createdTrapHalls)
            {
                LayerOnMap(trap, 6, true);
            }
        }
    }

    public void GatherMap()
    {
        StartCoroutine(GatherMapIcons());
    }

    public void GatherLayers()
    {
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layerNames.Add(layerName);
            }
        }
    }

    public void LayerOnMap(Cube cube, int layer, bool includeIcon = false) // 6 on - 7 off
    {
        cube.mapIcon.gameObject.layer = layer;
        foreach(Transform child in cube.mapIcon.transform)
        {
            child.gameObject.layer = layer;
        }
        if (includeIcon)
        {
            cube.mapIcon.icon.gameObject.layer = layer;
        }        
        if (layer == 6)
        {
            cube.mapIcon.onMap = true;
        }
        if (layer == 7)
        {
            cube.mapIcon.onMap = false;
        }
    }

    private IEnumerator EnableToggling()
    {
        yield return new WaitForSeconds(0.15f);
        isToggling = false;
    }

    public void ToggleMap()
    {
        if (isToggling)
        {
            return;
        }

        isToggling = true;

        if (mapParentObject.activeSelf)
        {
            playerController.enabled = true;
            mapParentObject.SetActive(false);
            uiController.uiActive = false;
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[3]);
            uiController.lowerUIobj.SetActive(true);          
            uiController.compassObj.SetActive(true);

            StartCoroutine(EnableToggling());
            return;
        }

        if (!mapParentObject.activeSelf)
        {
            playerController.enabled = false;      
            PlaceCamera();
            uiController.uiActive = true;
            uiController.uiAudioSource.PlayOneShot(uiController.uiSounds[2]);
            mapParentObject.SetActive(true);

            uiController.lowerUIobj.SetActive(false);   
            uiController.compassObj.SetActive(false);

            StartCoroutine(EnableToggling());
            return;
        }
    }

    public void PlaceCamera()
    {
        mapCam.transform.position = new Vector3(playerController.transform.position.x, 100, playerController.transform.position.z);
        mapCam.orthographicSize = 50;
    }

    private IEnumerator GatherMapIcons()
    {
        foreach (Cube room in sceneBuilder.createdRooms)
        {
            SpriteRenderer targetIcon = room.mapIcon.icon;
            roomIcons.Add(targetIcon);
            Vector3 scale = new Vector3(targetIcon.transform.localScale.x, targetIcon.transform.localScale.y, targetIcon.transform.localScale.z);
            targetIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // to reset
            targetIcon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            targetIcon.transform.localScale = scale;
            room.mapIcon.onMap = false;
        }
 
        yield return new WaitForSeconds(.1f);
        foreach (Cube end in sceneBuilder.createdDeadEnds)
        {        
            SpriteRenderer targetIcon = end.mapIcon.icon;
            endIcons.Add(targetIcon);
            Vector3 scale = new Vector3(targetIcon.transform.localScale.x, targetIcon.transform.localScale.y, targetIcon.transform.localScale.z);
            targetIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // to reset
            targetIcon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            targetIcon.transform.localScale = scale;
            end.mapIcon.onMap = false;
        }
   
        yield return new WaitForSeconds(.1f);
        foreach (Cube hall in sceneBuilder.createdHallCubes)
        {
            SpriteRenderer targetIcon = hall.mapIcon.icon;
            hallIcons.Add(targetIcon);
            Vector3 scale = new Vector3(targetIcon.transform.localScale.x, targetIcon.transform.localScale.y, targetIcon.transform.localScale.z);
            targetIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // to reset
            targetIcon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            targetIcon.transform.localScale = scale;
            hall.mapIcon.onMap = false;
        }

        yield return new WaitForSeconds(.1f);
        foreach (Cube turn in sceneBuilder.createdTurns)
        {
            SpriteRenderer targetIcon = turn.mapIcon.icon;
            turnIcons.Add(targetIcon);
            Vector3 scale = new Vector3(targetIcon.transform.localScale.x, targetIcon.transform.localScale.y, targetIcon.transform.localScale.z);
            targetIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // to reset
            targetIcon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            targetIcon.transform.localScale = scale;
            turn.mapIcon.onMap = false;
        }

        foreach (Cube secret in sceneBuilder.createdSecretEnds)
        {
            SpriteRenderer targetIcon = secret.mapIcon.icon;
            turnIcons.Add(targetIcon);
            Vector3 scale = new Vector3(targetIcon.transform.localScale.x, targetIcon.transform.localScale.y, targetIcon.transform.localScale.z);
            targetIcon.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // to reset
            targetIcon.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            targetIcon.transform.localScale = scale;
            secret.mapIcon.onMap = false;
        }
   
        buildFinished = true;
    }

    private void Update()
    {
        if (mapParentObject.activeSelf)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            float stickInput = Input.GetAxis("Joystick Right Vertical");
            float joystickHorizontalInput = Input.GetAxis("Joystick Horizontal");
            float joystickVerticalInput = Input.GetAxis("Joystick Vertical");


            if (Input.GetKey(KeyCode.D) || joystickHorizontalInput > 0.5f)
            {
                camObject.transform.position = Vector3.Lerp(camObject.transform.position, boundries[3].transform.position, moveSpeed);
            }
            if (Input.GetKey(KeyCode.A) || joystickHorizontalInput < -0.5f)
            {
                camObject.transform.position = Vector3.Lerp(camObject.transform.position, boundries[2].transform.position, moveSpeed);
            }
            if (Input.GetKey(KeyCode.S) || joystickVerticalInput > 0.5f)
            {
                camObject.transform.position = Vector3.Lerp(camObject.transform.position, boundries[0].transform.position, moveSpeed);
            }
            if (Input.GetKey(KeyCode.W) || joystickVerticalInput < -0.5f)
            {
                camObject.transform.position = Vector3.Lerp(camObject.transform.position, boundries[1].transform.position, moveSpeed);
            }

            


            if (scrollInput > 0f || stickInput < -0.5f)
            {
                if (mapCam.orthographicSize > 30)
                {
                    mapCam.orthographicSize = mapCam.orthographicSize - 5;
                }
            }
            else if (scrollInput < 0f || stickInput > 0.5f)
            {
                if (mapCam.orthographicSize < 500)
                {
                    mapCam.orthographicSize = mapCam.orthographicSize + 5;
                }
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                ToggleMap();
            }
        }
        if (buildFinished)
        {
            if (!uiController.uiActive || mapParentObject.activeSelf)
            {
                if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.Mouse2) || Input.GetKey(KeyCode.Alpha2))
                {
                    ToggleMap();
                    return;
                }
            }
        }
    }



}
