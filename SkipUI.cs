using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipUI : MonoBehaviour
{
    public Image spaceBar;
    public Image aButton;
    public Slider slider;
    public SceneController controller;

    private void Update()
    {
        if (controller.activePlayable == null)
        {
            slider.gameObject.SetActive(false);
            spaceBar.gameObject.SetActive(false);
            aButton.gameObject.SetActive(false);
        }
    }
}
