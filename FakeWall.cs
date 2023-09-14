using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Playables;

public class FakeWall : MonoBehaviour
{
    public BoxCollider blockCollider;
    public bool inRange;
    public bool wallBroken;
    public bool wallBreakTest;
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
        if (!wallBroken)
        {
            if (wallBreakTest)
            {
                WallBreak();
            }
        }

        if (inRange && !wallBroken)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallBreak();
            }
        }
    }
}
