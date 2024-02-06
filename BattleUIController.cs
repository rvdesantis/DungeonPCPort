using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public Button endBattleBT;
    public BattleController battleC;
    public GameObject fadeImageObj;
    public Animator fadeImageAnim;
    public PlayableDirector fadeDarkPlayable;
    
    public void ReturnToDungeon()
    {
        battleC.BattleRewards();
        
        endBattleBT.gameObject.SetActive(false);
        battleC.activeRoom.mainCam.m_Priority = -1;

        battleC.sceneController.playerController.cinPersonCam.m_Priority = 10;
        battleC.sceneController.playerController.controller.enabled = true;
        battleC.sceneController.uiController.compassObj.SetActive(true);

        battleC.ClearBattle();
        if (battleC.afterBattleAction !=null)
        {
            battleC.afterBattleAction.Invoke();
        }
        battleC.afterBattleAction = null;

        battleC.sceneController.gameState = SceneController.GameState.Dungeon;
    }




    public void AssignFadeImage(PlayableDirector dir) // always assign fade animator to last output position
    {
        BattleController battleC = FindObjectOfType<BattleController>();

        PlayableGraph graph = dir.playableGraph;
        

        if (graph.IsValid())
        {
            Debug.Log("Assigning Fade Screen");
            int graphCount = graph.GetOutputCount();

            int posNum = graphCount - 1;
            PlayableBinding playableBinding = dir.playableAsset.outputs.ElementAt(posNum);
            dir.SetGenericBinding(playableBinding.sourceObject, fadeImageAnim);
        }
        else
        {
            Debug.LogError("The PlayableGraph is not valid. Make sure it is properly initialized.");
        }



    }
}
