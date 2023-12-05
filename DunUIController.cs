using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DunUIController : MonoBehaviour
{
    public SceneController controller;
    public bool uiStart;
    public bool uiActive;
    public bool isToggling;
    public List<Button> startButtons;
    public GameObject buttonFrameUI;
    public GameObject compassObj;
    public GameObject titleObj;
    public GameObject lowerUIobj;

    public bool joystick;
    public LoadingBarUI loadingBar;

    public List<Sprite> rangeSprites; // treasure - 0, interact - 1, merchant - 2, empty - 3- potion, 4 - custom (empty)
    public List<Sprite> customSprites; // party/campfire - 0, lock - 1, anvil - 2, armor 3, crystal 4
    public Image rangeImage;
    public Image customImage;
    public GameObject interactParent;
    public InteractUI interactUI;
    public GameObject spacebarObj;
    public GameObject btObj;
    public List<AudioClip> uiSounds; // menu open, 1 - inventory open, 2 - Map open, 3 - Map Close, 4 - unlocked stinger
    public AudioSource uiAudioSource;

    public SkipUI skipUI;
    public InventoryUI inventoryUI;
    public PickUpUI pickUpUI;
    public GameObject settingsUI;
    public MessagePanelUI messagePanelUI;
    public UnlockedUI unlockUI;
    public BlacksmithUI blackSmithUI;
    public ConfirmUI confirmUI;

    IEnumerator ToggleTimer()
    {
        yield return new WaitForSeconds(.25f);
        isToggling = false;
    }

    public void RemoteToggleTimer()
    {
        StartCoroutine(ToggleTimer());
    }

    public void UIExitGame()
    {
        // saves
        Application.Quit();
    }

    public void ToggleKeyUI(GameObject activeObj, bool inRange)
    {
        if (!inRange && interactUI.activeObj != null)
        {
            interactUI.activeObj = null;
            if (joystick)
            {
                btObj.SetActive(false);
                interactParent.SetActive(false);
            }
            if (!joystick)
            {
                spacebarObj.SetActive(false);
                interactParent.SetActive(false);
            }      
            return;
        }

        if (inRange && interactUI.activeObj == null)
        {
            interactUI.activeObj = activeObj;
            if (joystick)
            {
                btObj.SetActive(true);
                interactParent.SetActive(true);
            }
            if (!joystick)
            {
                spacebarObj.SetActive(true);
                interactParent.SetActive(true);
            }
            return;
        }
    }

    public void JoyStickChecker()
    {
        if (!joystick)
        {
            float joystickHorizontalInput = Input.GetAxis("Joystick Horizontal");
            float joystickVerticalInput = Input.GetAxis("Joystick Vertical");
            float joystickRightHorizontalInput = Input.GetAxis("Joystick Right Horizontal");
            float joystickRightVerticalInput = Input.GetAxis("Joystick Right Vertical");

            if (Mathf.Abs(joystickRightHorizontalInput) > 0.1f || Mathf.Abs(joystickRightHorizontalInput) < -0.1f)
            {
                joystick = true;
            }

            if (Mathf.Abs(joystickRightVerticalInput) > 0.1f || Mathf.Abs(joystickRightVerticalInput) < -0.1f)
            {
                joystick = true;
            }

            if (Mathf.Abs(joystickVerticalInput) > 0.1f || Mathf.Abs(joystickVerticalInput) < -0.1f)
            {
                joystick = true;


                if (Mathf.Abs(joystickHorizontalInput) > 0.1f || Mathf.Abs(joystickHorizontalInput) < -0.1f)
                {
                    joystick = true;
                }


                if (Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    joystick = true;
                }


                if (joystick)
                {
                    inventoryUI.JoyStickSwap();
                    controller.mapController.JoyStickSwap();
                }
            }

        }
    }

    public void ActivePlayableChecker()
    {
        if (controller.activePlayable != null)
        {
            if (interactParent.activeSelf)
            {
                interactParent.SetActive(false);
            }
        }
    }

    public void OpenDunInventory()
    {        
        inventoryUI.OpenInventory();
        uiAudioSource.PlayOneShot(uiSounds[1]);
    }

    public void CloseDunInventory()
    {
        compassObj.SetActive(true);
        lowerUIobj.SetActive(true);
        uiActive = false;
        inventoryUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isToggling && controller.active)
        {
            if (!uiActive)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    isToggling = true;
                    compassObj.SetActive(false);
                    lowerUIobj.SetActive(false);
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(true);
                    reset.Select();

                    startButtons[2].gameObject.SetActive(true);
                    startButtons[3].gameObject.SetActive(true);
                    startButtons[4].gameObject.SetActive(true);
                    uiActive = true;
                    buttonFrameUI.SetActive(true);
                    uiAudioSource.PlayOneShot(uiSounds[0]);
                    StartCoroutine(ToggleTimer());
                }
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    if (!inventoryUI.gameObject.activeSelf)
                    {
                        isToggling = true;
                        compassObj.SetActive(false);
                        lowerUIobj.SetActive(false);
                        uiActive = true;

                        inventoryUI.gameObject.SetActive(true);
                        OpenDunInventory();
                        StartCoroutine(ToggleTimer());
                    }
                }
            }

            if (uiActive && !isToggling)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    bool highlighted = false;
                    foreach (Button button in startButtons)
                    {
                        if (EventSystem.current.currentSelectedGameObject == button)
                        {
                            highlighted = true;
                        }
                    }

                    if (!highlighted)
                    {
                        if (startButtons[0].gameObject.activeSelf)
                        {
                            startButtons[0].Select();
                        }
                        else
                        {
                            startButtons[1].Select();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    isToggling = true;
                    Button reset = startButtons[1];
                    reset.gameObject.SetActive(false);
                    startButtons[1].gameObject.SetActive(false);
                    startButtons[2].gameObject.SetActive(false);
                    startButtons[3].gameObject.SetActive(false);
                    startButtons[4].gameObject.SetActive(false);

                    buttonFrameUI.SetActive(false);
                    uiActive = false;
                  
                    compassObj.SetActive(true);
                    lowerUIobj.SetActive(true);
                    StartCoroutine(ToggleTimer());

                    if (inventoryUI.gameObject.activeSelf)
                    {
                        CloseDunInventory();                      
                    }
                    if (controller.mapController.mapParentObject.activeSelf)
                    {
                        controller.mapController.ToggleMap();                       
                    }
                    if (interactParent.activeSelf)
                    {
                        if (interactUI.activeObj != null)
                        {
                            interactUI.activeObj = null;
                        }
                        interactParent.SetActive(false);
                    }
                    if (settingsUI.activeSelf)
                    {                       
                        settingsUI.SetActive(false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    if (inventoryUI.gameObject.activeSelf)
                    {
                        isToggling = true;
                        CloseDunInventory();    
                        StartCoroutine(ToggleTimer());
                    }
                }
            }
        }

        if (!uiStart)
        {
            if (startButtons[0].gameObject.activeSelf)
            {
                uiStart = true;
                startButtons[0].Select();
            }
        }

        if (!joystick)
        {
            JoyStickChecker();
        }
        ActivePlayableChecker();
        if (compassObj.activeSelf)
        {
            if (!lowerUIobj.activeSelf)
            {
                lowerUIobj.SetActive(true);
            }
        }
        if (!compassObj.activeSelf)
        {
            if (lowerUIobj.activeSelf)
            {
                lowerUIobj.SetActive(false);
            }
        }
    }
}
