using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FakeWall : MonoBehaviour
{
    public SceneController controller;
    public BoxCollider blockCollider;
    public bool inRange;
    public bool wallBroken;
    public bool locked;
    public GameObject objectSetActive;
    public PlayableDirector standardBreak;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator Break()
    {
        if (!wallBroken)
        {
            wallBroken = true;
            standardBreak.Play();
            if (objectSetActive != null)
            {
                objectSetActive.SetActive(true);
            }
            yield return new WaitForSeconds(1);
            blockCollider.enabled = false;
        }
    }

    public void WallBreak()
    {
        StartCoroutine(Break());        
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && !wallBroken && !locked)
        {
            if (controller == null)
            {
                controller = FindObjectOfType<SceneController>();
            }
            if (controller.gameState == SceneController.GameState.Dungeon && controller.playerController.enabled && !controller.uiController.uiActive)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    WallBreak();
                }
            }
        }
    }
}
