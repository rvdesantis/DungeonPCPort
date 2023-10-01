using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCam;
    public CinemachineBrain cinBrain;
    public CinemachineVirtualCamera cinPersonCam;
    public CharacterController controller;
    public SceneController sceneController;
    public PlayerSoundController playerSound;
    public Light playerLight;
    public Transform playerBody;
    public float mouseSensitivity = 80f;

    float xRotation = 0;
    private float currentRotation = 0f;
    public float rotationTime = .5f;
    public float distanceTraveled;
    public float steps;
    public int stepsForLaunch;

    public float moveSpeed = 12;
    public bool walking;
    public bool turning = false;
    public float mouseTurnSensativity; // between 0 - 1

    private Vector3 moveDirection = Vector3.zero;
    private Quaternion targetRotation;
    public Vector3 lastPosition;
    public bool active;

    public float gravity = 9.8f; // Adjust this value to control the strength of gravity
    public float groundDistance = 0.2f; // Adjust this value based on your player's height and ground detection accuracy


    private Vector3 velocity;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    private void Start()
    {
        targetRotation = transform.rotation;
        lastPosition = transform.position;
    }

    public void CheckMove()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0;

        if (isMoving)
        {
            walking = true;
            // Your movement code here
        }
        else
        {
            walking = false;            
        }
    }

    private IEnumerator RotatePlayer(Quaternion targetRotation, float rotationTime)
    {
        float t = 0;
        Quaternion startRotation = transform.rotation;

        while (t < rotationTime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / rotationTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);
            yield return null;
        }

        transform.rotation = targetRotation;
        turning = false;
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            velocity.y = 0f; // Reset vertical velocity when grounded
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime; // Apply gravity
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Mouse X");

        float joystickHorizontalInput = Input.GetAxis("Joystick Horizontal");
        float joystickVerticalInput = Input.GetAxis("Joystick Vertical");
        float joystickRightHorizontalInput = Input.GetAxis("Joystick Right Horizontal");
        float joystickRightVerticalInput = Input.GetAxis("Joystick Right Vertical");

        if (active && !sceneController.uiController.uiActive)
        {
            ApplyGravity();
            CheckMove();
            moveDirection = transform.forward * verticalInput * moveSpeed;
            moveDirection += transform.right * horizontalInput * moveSpeed;
            controller.Move(moveDirection * Time.deltaTime);
            
            

            if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button4)) && !turning)
            {
                turning = true;
                currentRotation -= 90f;
                currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                targetRotation = Quaternion.Euler(0, currentRotation, 0);
                StartCoroutine(RotatePlayer(targetRotation, rotationTime));
            }
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button5)) && !turning)
            {
                turning = true;
                currentRotation += 90f;
                currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                targetRotation = Quaternion.Euler(0, currentRotation, 0);
                StartCoroutine(RotatePlayer(targetRotation, rotationTime));
            }
            else if (Mathf.Abs(rotationInput) > 0.1f)
            {
                if (rotationInput < -mouseTurnSensativity & !turning)
                {
                    turning = true;
                    currentRotation -= 90f;
                    currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                    targetRotation = Quaternion.Euler(0, currentRotation, 0);
                    StartCoroutine(RotatePlayer(targetRotation, rotationTime));
                }
                if (rotationInput > mouseTurnSensativity & !turning)
                {
                    turning = true;
                    currentRotation += 90f;
                    currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                    targetRotation = Quaternion.Euler(0, currentRotation, 0);
                    StartCoroutine(RotatePlayer(targetRotation, rotationTime));
                }
            }
            // Joystick
            if (Mathf.Abs(joystickRightHorizontalInput) > 0.1f && active)
            {
                if (joystickRightHorizontalInput > 0.5f && !turning)
                {
                    turning = true;
                    currentRotation += 90f;
                    currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                    targetRotation = Quaternion.Euler(0, currentRotation, 0);
                    StartCoroutine(RotatePlayer(targetRotation, rotationTime));
                }
                if (joystickRightHorizontalInput < -0.5f && !turning)
                {
                    turning = true;
                    currentRotation -= 90f;
                    currentRotation = Mathf.Round(currentRotation / 90f) * 90f;
                    targetRotation = Quaternion.Euler(0, currentRotation, 0);
                    StartCoroutine(RotatePlayer(targetRotation, rotationTime));
                }
            }

            if (Mathf.Abs(joystickVerticalInput) > 0.1f)
            {
                moveDirection = transform.forward * joystickVerticalInput * moveSpeed;
            }
        }
    }
}
