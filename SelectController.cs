using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using System;

public class SelectController : MonoBehaviour
{
    public PartyController party;
    public SceneBuilder builder;
    public SceneController controller;
    public CharacterUI characterUI;
    public int selectIndex;

    public List<DunModel> partyModels;
    public List<CinemachineVirtualCamera> vCams;
    public List<int> camIndexList;
    public PlayableDirector startPlayable;
    public Action delayAction;

    public void FinalizeHeroes()
    {
        foreach (DunModel activeOption in party.masterParty)
        {
            if (activeOption.gameObject.activeSelf)
            {
                int x = party.masterParty.IndexOf(activeOption);
                partyModels.Add(activeOption);
                camIndexList.Add(x);
            }
        }
        foreach (BattleModel battle in party.combatMaster)
        {
            battle.health = battle.maxH;
            battle.mana = battle.maxM;
        }

        FinalizeCams();
    }

    public void FinalizeCams()
    {
        foreach (CinemachineVirtualCamera cam in vCams)
        {
            bool active = false;
            int y = vCams.IndexOf(cam);
            foreach(int indexS in camIndexList)
            {
                if (y == indexS)
                {
                    active = true;
                    break;
                }
            }

            if (!active)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

    public void SelectStart()
    {
        if (!controller.active)
        {
            delayAction = controller.SceneStart;
        }
        selectIndex = 0;
        characterUI.LoadStats(partyModels[0], true);
        StartCoroutine(SelectTimer());
    }

    IEnumerator SelectTimer()
    {
        startPlayable.Play();
        vCams[0].m_Priority = 10;
        yield return new WaitForSeconds((float)startPlayable.duration);
        builder.StartBuild();
    }

    public void AddToParty()
    {  
        int x = party.masterParty.IndexOf(partyModels[selectIndex]);

        party.activeParty.Add(party.masterParty[x]);
        party.combatParty.Add(party.combatMaster[x]);

        partyModels[selectIndex].gameObject.SetActive(false);

        if (party.activeParty.Count == 3)
        {
            party.LoadCounters();
            if (builder.frameBuild)
            {
                controller.SceneStart();
                characterUI.CloseUI();
                foreach (CinemachineVirtualCamera cam in vCams)
                {
                    cam.m_Priority = -1;
                }
                
            }
            if (!builder.frameBuild)
            {
                characterUI.CloseUI();
                foreach (CinemachineVirtualCamera cam in vCams)
                {
                    cam.m_Priority = -1;
                    controller.playerController.firstPersonCam.depth = 1;
                    controller.playerController.cinPersonCam.m_Priority = 5;
                }
            }
        }

        if (party.activeParty.Count < 3)
        {
           RightArrowAdd();
        }
    }

    

    int IndexChecker(bool left)
    {

        int updatedIndex = 0;
        int checkIndex = 0;
        bool activeM = false;

        if (left)
        {
            if (selectIndex != 0)
            {
                checkIndex = selectIndex - 1;
                if (partyModels[checkIndex].gameObject.activeSelf)
                {
                    updatedIndex = checkIndex;
                    activeM = true;
                }
                if (!activeM)
                {
                    checkIndex = selectIndex - 2;
                    if (checkIndex < 0)
                    {
                        selectIndex = 0; // triggers the below IF to wrap to top of list
                    }
                    if (checkIndex >= 0)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                }
                if (!activeM)
                {
                    checkIndex = selectIndex - 3; // shouldn't need to exceed 3 as that is maximum party size.  Shouldn't be more than 2 inactive next to each other at any time
                    if (checkIndex < 0)
                    {
                        selectIndex = 0; // triggers the below IF to wrap to top of list
                    }
                    if (checkIndex >= 0)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                }
            }
            if (selectIndex == 0)
            {
                checkIndex = partyModels.Count - 1;
                if (partyModels[checkIndex].gameObject.activeSelf)
                {
                    updatedIndex = checkIndex;
                    activeM = true;
                }
                if (!activeM)
                {
                    checkIndex = partyModels.Count - 2;
                    if (checkIndex < 0)
                    {
                        selectIndex = 0; // triggers the below IF to wrap to top of list
                    }
                    if (checkIndex >= 0)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                }
                if (!activeM)
                {
                    checkIndex = partyModels.Count - 3; // shouldn't need to exceed 3 as that is maximum party size.  Shouldn't be more than 2 inactive next to each other at any time
                    if (checkIndex < 0)
                    {
                        selectIndex = 0; // triggers the below IF to wrap to top of list
                    }
                    if (checkIndex >= 0)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                }
            }
           
        }
        if (!left)
        {
            if (selectIndex < partyModels.Count - 1)
            {
                checkIndex = selectIndex + 1;
                if (partyModels[checkIndex].gameObject.activeSelf)
                {
                    updatedIndex = checkIndex;
                    activeM = true;
                }
                if (!activeM)
                {
                    checkIndex = selectIndex + 2;
                    if (checkIndex <= partyModels.Count - 1)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                    if (checkIndex > partyModels.Count - 1)
                    {
                        selectIndex = partyModels.Count - 1; 
                    }
                }
                if (!activeM)
                {
                    checkIndex = selectIndex + 3; // shouldn't need to exceed 3 as that is maximum party size.  Shouldn't be more than 2 inactive next to each other at any time

                    if (checkIndex <= partyModels.Count - 1)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }

                    if (checkIndex > partyModels.Count - 1)
                    {
                        selectIndex = partyModels.Count - 1; // triggers the below IF to wrap to top of list
                    }
                }
            }
            if (selectIndex == partyModels.Count - 1)
            {
                checkIndex = 0;
                if (partyModels[checkIndex].gameObject.activeSelf)
                {
                    updatedIndex = checkIndex;
                    activeM = true;
                }
                if (!activeM)
                {
                    checkIndex = 1;
                    if (checkIndex <= partyModels.Count - 1)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }
                    if (checkIndex > partyModels.Count - 1)
                    {
                        selectIndex = 0;
                    }
                }
                if (!activeM)
                {
                    checkIndex = 2; // shouldn't need to exceed 3 as that is maximum party size.  Shouldn't be more than 2 inactive next to each other at any time

                    if (checkIndex <= partyModels.Count - 1)
                    {
                        if (partyModels[checkIndex].gameObject.activeSelf)
                        {
                            updatedIndex = checkIndex;
                            activeM = true;
                        }
                    }

                    if (checkIndex > partyModels.Count - 1)
                    {
                        selectIndex = 0; // triggers the below IF to wrap to top of list
                    }
                }
            }
        }


        return updatedIndex;
    }

    public void LeftArrowAdd()
    {
        int last = selectIndex;

        selectIndex = IndexChecker(true);

        characterUI.toggling = true;
        characterUI.LoadStats(partyModels[selectIndex], true);

        int oldCam = camIndexList[last];
        int newCam = camIndexList[selectIndex];

        vCams[oldCam].m_Priority = -1;
        vCams[newCam].m_Priority = 10;
    }

    public void RightArrowAdd()
    {
        int last = selectIndex;
        selectIndex = IndexChecker(false);

        characterUI.toggling = true;
        characterUI.LoadStats(partyModels[selectIndex], true);

        int oldCam = camIndexList[last];
        int newCam = camIndexList[selectIndex];

        vCams[oldCam].m_Priority = -1;
        vCams[newCam].m_Priority = 10;
        
    }
}
