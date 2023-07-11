using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Playables;

public class FakeWall : MonoBehaviour
{
    public BoxCollider blockCollider;
    public bool inRange;
    public bool wallBreak;
    public bool wallBreakTest;
    public PlayableDirector standardBreak;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator Break()
    {
        if (!wallBreak)
        {
            wallBreak = true;
            standardBreak.Play();
        }
        yield return new WaitForSeconds(1);
        blockCollider.enabled = false;
    }

    public void WallBreak()
    {
        StartCoroutine(Break());        
    }

    // Update is called once per frame
    void Update()
    {
        if (!wallBreak)
        {
            if (wallBreakTest)
            {
                WallBreak();
            }
        }

        if (inRange && !wallBreak)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WallBreak();
            }
        }
    }
}
