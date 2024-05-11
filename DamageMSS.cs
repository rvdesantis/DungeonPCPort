using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Linq.Expressions;

public class DamageMSS : MonoBehaviour
{
    public Canvas damCanvas;
    public TextMeshProUGUI damTXT;
    public CinemachineVirtualCamera activeCam;
    public Animator canvasAnim;

    public float floatHeight = 5f; // Desired height to float to
    public float floatDuration = 2f; // Time taken to float

    private float startTime;
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;
        targetPos = startPos + (Vector3.up * floatHeight);
    }

    public void ShowDamage(int damage, GameObject targetObj)
    {
        transform.LookAt(activeCam.transform);
        damTXT.text = damage.ToString();
        StartCoroutine(DamageTimer());
    }

    IEnumerator DamageTimer()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    void Update()
    {
        float journeyLength = Vector3.Distance(startPos, targetPos);
        float distCovered = (Time.time - startTime) * journeyLength / floatDuration;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startPos, targetPos, fracJourney);
    }

}
