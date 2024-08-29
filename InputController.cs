using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public enum ActiveInput { keyboard, joystick}
    public ActiveInput activeInput; 
    public SceneController controller;
    public DunUIController uiController;
    public ConfirmUI confirmUI;

    public bool isActionBTDown = false;
    public float timeHeldDown = 0f;
    public float timeNotHeld = 0f;
    public bool actionHOLD;
    public string[] joystickNames;
    public bool joyConnect;
    public bool joyDisconnect;

    private void ButtonHoldChecker()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Joystick1Button0))
        {
            if (!isActionBTDown)
            {
                isActionBTDown = true;
                timeHeldDown = 0f;
                timeNotHeld = 0f;
                uiController.skipUI.slider.value = 0;
                uiController.skipUI.slider.gameObject.SetActive(true);
                if (uiController.joystick && controller.activePlayable != null)
                {
                    uiController.skipUI.spaceBar.gameObject.SetActive(false);
                    uiController.skipUI.aButton.gameObject.SetActive(true);
                }
                if (!uiController.joystick && controller.activePlayable != null)
                {
                    uiController.skipUI.spaceBar.gameObject.SetActive(true);
                    uiController.skipUI.aButton.gameObject.SetActive(false);
                }
            }
            else
            {
                timeHeldDown += Time.deltaTime;
                uiController.skipUI.slider.value = timeHeldDown;
                // Check if the spacebar has been held down for the required time.
                if (timeHeldDown >= 1)
                {
                    actionHOLD = true;
                    Debug.Log("Spacebar held down for " + 1 + " seconds.");
                    // Perform your desired action here.
                    if (controller.activePlayable != null)
                    {
                        uiController.skipUI.spaceBar.gameObject.SetActive(false);
                        uiController.skipUI.aButton.gameObject.SetActive(false);
                        uiController.skipUI.slider.value = 0;
                        uiController.skipUI.slider.gameObject.SetActive(false);

                        controller.SkipScene();
                        actionHOLD = false;
                        isActionBTDown = false;
                    }
                }
            }
        }
        else
        {
            isActionBTDown = false;
            timeHeldDown = 0f;
            uiController.skipUI.spaceBar.gameObject.SetActive(false);
            uiController.skipUI.aButton.gameObject.SetActive(false);
            uiController.skipUI.slider.value = 0;
            uiController.skipUI.slider.gameObject.SetActive(false);
        }
    }

    void CheckJoystickConnection()
    {
        joystickNames = Input.GetJoystickNames();
        if (joystickNames.Length == 1 && !joyConnect)
        {
            if (string.IsNullOrEmpty(joystickNames[0]))
            {
               
            }
            if (!string.IsNullOrEmpty(joystickNames[0]))
            {
                joyConnect = true;
                if (activeInput == ActiveInput.keyboard)
                {
                    activeInput = ActiveInput.joystick;
                }
                if (joyDisconnect == true)
                {
                    Debug.Log("Joystick RECONNECTED");
                    joyDisconnect = false;
                }
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1f;
                }
            }
        }
        if (joystickNames.Length == 1 && joyConnect)
        {
            if (string.IsNullOrEmpty(joystickNames[0]))
            {
                joyConnect = false;
                joyDisconnect = true;
                if (Time.timeScale != 0)
                {
                    Debug.Log("ERROR - Joystick Disconnected");
                    Time.timeScale = 0f;
                    string mss = "Game Paused - Controller Disconnected\nPlease Reconnect PC Controller";
                    confirmUI.ConfirmMessageUI(mss, false, false, false, true);
                }
            }
            if (!string.IsNullOrEmpty(joystickNames[0]))
            {

            }
        }


    }

    private void Update()
    {
        ButtonHoldChecker();
        CheckJoystickConnection();
    }

}
