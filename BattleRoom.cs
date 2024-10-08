using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class BattleRoom : MonoBehaviour
{
    public BattleController battleC;
    public RoomPropParent activeRoomParent;

    public List<GameObject> playerSpawnPoints;
    public List<GameObject> enemySpawnPoints;
 
    public List<GameObject> activeObjects;
    public List<DunItem> battleTrinkets;
    public CinemachineVirtualCamera mainCam;
    public List<CinemachineVirtualCamera> targetingCams;
    public PlayableDirector introPlayable;
    public List<PlayableDirector> intros;
    public float comboOffset;
    public Transform afterBattleSpawnPoint;    

    public void SetProps(int monsterNum) // for small & large room, roof gameobject is set by battle intro playable
    {      
        activeObjects[monsterNum].gameObject.SetActive(true);
    }

    public void IntroTimer()
    {
        if (battleC == null)
        {
            battleC = FindObjectOfType<BattleController>();
        }
        mainCam.m_Priority = 20;
        if (introPlayable != null)
        {
            introPlayable.Play();
            StartCoroutine(IntroTimerRoutine());
        }
        if (introPlayable == null)
        {
            foreach (BattleModel heroMod in battleC.heroParty)
            {
                heroMod.anim.SetTrigger("unsheath");
            }
        }
    }

    IEnumerator IntroTimerRoutine()
    {
        yield return new WaitForSeconds((float)introPlayable.duration);
        foreach (BattleModel heroMod in battleC.heroParty)
        {
            heroMod.anim.SetTrigger("unsheath");
        }
    }
}
