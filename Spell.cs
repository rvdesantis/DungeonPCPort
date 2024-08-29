using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spell : MonoBehaviour
{
    public string spellName;
    public string spellInfo;
    public enum SpellType { heal, fire, thunder, voidMag, ice, nature, offense, defense}
    public SpellType spellType;
    public int spellCost;
    public int baseDamage;
    public float spellSpeed;
    public float impactTime;
    public float castTime;
    private bool isMoving;
    private Vector3 spawnPoint;
    private Vector3 impactPos;
    public Sprite spellIcon;

    private Vector3 spellDirection;
    public enum CastType { projectile, AOE, }
    public CastType castType;

    public Transform targetTransform;
    public enum SpellTargeting { enemies, party, all}
    public SpellTargeting spellTargeting;

    public List<ParticleSystem> spellFX;
    public List<ParticleSystem> spellImpactFX;
    public RFX4_EffectSettings kriptoController;

    public void CastSpell(Vector3 spellSpawnPoint, Vector3 impactPosition)
    {
        if (spellFX.Count > 0)
        {
            foreach (ParticleSystem part in spellFX)
            {
                part.gameObject.SetActive(true);
                part.Play();
            }
        }

        if (castType == CastType.projectile)
        {
            spawnPoint = spellSpawnPoint;
            impactPos = impactPosition;
            spellDirection = (impactPosition - spellSpawnPoint).normalized;
            transform.position = spellSpawnPoint;
            isMoving = true;
        }

        if (castType == CastType.AOE)
        {
            spawnPoint = spellSpawnPoint;
            impactPos = impactPosition;
            transform.position = spellSpawnPoint;
            StartCoroutine(AOETimer());
        }
    }

    IEnumerator AOETimer()
    {

        yield return new WaitForSeconds(castTime);
        if (spellImpactFX.Count > 0)
        {
            foreach(ParticleSystem part in spellImpactFX)
            {
                ParticleSystem newImpact = Instantiate(part, impactPos, transform.rotation);
                newImpact.Play();
            }
        }
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isMoving && castType == CastType.projectile)
        {  
            transform.Translate(spellDirection * spellSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, spawnPoint) >= Vector3.Distance(impactPos, spawnPoint))
            {
                isMoving = false;
                if (spellImpactFX.Count > 0)
                {
                    foreach (ParticleSystem part in spellImpactFX)
                    {
                        ParticleSystem newImpact = Instantiate(part, impactPos, transform.rotation);
                        newImpact.Play();
                    }
                }
            }
        }
    }


}
