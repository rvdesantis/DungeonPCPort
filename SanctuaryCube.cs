using System.Collections;
using System.Collections.Generic;
using DTT.PlayerPrefsEnhanced;
using UnityEngine;

public class SanctuaryCube : Cube
{
    public DunPortal returnPortal;
    public EventCube eventCubes;
    public GameObject campFire;
    public bool fireRange;
    public PlayerController player;
    public DunUIController uiController;
    public UnlockController unlockables;
    public CampfireUI campfireUI;
    public List<DunNPC> sanctVendors;



    public void FireRange()
    {
        Vector3 playerPosition = player.transform.position;

        if (Vector3.Distance(playerPosition, campFire.transform.position) < 5 && !fireRange)
        {
            fireRange = true;
        }
        if (Vector3.Distance(playerPosition, campFire.transform.position) > 5 && fireRange)
        {
            fireRange = false;
            uiController.rangeImage.gameObject.SetActive(false);
            uiController.customImage.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        FireRange();
        if (fireRange)
        {
            if (!uiController.customImage.gameObject.activeSelf)
            {
                uiController.rangeImage.sprite = uiController.rangeSprites[4];
                uiController.customImage.sprite = uiController.customSprites[0];
                uiController.rangeImage.gameObject.SetActive(true);
                uiController.customImage.gameObject.SetActive(true);
            }

            if (!uiController.uiActive && player.enabled && !uiController.isToggling)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
                {
                    if (!campfireUI.gameObject.activeSelf)
                    {
                        campfireUI.ToggleCampFireUI(true);
                        uiController.RemoteToggleTimer();
                    }
                }
            }
        }
    }
}
