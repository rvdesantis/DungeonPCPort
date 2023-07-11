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
    public GameObject repairFloor;
  
}
