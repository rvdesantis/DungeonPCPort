using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq.Expressions;

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
            playerController.active = true;
            mapParentObject.SetActive(false);
            uiController.uiActive = false;
            StartCoroutine(EnableToggling());
            return;
        }

        if (!mapParentObject.activeSelf)
        {
            playerController.active = false;      
            PlaceCamera();
            uiController.uiActive = true;
            mapParentObject.SetActive(true);
            StartCoroutine(EnableToggling());
            return;
        }
    }

    public void PlaceCamera()
    {
        mapCam.transform.position = new Vector3(playerController.transform.position.x, 100, playerController.transform.position.z);
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
        foreach (Cube end in sceneBuilder.allDeadEnds)
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
            if (scrollInput > 0f || stickInput < -0.5f)
            {
                if (mapCam.fieldOfView > 30)
                {
                    mapCam.fieldOfView = mapCam.fieldOfView - 5;
                }
            }
            else if (scrollInput < 0f || stickInput > 0.5f)
            {
                if (mapCam.fieldOfView < 100)
                {
                    mapCam.fieldOfView = mapCam.fieldOfView + 5;
                }
            }
        }
        if (buildFinished)
        {
            if (Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.JoystickButton3) || Input.GetKey(KeyCode.Mouse2))
            {
                ToggleMap();
                return;
            }
        }

    }



}
