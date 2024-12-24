using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using System.Linq.Expressions;
using DTT.Utils.Extensions;

public class DamageMSS : MonoBehaviour
{
    public Canvas damCanvas;
    public TextMeshProUGUI damTXT;
    public CinemachineVirtualCamera activeCam;
    public Animator canvasAnim;
    public BattleCamController bCamController;

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
        bCamController = FindObjectOfType<BattleCamController>();
    }

    public void ShowDamage(int damage, bool crit = false)
    {
        transform.LookAt(activeCam.transform);
        damTXT.text = damage.ToString();
        if (crit)
        {
            damTXT.text = damTXT.text + "(CRIT)";
        }
        StartCoroutine(DamageTimer());
    }

    public void ShowHeal(int healamount, bool crit = false)
    {
        transform.LookAt(activeCam.transform);
        damTXT.text = healamount.ToString();
        damTXT.color = Color.green;
        if (crit)
        {
            damTXT.text = damTXT.text + "(CRIT)";
        }
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
        transform.LookAt(bCamController.activeCam.transform);
    }

}
