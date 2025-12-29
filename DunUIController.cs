using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DunUIController : MonoBehaviour
{
    public SceneController controller;
    [SerializeField] private CanvasScaler canvasScaler;
    public bool uiStart;
    public bool uiActive;
    public bool isToggling;
    public List<Button> startButtons;
    public GameObject buttonFrameUI;
    public GameObject compassObj;
    public GameObject titleObj;
    public GameObject lowerUIobj;
    public GameHintsUI gameHintsUI;

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
    public CharacterUI characterUI;
    public InventoryUI inventoryUI;
    public PickUpUI pickUpUI;
    public GameObject settingsUI;
    public MessagePanelUI messagePanelUI;
    public UnlockedUI unlockUI;
    public BlacksmithUI blackSmithUI;
    public SpellSmithUI spellSmithUI;
    public ShopUI shopUI;
    public ConfirmUI confirmUI;


    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    [SerializeField] private AudioMixer masterMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button settingsConfirmBT;

    private void Start()
    {

    }

    private void AdjustUIScaling()
    {
        if (canvasScaler == null)
        {
            Debug.LogWarning("CanvasScaler reference not assigned!");
            return;
        }

        float aspect = (float)Screen.width / Screen.height;

        // Adjust scaling based on aspect ratio
        if (aspect > 3.0f)
        {
            // 32:9 or wider
            canvasScaler.matchWidthOrHeight = 0.8f;
            Debug.Log($"[ResolutionManager] Super Ultrawide detected ({aspect:F2}) → scaling = 0.8");
        }
        else if (aspect > 2.3f)
        {
            // 21:9
            canvasScaler.matchWidthOrHeight = 0.65f;
            Debug.Log($"[ResolutionManager] Ultrawide detected ({aspect:F2}) → scaling = 0.65");
        }
        else
        {
            // Standard 16:9 or 16:10
            canvasScaler.matchWidthOrHeight = 0.5f;
            Debug.Log($"[ResolutionManager] Standard aspect ({aspect:F2}) → scaling = 0.5");
        }

        Canvas.ForceUpdateCanvases();
    }


    IEnumerator ToggleTimer(float timer)
    {
        isToggling = true;
        yield return new WaitForSeconds(timer);
        isToggling = false;
    }

    public void RemoteToggleTimer(float timer)
    {
        StartCoroutine(ToggleTimer(timer));
    }

    public void CloseMenuUI()
    {
        isToggling = true;
        Cursor.visible = false;
        Button reset = startButtons[1];
        reset.gameObject.SetActive(false);
        startButtons[1].gameObject.SetActive(false);
        startButtons[2].gameObject.SetActive(false);
        startButtons[3].gameObject.SetActive(false);
        startButtons[4].gameObject.SetActive(false);
        startButtons[5].gameObject.SetActive(false);
        startButtons[6].gameObject.SetActive(false);
        buttonFrameUI.SetActive(false);
        gameHintsUI.gameObject.SetActive(false);
        uiActive = false;

        compassObj.SetActive(true);
        lowerUIobj.SetActive(true);

        StartCoroutine(ToggleTimer(.25f));
    }

    public void OpenMenuUI()

    {
        isToggling = true;
        Cursor.visible = true;
        compassObj.SetActive(false);
        lowerUIobj.SetActive(false);
        Button reset = startButtons[1];
        reset.gameObject.SetActive(true);
        reset.Select();

        startButtons[2].gameObject.SetActive(true);
        startButtons[3].gameObject.SetActive(true);
        startButtons[4].gameObject.SetActive(true);
        startButtons[5].gameObject.SetActive(true);
        startButtons[6].gameObject.SetActive(true);
        uiActive = true;
        buttonFrameUI.SetActive(true);
        gameHintsUI.gameObject.SetActive(true);
        uiAudioSource.PlayOneShot(uiSounds[0]);
        StartCoroutine(ToggleTimer(.25f));
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

    public void ResetAllDataBT()
    {
        UnlockController unlockC = FindAnyObjectByType<UnlockController>();
        SceneBuilder builder = controller.builder;
        unlockC.ResetAllData();
        builder.ReLoadScene();
    }

    public void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        var uniqueResolutions = new List<Resolution>();

        foreach (var res in resolutions)
        {
            // Check if we already added this width/height combo
            bool exists = uniqueResolutions.Exists(r =>
                r.width == res.width && r.height == res.height);

            if (exists)
                continue;

            uniqueResolutions.Add(res);
            options.Add($"{res.width} x {res.height}");

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = uniqueResolutions.Count - 1;
            }
        }

        resolutions = uniqueResolutions.ToArray();

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        currentResolutionIndex = resolutionIndex;
        AdjustUIScaling();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        AdjustUIScaling();  
    }

    public void CloseSettings()
    {
        
        isToggling = true;
        Cursor.visible = false;
        settingsUI.SetActive(false);
        uiActive = false;

        compassObj.SetActive(true);
        lowerUIobj.SetActive(true);

        StartCoroutine(ToggleTimer(.25f));
    }

    public void OpenSettings()
    {
        LoadResolutions();
        isToggling = true;
        settingsUI.SetActive(true);
        settingsConfirmBT.Select();
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Clamp01(value / 10f); // Convert 0–10 slider to 0–1 range
        masterMixer.SetFloat("MusicVol", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Clamp01(value / 10f); // Convert 0–10 slider to 0–1 range
        masterMixer.SetFloat("SFXVol", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    public void ConfirmSettings()
    {
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if (!isToggling && controller.active && controller.gameState == SceneController.GameState.Dungeon)
        {
            if (!uiActive)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    OpenMenuUI();
                }
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    if (!inventoryUI.gameObject.activeSelf)
                    {
                        isToggling = true;
                        Cursor.visible = true;
                        compassObj.SetActive(false);
                        lowerUIobj.SetActive(false);
                        uiActive = true;

                        inventoryUI.gameObject.SetActive(true);
                        OpenDunInventory();
                        StartCoroutine(ToggleTimer(.25f));
                    }
                }
                if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.JoystickButton6))
                {
                    if (!characterUI.gameObject.activeSelf)
                    {
                        PartyController party = FindAnyObjectByType<PartyController>();
                        isToggling = true;
                        Cursor.visible = true;
                        compassObj.SetActive(false);
                        lowerUIobj.SetActive(false);
                        uiActive = true;

                        characterUI.gameObject.SetActive(true);
                        characterUI.LoadStats(party.activeParty[0]);
                        StartCoroutine(ToggleTimer(.25f));
                    }
                }
            }

            if (uiActive && !isToggling)
            {                
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
                {
                    if (controller.dunFull)
                    {
                        CloseMenuUI();
                    }
               
                    StartCoroutine(ToggleTimer(.25f));

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
                        CloseSettings();
                    }
                    if (characterUI.gameObject.activeSelf)
                    {
                        if (characterUI.uiType == CharacterUI.UIType.CMenu)
                        {
                            characterUI.CloseUI();
                        }
                    }
                    if (shopUI.gameObject.activeSelf)
                    {
                        shopUI.CloseUI();
                        CloseDunInventory();
                    }
                    if (spellSmithUI.gameObject.activeSelf)
                    {
                        spellSmithUI.CloseUI();
                    }
                }
                if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.JoystickButton2))
                {
                    if (inventoryUI.gameObject.activeSelf)
                    {
                        isToggling = true;
                        Cursor.visible = false;
                        CloseDunInventory();    
                        StartCoroutine(ToggleTimer(.25f));
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
