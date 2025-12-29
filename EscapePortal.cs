using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePortal : DunItem
{
    public bool test;
    public List<AudioClip> audioClips; // uses audiosource on Player
    public override void UseItem(DunModel target = null, BattleModel battleTarget = null)
    {
        StartCoroutine(Dip());
    }

    IEnumerator Dip()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        SanctuaryCube sanctuary = FindAnyObjectByType<SanctuaryCube>();

        Vector3 currentPos = player.transform.position;
        Vector3 dipPosition = new Vector3(player.transform.position.x, player.transform.position.y - 2, player.transform.position.z);
        Vector3 returnPosition = sanctuary.floorPoint.transform.position;

        player.audioSource.PlayOneShot(audioClips[0]);

        ParticleSystem groundParticles = Instantiate(player.vfxLIST[2], player.transform.position, player.vfxLIST[2].transform.rotation);
        groundParticles.gameObject.SetActive(true);
        groundParticles.Play();

        yield return new WaitForSeconds(.1f);

        float elapsedTime = 0f;
        float duration = .25f; 

        while (elapsedTime < duration)
        {
            player.transform.position = Vector3.Lerp(currentPos, dipPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        player.transform.position = returnPosition;
        sanctuary.sanctPlayables[0].Play();
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            UseItem();
        }
    }
}
