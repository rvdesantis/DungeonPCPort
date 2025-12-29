using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    public Canvas titleUICanvas;
    [SerializeField] private CanvasScaler canvasScaler;


    public TextMeshProUGUI loadingTXT; // Reference to the TMP text component
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    public GameObject menuBack;

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    [SerializeField] private AudioMixer masterMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Start loading the scene
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        LoadResolutions();

        menuBack.SetActive(true);
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

        // Force the UI to refresh
        Canvas.ForceUpdateCanvases();

        // --- Add Ultrawide Detection + Scaling ---
        AdjustUIScaling();
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

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void ConfirmSettings()
    {
        loadingTXT.gameObject.SetActive(true);
        PlayerPrefs.Save();
        StartCoroutine(LoadSceneAsync("Builder"));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(.5f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);


        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
