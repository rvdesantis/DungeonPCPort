using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using System.Collections;

public class TrapHallCube : Cube
{
    public enum TrapType { enemy, chest, NPC, other, empty }
    public TrapType trapType;
    public FakeFloor fakeFloor;
    public List<GameObject> fallTubes;
    public GameObject fallRoom;
    public GameObject fallRoomSpawnPoint;
    public PlayableDirector fallDirector;
    public List<PlayableDirector> landingDirectors;
    public List<PlayableDirector> monsterDirectors;
    public GameObject repairFloor;


    private void Start()
    {
        TestSet();
    }

    public void TestSet()
    {
        int num = Random.Range(0, 2);
        if (num == 0)
        {
            trapType = TrapType.empty;
        }
        if (num == 1)
        {
            trapType = TrapType.enemy;
        }
    }
}
