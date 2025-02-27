using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Unity.VisualScripting;
using System.Linq.Expressions;


public class SceneBuilder : MonoBehaviour
{
    public int targetSize;
    public int currentSize;

    public List<HallStarterCube> sanctStarters;
    public Cube blankCube;
    public List<Cube> blankExtenderPreLoads;
    public Cube blankColliderTestCube;
    public Cube bossHallCube;
    public SideExtenderCube sideCubeExtender;
    public TrapHallCube trapHallCube;

    public List<Cube> allTurns;
    public List<Cube> lTurnPreLoads;
    public List<Cube> rTurnPreLoads;
    public List<Cube> tTurnPreLoads;

    public List<Cube> allRooms;
    public List<Cube> sRoomPreLoads;
    public List<Cube> lRoomPreLoads;
    public int loopCount;
    public List<Cube> allDeadEnds;
    public List<Cube> allBossRooms;

    public Cube sanctuary;
    public List<Cube> createdStarters;

    public List<Cube> hall0Starters;
    public List<Cube> hall1Starters;
    public List<Cube> hall2Starters;
    public int hallBuildIndex;

    public List<Cube> startersBlocked;
    public List<Cube> createdHallCubes;
    public List<Cube> hallcubesBlocked;
    public List<CubeRoom> createdRooms;
    public List<TurnCube> createdTurns;
    public List<Cube> createdDeadEnds;
    public List<Cube> createdSecretEnds;
    public List<Cube> createdHallSideCubes;
    public List<Cube> createdTrapHalls;
    public List<BossCube> createdBossRooms;

    public PlayerController playerController;
    public SceneController sceneController;
    public UnlockController unlockables;
    public LoadingBarUI loadingBar;
    public bool buildChecked;
    public bool frameBuild;
    public int bossHallNum;
    public List<PlayableDirector> openingPlayables;

    

    private void Start()
    {
        PreBuild();
    }

    public void PreBuild()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        unlockables.OpenGameLoad();
        currentSize = 0;

        // add size controller.  targetSize Default set to 500 in Inspecter.

        if (targetSize == 0) // checks for error, sets to small by default if 0
        {
            Debug.Log("ERROR - Target Size Set to 0");
            targetSize = 250;
        }
        loadingBar.skullSlider.value = 0;

        sceneController.partySelect.FinalizeHeroes();

        foreach (PlayableDirector opening in openingPlayables)
        {
            opening.Play();
        }

        NewStartBuild();
    }

    public void StartBuild()
    {
        Debug.Log("Starting Build with " + sanctStarters.Count + " Santuary Starting Cubes");
        foreach (HallStarterCube starter in sanctStarters)
        {
            createdStarters.Add(starter);
            starter.builder = this;     
        }

        StartCoroutine(StartTimer());

        IEnumerator StartTimer()
        {
            BuildOldHallway(sanctStarters[0]);
            BuildOldHallway(sanctStarters[1]); 
            BuildOldHallway(sanctStarters[2]);
            yield return new WaitForSeconds(.25f);
            foreach (GameObject fog in sanctuary.fogWalls)
            {
                fog.SetActive(true);
            }
            loadingBar.text.text = "Building Dungeon...";
            //StartCoroutine(StarterCubeCheck());
        }          
    }

    public void NewStartBuild()
    {
        Debug.Log("Starting Build with Hallway 0", sanctStarters[0].gameObject);
        hall0Starters.Add(sanctStarters[0]);
        createdStarters.Add(sanctStarters[0]);

        hall1Starters.Add(sanctStarters[1]);
        createdStarters.Add(sanctStarters[1]);

        hall2Starters.Add(sanctStarters[2]);
        createdStarters.Add(sanctStarters[2]);

        StartCoroutine(StartTimer());

        IEnumerator StartTimer()
        {
            StartCoroutine(BuildNewHallway(sanctStarters[0]));
            yield return new WaitForSeconds(.25f);

            sanctuary.fogWalls[0].gameObject.SetActive(true);

            loadingBar.text.text = "Building Dungeon...";
            //StartCoroutine(StarterCubeCheck());
        }
    }

    public void ReLoadScene()
    {
        SceneManager.LoadScene("Title Screen");
    }

    public void BuildOldHallway(HallStarterCube starter)
    {
        int maxLength = 10;
        bool blocked = false;
        int hallLength = Random.Range(3, maxLength);
        if (starter == null)
        {
            Debug.Log("Error, Startercube 'Starter' not imported to BuildHallway()");
        }
 
        starter.hallType = HallStarterCube.HallType.large;
        Cube firstCube = null;
        if (blankExtenderPreLoads.Count == 0)
        {
            firstCube = Instantiate(blankCube, starter.transform.position, starter.transform.rotation);
        }
        if (blankExtenderPreLoads.Count > 0)
        {
            firstCube = blankExtenderPreLoads[0];
            firstCube.transform.position = starter.transform.position;
            firstCube.transform.rotation = starter.transform.rotation;
            firstCube.gameObject.SetActive(true);
            blankExtenderPreLoads.Remove(firstCube);
            createdHallCubes.Add(firstCube);
        }      
        float cubeLength = firstCube.lengthMesh.bounds.size.z;

        for (int i = 1; i < hallLength; i++)
        {
            Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
            Cube newCube = null;

            if (blankExtenderPreLoads.Count == 0)
            {
                newCube = Instantiate(blankCube, position, firstCube.transform.rotation);
            }
            if (blankExtenderPreLoads.Count > 0)
            {
                newCube = blankExtenderPreLoads[0];
                newCube.transform.position = position;
                newCube.transform.rotation = firstCube.transform.rotation;
                newCube.gameObject.SetActive(true);
                blankExtenderPreLoads.Remove(newCube);
         
            }

            starter.generatedHallway.Add(newCube);
            createdHallCubes.Add(newCube);
            currentSize++;
        }
        Physics.SyncTransforms();

        foreach (Cube hallbox in starter.generatedHallway)
        {            
            hallbox.positioner.SetActive(false);
            hallbox.collisionChecker.enabled = true;
            bool collision = false;
            Collider[] colliders = Physics.OverlapBox(hallbox.collisionChecker.bounds.center, hallbox.collisionChecker.bounds.extents);
            if (colliders.Length > 1)
            {
                collision = true;
                Debug.Log("BoxChecker Collision with (GameObject) " + colliders[0].gameObject.name, hallbox.gameObject);
            }
            hallbox.collisionChecker.enabled = false;
            hallbox.positioner.SetActive(true);
            if (collision)
            {
                blocked = true;
                break;
            }
        }
     
        if (!blocked) // end checker
        {
            foreach (Cube turn in allTurns)
            {
                int x = starter.generatedHallway.Count;
                if (x > 1)
                {
                    Cube lastHallCube = starter.generatedHallway[x - 1];
                    Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;

                    turn.gameObject.SetActive(true);
                    turn.transform.position = position;
                    turn.transform.rotation = lastHallCube.transform.rotation;

                    TurnCube turnCheck = turn.GetComponent<TurnCube>();
                    turnCheck.turnCollider.enabled = true;
                    lastHallCube.gameObject.SetActive(false);
                    if (turnCheck.TurnChecker())
                    {
                        blocked = true;
                    }

                    lastHallCube.gameObject.SetActive(true);
                    turnCheck.turnCollider.enabled = false;
                    turn.gameObject.SetActive(false);
                }
            }
        }

        // finalize

        if (!blocked)
        {           
            firstCube.gameObject.SetActive(false);
            starter.hallBuildFin = true;            
            float loading = (float)currentSize / (float)targetSize;
            float adjusted = loading / 2;
            loadingBar.skullSlider.value = adjusted;
            StartCoroutine(EndHallway(starter, starter.generatedHallway[starter.generatedHallway.Count - 1]));
        }

        if (blocked)
        {
            Debug.Log("Starter Hallway Blocked, adding Dead End (GameObject)", starter.gameObject);
            starter.hallBuildFin = true;
            startersBlocked.Add(starter);
            DeadEndCube newDeadEnd = starter.deadEnd;
            starter.hallType = HallStarterCube.HallType.deadEnd;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);

            starter.mapIcon.gameObject.SetActive(false);
            newDeadEnd.mapIcon.gameObject.SetActive(true);

            if (starter.generatedHallway.Count > 0)
            {
                foreach (Cube targetCube in starter.generatedHallway)
                {
                    currentSize--;
                    targetCube.gameObject.SetActive(false);
                    createdHallCubes.Remove(targetCube);
                    hallcubesBlocked.Add(targetCube);
                }
            }
            starter.generatedHallway.Clear();
        }
    }

    public IEnumerator BuildNewHallway(HallStarterCube starter)
    {
        if (starter == null)
        {
            Debug.Log("Error, Startercube 'Starter' not imported to BuildHallway()");
        }
        int maxLength = 15;
        int hallLength = Random.Range(5, maxLength);

        Cube firstCube = null;
        if (blankExtenderPreLoads.Count == 0)
        {
            firstCube = Instantiate(blankCube, starter.transform.position, starter.transform.rotation);
        }
        if (blankExtenderPreLoads.Count > 0)
        {
            firstCube = blankExtenderPreLoads[0];
            firstCube.transform.position = starter.transform.position;
            firstCube.transform.rotation = starter.transform.rotation;
            firstCube.gameObject.SetActive(true);
            blankExtenderPreLoads.Remove(firstCube);
            createdHallCubes.Add(firstCube);
        }
        float cubeLength = firstCube.lengthMesh.bounds.size.z;
        
        
        bool blocked = false;
        bool largeBlock = false;

        starter.largeHallwayCollider.gameObject.SetActive(true);
        starter.largeHallwayCollider.enabled = true;
            
        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();

        Collider[] colliders = Physics.OverlapBox(starter.largeHallwayCollider.bounds.center, starter.largeHallwayCollider.bounds.extents);
        if (colliders.Length > 1)
        {
            largeBlock = true;
            Debug.Log("Large Hall Checker Collision" + colliders[0].gameObject.name, starter.gameObject);
        }
        starter.largeHallwayCollider.enabled = false;
        starter.largeHallwayCollider.gameObject.SetActive(false);
        if (largeBlock)
        {
            hallLength = 5;
            starter.smallHallwayCollider.gameObject.SetActive(true);
            starter.smallHallwayCollider.enabled = true;

            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            Collider[] colliders2 = Physics.OverlapBox(starter.smallHallwayCollider.bounds.center, starter.smallHallwayCollider.bounds.extents);
            if (colliders2.Length > 1)
            {
                blocked = true;         
                Debug.Log("Small Hall Checker Collision" + colliders2[0].gameObject.name, starter.gameObject);
            }
            starter.smallHallwayCollider.enabled = false;
            starter.smallHallwayCollider.gameObject.SetActive(false);
        }       

        if (!blocked)
        {
            for (int i = 1; i < hallLength; i++)
            {
                Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
                Cube newCube = null;

                if (blankExtenderPreLoads.Count == 0)
                {
                    newCube = Instantiate(blankCube, position, firstCube.transform.rotation);
                }
                if (blankExtenderPreLoads.Count > 0)
                {
                    newCube = blankExtenderPreLoads[0];
                    newCube.transform.position = position;
                    newCube.transform.rotation = firstCube.transform.rotation;
                    newCube.gameObject.SetActive(true);
                    blankExtenderPreLoads.Remove(newCube);

                }
                starter.generatedHallway.Add(newCube);
                createdHallCubes.Add(newCube);
                currentSize++;
            }
        }
        if (!blocked) // end checker
        {
            foreach (Cube turn in allTurns)
            {
                int x = starter.generatedHallway.Count;
                if (x > 1)
                {
                    Cube lastHallCube = starter.generatedHallway[x - 1];
                    Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;

                    turn.gameObject.SetActive(true);
                    turn.transform.position = position;
                    turn.transform.rotation = lastHallCube.transform.rotation;

                    TurnCube turnCheck = turn.GetComponent<TurnCube>();
                    turnCheck.turnCollider.enabled = true;
                    lastHallCube.gameObject.SetActive(false);
                   
                    Physics.SyncTransforms();
                    yield return new WaitForFixedUpdate();
                    Collider[] colliders3 = Physics.OverlapBox(turnCheck.turnCollider.bounds.center, turnCheck.turnCollider.bounds.extents);
                    if (colliders3.Length > 1)
                    {
                        blocked = true;
                        Debug.Log("BoxChecker Collision" + colliders3[0].gameObject.name, colliders3[0].gameObject);
                    }      
                    lastHallCube.gameObject.SetActive(true);
                    turnCheck.turnCollider.enabled = false;
                    turn.gameObject.SetActive(false);
                }
            }
        }

        if (!blocked)
        {
            firstCube.gameObject.SetActive(false);
            hallcubesBlocked.Add(firstCube);
            starter.hallBuildFin = true;

            float loading = (float)currentSize / (float)targetSize;
            float adjusted = loading / 2;
            loadingBar.skullSlider.value = adjusted;

            foreach (Cube hallCubes in starter.generatedHallway)
            {   
                hallCubes.positioner.SetActive(true);
            }
            
            StartCoroutine(EndNewHallway(starter, starter.generatedHallway[starter.generatedHallway.Count - 1]));
        }

        if (blocked)
        {
            Debug.Log("Starter Hallway Blocked, adding Dead End (GameObject)", starter.gameObject);
            starter.hallBuildFin = true;
            startersBlocked.Add(starter);

            DeadEndCube newDeadEnd = starter.deadEnd;
            starter.hallType = HallStarterCube.HallType.deadEnd;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);

            starter.mapIcon.gameObject.SetActive(false);
            newDeadEnd.mapIcon.gameObject.SetActive(true);

            if (starter.generatedHallway.Count > 0)
            {
                foreach (Cube targetCube in starter.generatedHallway)
                {
                    currentSize--;
                    targetCube.gameObject.SetActive(false);
                    createdHallCubes.Remove(targetCube);
                    hallcubesBlocked.Add(targetCube);
                }
            }
            starter.generatedHallway.Clear();
            StartCoroutine(DungeonStatusChecker());
        }
    }

    public IEnumerator EndHallway(HallStarterCube starter, Cube lastHallCube) // adds Room or Turn or Dead End depending on cubecount;
    {
        if (currentSize >= targetSize)
        {
            Debug.Log("Target Size Reached, Closing Hallway");
            loadingBar.text.text = "Closing Dungeon...";
            DeadEndCube newDeadEnd = lastHallCube.cap;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);
            lastHallCube.mapIcon.gameObject.SetActive(false);
            starter.hallType = HallStarterCube.HallType.deadEnd;
        }
        if (currentSize < targetSize)
        {
            List<Cube> availTurns = new List<Cube>();
            Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;
            Cube newTurn = null;

            allRooms[0].gameObject.SetActive(true);
            allRooms[0].transform.position = position;
            allRooms[0].transform.rotation = lastHallCube.transform.rotation;

            CubeRoom roomCheck = allRooms[0].GetComponent<CubeRoom>();
            roomCheck.turnCollider.enabled = true;

            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            if (!roomCheck.TurnChecker())
            {
                availTurns.Add(allRooms[0]); // large room
                availTurns.Add(allRooms[1]); // small room
            }
            if (roomCheck.TurnChecker())
            {

            }
            roomCheck.turnCollider.enabled = false;
            allRooms[0].gameObject.SetActive(false);         


            foreach (Cube turnCube in allTurns)
            {
                turnCube.gameObject.SetActive(true);
                turnCube.transform.position = position;
                turnCube.transform.rotation = lastHallCube.transform.rotation;

                TurnCube turnChecker = turnCube.GetComponent<TurnCube>();
                turnChecker.turnCollider.enabled = true;

                Physics.SyncTransforms();
                yield return new WaitForFixedUpdate();

                if (!turnChecker.TurnChecker())
                {
                    availTurns.Add(turnCube);
                }
                if(turnChecker.TurnChecker())
                {

                }
                turnChecker.turnCollider.enabled = false;
                turnCube.gameObject.SetActive(false);
            }           

            if (availTurns.Count > 0)
            {
                int turnNum = Random.Range(0, availTurns.Count);
                Debug.Log("Total turn options for hall end = " + availTurns.Count, lastHallCube.gameObject);
                
               
                if (availTurns[turnNum] == allTurns[0])
                {
                    currentSize = currentSize + 3;
                    Debug.Log("Placing Left Turn", lastHallCube.gameObject);
                    if (lTurnPreLoads.Count == 0)
                    {
                        newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                        newTurn.gameObject.SetActive(true);
                    }
                    if (lTurnPreLoads.Count > 0)
                    {
                        newTurn = lTurnPreLoads[0];
                        newTurn.transform.position = position;
                        newTurn.transform.rotation = lastHallCube.transform.rotation;
                        newTurn.gameObject.SetActive(true);
                        lTurnPreLoads.Remove(newTurn);
                    }
                }
                if (availTurns[turnNum] == allTurns[1])
                {
                    currentSize = currentSize + 3;
                    Debug.Log("Placing Reft Turn", lastHallCube.gameObject);
                    if (rTurnPreLoads.Count == 0)
                    {
                        newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                        newTurn.gameObject.SetActive(true);
                    }
                    if (rTurnPreLoads.Count > 0)
                    {
                        newTurn = rTurnPreLoads[0];
                        newTurn.transform.position = position;
                        newTurn.transform.rotation = lastHallCube.transform.rotation;
                        newTurn.gameObject.SetActive(true);
                        rTurnPreLoads.Remove(newTurn);
                    }
                }
                if (availTurns[turnNum] == allTurns[2])
                {
                    currentSize = currentSize + 3;
                    Debug.Log("Placing T Turn", lastHallCube.gameObject);
                    if (tTurnPreLoads.Count == 0)
                    {
                        newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                        newTurn.gameObject.SetActive(true);
                    }
                    if (tTurnPreLoads.Count > 0)
                    {
                        newTurn = tTurnPreLoads[0];
                        newTurn.transform.position = position;
                        newTurn.transform.rotation = lastHallCube.transform.rotation;
                        newTurn.gameObject.SetActive(true);
                        tTurnPreLoads.Remove(newTurn);
                    }
                }
                if (availTurns[turnNum] == allRooms[0])
                {
                    currentSize = currentSize + 8;
                    Debug.Log("Placing Large Room", lastHallCube.gameObject);
                    if (lRoomPreLoads.Count == 0)
                    {
                        newTurn = Instantiate(allRooms[0], position, lastHallCube.transform.rotation);
                        newTurn.gameObject.SetActive(true);
                    }
                    if (lRoomPreLoads.Count > 0)
                    {
                        newTurn = lRoomPreLoads[0];
                        newTurn.transform.position = position;
                        newTurn.transform.rotation = lastHallCube.transform.rotation;
                        newTurn.gameObject.SetActive(true);
                        lRoomPreLoads.Remove(newTurn);
                    }
                }
                if (availTurns[turnNum] == allRooms[1])
                {
                    currentSize = currentSize + 5;
                    Debug.Log("Placing Small Room", lastHallCube.gameObject);
                    if (sRoomPreLoads.Count == 0)
                    {
                        newTurn = Instantiate(allRooms[1], position, lastHallCube.transform.rotation);
                        newTurn.gameObject.SetActive(true);
                    }
                    if (sRoomPreLoads.Count > 0)
                    {
                        newTurn = sRoomPreLoads[0];
                        newTurn.transform.position = position;
                        newTurn.transform.rotation = lastHallCube.transform.rotation;
                        newTurn.gameObject.SetActive(true);
                        sRoomPreLoads.Remove(newTurn);
                    }
                }

                if (newTurn == null)
                {
                    Debug.Log("newTurn == null", lastHallCube.gameObject);
                }
              
                TurnCube turnComp = newTurn.GetComponent<TurnCube>();
                turnComp.positioner.SetActive(true);
                if (turnComp.turnCollider != null)
                {
                    turnComp.turnCollider.enabled = false;
                }
                starter.endTurn = turnComp;
                turnComp.hallwayNum = starter.hallwayNumber;

                CubeRoom room = newTurn.GetComponent<CubeRoom>();
                if (room != null)
                {
                    room.turnCollider.enabled = false;
                    createdRooms.Add(room);
                    int x = Random.Range(0, room.starterCubes.Count);
                    room.starterCubes[x].gameObject.SetActive(false);
                    room.wallCovers[x].gameObject.SetActive(true);
                    room.openWall = room.wallCovers[x];
                    starter.endRoom = room;
                }
                if (room == null)
                {
                    createdTurns.Add(turnComp);
                }

                foreach (HallStarterCube newStarter in turnComp.starterCubes)
                {
                    if (newStarter.gameObject.activeSelf && !newStarter.hallBuildFin)
                    {
                        createdStarters.Add(newStarter);
                        newStarter.hallwayNumber = starter.hallwayNumber;
                    }
                }
            }
            if (availTurns.Count == 0)
            {
                starter.hallBuildFin = true;
                DeadEndCube newDeadEnd = lastHallCube.cap;
                starter.hallType = HallStarterCube.HallType.deadEnd;
                createdDeadEnds.Add(newDeadEnd);
                newDeadEnd.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(.25f);
            StartCoroutine(StarterCubeCheck());
        }
    }

    public IEnumerator EndNewHallway(HallStarterCube starter, Cube lastHallCube)
    {
        Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;
        Cube newTurn = null;
        bool blocked = false;
        bool largeSpace = true;


        allRooms[0].gameObject.SetActive(true);
        allRooms[0].transform.position = position;
        allRooms[0].transform.rotation = lastHallCube.transform.rotation;

        CubeRoom roomCheck = allRooms[0].GetComponent<CubeRoom>();
        roomCheck.turnCollider.enabled = true;

        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        Collider[] colliders = Physics.OverlapBox(roomCheck.turnCollider.bounds.center, roomCheck.turnCollider.bounds.extents);
        if (colliders.Length > 1)
        {    
            largeSpace = false;
            Debug.Log("Large Room BoxChecker Collision" + colliders[0].gameObject.name, colliders[0].gameObject);
        }
        allRooms[0].gameObject.SetActive(false);
        if (!largeSpace)
        {
            allRooms[1].gameObject.SetActive(true);
            allRooms[1].transform.position = position;
            allRooms[1].transform.rotation = lastHallCube.transform.rotation;

            CubeRoom sRoomCheck = allRooms[1].GetComponent<CubeRoom>();
            sRoomCheck.turnCollider.enabled = true;

            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Collider[] sColliders = Physics.OverlapBox(sRoomCheck.turnCollider.bounds.center, sRoomCheck.turnCollider.bounds.extents);
            if (sColliders.Length > 1)
            {
                blocked = true;
                Debug.Log(" Small Room BoxChecker Collision" + sColliders[0].gameObject.name, sColliders[0].gameObject);
            }
            allRooms[1].gameObject.SetActive(false);
        }
        if (!blocked)
        {
            currentSize++;
            List<Cube> availTurns = new List<Cube>();
            availTurns.Add(allTurns[0]);
            availTurns.Add(allTurns[1]);
            availTurns.Add(allTurns[2]);
            availTurns.Add(allRooms[1]);

            if (largeSpace)
            {
                availTurns.Add(allRooms[0]);
            }

            int turnNum = Random.Range(0, availTurns.Count);

            if (availTurns[turnNum] == allTurns[0])
            {
                Debug.Log("Placing Left Turn", lastHallCube.gameObject);
                if (lTurnPreLoads.Count == 0)
                {
                    newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                    newTurn.gameObject.SetActive(true);
                }
                if (lTurnPreLoads.Count > 0)
                {
                    newTurn = lTurnPreLoads[0];
                    newTurn.transform.position = position;
                    newTurn.transform.rotation = lastHallCube.transform.rotation;
                    newTurn.gameObject.SetActive(true);
                    lTurnPreLoads.Remove(newTurn);
                }
            }
            if (availTurns[turnNum] == allTurns[1])
            {
                Debug.Log("Placing Right Turn", lastHallCube.gameObject);
                if (rTurnPreLoads.Count == 0)
                {
                    newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                    newTurn.gameObject.SetActive(true);
                }
                if (rTurnPreLoads.Count > 0)
                {
                    newTurn = rTurnPreLoads[0];
                    newTurn.transform.position = position;
                    newTurn.transform.rotation = lastHallCube.transform.rotation;
                    newTurn.gameObject.SetActive(true);
                    rTurnPreLoads.Remove(newTurn);
                }
            }
            if (availTurns[turnNum] == allTurns[2])
            {
                Debug.Log("Placing T Turn", lastHallCube.gameObject);
                if (tTurnPreLoads.Count == 0)
                {
                    newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                    newTurn.gameObject.SetActive(true);
                }
                if (tTurnPreLoads.Count > 0)
                {
                    newTurn = tTurnPreLoads[0];
                    newTurn.transform.position = position;
                    newTurn.transform.rotation = lastHallCube.transform.rotation;
                    newTurn.gameObject.SetActive(true);
                    tTurnPreLoads.Remove(newTurn);
                }
            }
            if (availTurns[turnNum] == allRooms[0])
            {
                Debug.Log("Placing Large Room", lastHallCube.gameObject);
                if (lRoomPreLoads.Count == 0)
                {
                    newTurn = Instantiate(allRooms[0], position, lastHallCube.transform.rotation);
                    newTurn.gameObject.SetActive(true);
                }
                if (lRoomPreLoads.Count > 0)
                {
                    newTurn = lRoomPreLoads[0];
                    newTurn.transform.position = position;
                    newTurn.transform.rotation = lastHallCube.transform.rotation;
                    newTurn.gameObject.SetActive(true);
                    lRoomPreLoads.Remove(newTurn);
                }
            }
            if (availTurns[turnNum] == allRooms[1])
            {
                Debug.Log("Placing Small Room", lastHallCube.gameObject);
                if (sRoomPreLoads.Count == 0)
                {
                    newTurn = Instantiate(allRooms[1], position, lastHallCube.transform.rotation);
                    newTurn.gameObject.SetActive(true);
                }
                if (sRoomPreLoads.Count > 0)
                {
                    newTurn = sRoomPreLoads[0];
                    newTurn.transform.position = position;
                    newTurn.transform.rotation = lastHallCube.transform.rotation;
                    newTurn.gameObject.SetActive(true);
                    sRoomPreLoads.Remove(newTurn);
                }
            }

            TurnCube turnComp = newTurn.GetComponent<TurnCube>();
            turnComp.positioner.SetActive(true);
            turnComp.turnCollider.enabled = false;           
            starter.endTurn = turnComp;
            turnComp.hallwayNum = starter.hallwayNumber;

            CubeRoom room = newTurn.GetComponent<CubeRoom>();
            if (room != null)
            {
                room.turnCollider.enabled = false;
                createdRooms.Add(room);

                int x = Random.Range(0, room.starterCubes.Count);
                room.starterCubes[x].gameObject.SetActive(false);
                room.wallCovers[x].gameObject.SetActive(true);
                room.openWall = room.wallCovers[x];
                starter.endRoom = room;
            }
            if (room == null)
            {
                createdTurns.Add(turnComp);
            }

            foreach (HallStarterCube turnStarter in turnComp.starterCubes)
            {
                if (turnStarter.gameObject.activeSelf)
                {
                    turnStarter.hallwayNumber = turnComp.hallwayNum;
                    createdStarters.Add(turnStarter); 
                    if (turnStarter.hallwayNumber == 0)
                    {
                        hall0Starters.Add(turnStarter);
                    }
                    if (turnStarter.hallwayNumber == 1)
                    {
                        hall1Starters.Add(turnStarter);
                    }
                    if (turnStarter.hallwayNumber == 2)
                    {
                        hall2Starters.Add(turnStarter);
                    }
                }
            }

            StartCoroutine(DungeonStatusChecker());
        }

        if (blocked)
        {
            starter.hallBuildFin = true;
            lastHallCube.cap.gameObject.SetActive(true);

            DeadEndCube newDeadEnd = lastHallCube.cap;     
            starter.hallType = HallStarterCube.HallType.deadEnd;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);

            StartCoroutine(DungeonStatusChecker());
        }
        
    }

    IEnumerator DungeonStatusChecker()
    {
        Debug.Log("Checking Dungeon State");
        if (currentSize >= targetSize)
        {
            Debug.Log("Target Size Reached");
            HallStarterCube targetStarter = null;
            if (createdBossRooms.Count == 0) 
            {
                int index = 0;
                foreach (HallStarterCube startCube in createdStarters)
                {
                    if (!startCube.hallBuildFin)
                    {
                        targetStarter = startCube;
                        index = createdStarters.IndexOf(startCube);
                        Debug.Log("Starting Boss Hallway on starter " + index, startCube.gameObject);
                        break;
                    }
                }
                if (targetStarter != null)
                {
                   StartCoroutine(NewCreateBoss(targetStarter)); // calls Finalize Dungeon when finished
                }
                if (targetStarter == null)
                {
                    Debug.Log("No available starter cube for NewCreateBoss()");
                }
            }
        }
        if (currentSize < targetSize)
        {
            HallStarterCube nextStarter = null;
            hallBuildIndex++;
            if (hallBuildIndex >= 3)
            {
                hallBuildIndex = 0;
            }

            if (hallBuildIndex == 0)
            {
                foreach (HallStarterCube starter in hall0Starters)
                {
                    if (!starter.hallBuildFin)
                    {
                        nextStarter = starter;
                        break;
                    }
                }
            }
            if (hallBuildIndex == 1)
            {
                foreach (HallStarterCube starter in hall1Starters)
                {
                    if (!starter.hallBuildFin)
                    {
                        nextStarter = starter;
                        break;
                    }
                }
            }
            if (hallBuildIndex == 2)
            {
                foreach (HallStarterCube starter in hall2Starters)
                {
                    if (!starter.hallBuildFin)
                    {
                        nextStarter = starter;                    
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0);
            if (nextStarter!= null)
            {
                StartCoroutine(BuildNewHallway(nextStarter));
            }

            if (nextStarter == null)
            {
                Debug.Log("No Available Starter Cubes left in hallBuildIndex " + hallBuildIndex + ", Looping");
                loopCount++;
                yield return new WaitForFixedUpdate();
                StartCoroutine(DungeonStatusChecker());
            }
        }
    }

    IEnumerator NewCreateBoss(HallStarterCube starter)
    {
        Debug.Log("Starting Boss Loop");
        if (starter == null)
        {
            Debug.Log("No Starter attached to NewCreateBoss");
        }
        Cube firstCube = Instantiate(bossHallCube, starter.transform.position, starter.transform.rotation);
        float cubeLength = firstCube.lengthMesh.bounds.size.z;
        bool blocked = false;
        for (int i = 1; i < 10; i++)
        {
            Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
            blankColliderTestCube.transform.position = position;
            blankColliderTestCube.collisionChecker.enabled = true;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            Collider[] colliders = Physics.OverlapBox(blankColliderTestCube.collisionChecker.bounds.center, blankColliderTestCube.collisionChecker.bounds.extents);
            if (colliders.Length > 1)
            {
                blocked = true;
                Debug.Log("BoxChecker Collision" + colliders[0].gameObject.name + " " + position, colliders[0].gameObject);
            }
            blankColliderTestCube.collisionChecker.enabled = false;
            if (blocked)
            {
                break;
            }
        }
        if (!blocked)
        {
            Debug.Log("Boss Hall Clear, placing bosshallcubes");
            for (int i = 1; i < 10; i++)
            {
                Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
                Cube newBossCube = Instantiate(bossHallCube, position, Quaternion.identity);   
                
                newBossCube.transform.position = position;
                newBossCube.transform.rotation = firstCube.transform.rotation;
                newBossCube.gameObject.SetActive(true);
                starter.generatedHallway.Add(newBossCube);
            }
        }
        if (!blocked) // Boss Room checker
        {
            Debug.Log("Checking Space for Boss Room");
            Cube lastHallCube = starter.generatedHallway[starter.generatedHallway.Count - 1];
            Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;
            int b = Random.Range(0, allBossRooms.Count);

            Cube bossPrefab = Instantiate(allBossRooms[b], position, Quaternion.identity);
            bossPrefab.gameObject.SetActive(true);
            bossPrefab.transform.position = position;
            bossPrefab.transform.rotation = lastHallCube.transform.rotation;

            BossCube bossCheck = bossPrefab.GetComponent<BossCube>();
            bossCheck.collisionChecker.enabled = true;
            lastHallCube.gameObject.SetActive(false);

            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            Collider[] colliders = Physics.OverlapBox(bossCheck.collisionChecker.bounds.center, bossCheck.collisionChecker.bounds.extents);
            if (colliders.Length > 1)
            {
                blocked = true;
                Debug.Log("Boss Room BoxChecker Collision" + colliders[0].gameObject.name, colliders[0].gameObject);
            }
            lastHallCube.gameObject.SetActive(true);
            bossCheck.collisionChecker.enabled = false;

            if (blocked)
            {
                bossPrefab.gameObject.SetActive(false);
            }

            if (!blocked)
            {
                starter.hallBuildFin = true;
                starter.hallType = HallStarterCube.HallType.boss;
                SanctuaryCube sanct = sanctuary.GetComponent<SanctuaryCube>();
                sanct.bossHallway = starter.hallwayNumber;
                sanct.ProcessSigns();
                // link back to Santuary to confirm which hall has boss room.
               
                createdBossRooms.Add(bossCheck);
                bossCheck.collisionChecker.enabled = false;
                bossCheck.bossHallStarter = starter;
                bossCheck.controller = sceneController;
                bossPrefab.positioner.SetActive(true);
                bossHallNum = starter.hallwayNumber;
                currentSize = currentSize + 20;
                StartCoroutine(StarterCubeCheck());
            }
          
        }

        if (blocked)
        {
            Debug.Log("Boss Hall Blocked, looping...");
            firstCube.gameObject.SetActive(false);
            starter.hallBuildFin = true;
            DeadEndCube newDeadEnd = starter.deadEnd;
            starter.hallType = HallStarterCube.HallType.deadEnd;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);

            if (starter.generatedHallway.Count > 0)
            {
                foreach (Cube targetCube in starter.generatedHallway)
                {
                    targetCube.gameObject.SetActive(false);
                }
            }

            starter.generatedHallway.Clear();
            HallStarterCube refreshedStarter = null;

            foreach (HallStarterCube startCube in createdStarters)
            {
                if (!startCube.hallBuildFin)
                {
                    refreshedStarter = startCube;
                    break;
                }
            }
            if (refreshedStarter == null)
            {
                Debug.Log("No available Starters with !hallBuildFin");
            }
            StartCoroutine(NewCreateBoss(refreshedStarter));
        }


    }


    public void CreateBossRoom(HallStarterCube starter)
    {
        int maxLength = 10;
        bool blocked = false;
        int hallLength = maxLength;
        loadingBar.text.text = "Adding Boss Room...";
        starter.hallType = HallStarterCube.HallType.boss;

        

        Cube firstCube = Instantiate(bossHallCube, starter.transform.position, starter.transform.rotation);

        float cubeLength = firstCube.lengthMesh.bounds.size.z;
        Vector3 bossPosition = Vector3.zero;
        Quaternion bossRot = new Quaternion(0, 0, 0, 0);

        for (int i = 1; i < hallLength; i++)
        {
            Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
            Cube newCube = Instantiate(bossHallCube, position, firstCube.transform.rotation);
            starter.generatedHallway.Add(newCube);
            createdHallCubes.Add(newCube);
            currentSize++;
        }

        if (starter.HallwayCheck())
        {
            blocked = true;

        }
        if (!blocked) // end checker
        {
            foreach (Cube boss in allBossRooms)
            {
                int x = starter.generatedHallway.Count;
                Cube lastHallCube = starter.generatedHallway[x - 1];
                Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;
                Cube bossBase = Instantiate(boss, position, lastHallCube.transform.rotation);

                lastHallCube.gameObject.SetActive(false);
                if (bossBase.BoxChecker())
                {
                    blocked = true;
                }
                lastHallCube.gameObject.SetActive(true);
                bossBase.collisionChecker.enabled = false;
                bossBase.gameObject.SetActive(false);

                bossPosition = lastHallCube.transform.position;
                bossRot = lastHallCube.transform.rotation;
            }
        }

        // finalize

        if (!blocked)
        {
            int x = Random.Range(0, allBossRooms.Count);
            firstCube.gameObject.SetActive(false);
            starter.hallBuildFin = true;
            Cube bossRoom = Instantiate(allBossRooms[x], bossPosition, bossRot);
            BossCube bossCube = bossRoom.GetComponent<BossCube>();
            createdBossRooms.Add(bossCube);
            bossRoom.collisionChecker.enabled = false;
            bossCube.bossHallStarter = starter;
            bossCube.controller = sceneController;
            bossCube.positioner.SetActive(true);
            bossHallNum = starter.hallwayNumber;
            StartCoroutine(FinalizeDungeon());
        }



        if (blocked)
        {
            Debug.Log("Boss Hall Blocked, looping...");
            firstCube.gameObject.SetActive(false);
            starter.hallBuildFin = true;
            DeadEndCube newDeadEnd = starter.deadEnd;
            starter.hallType = HallStarterCube.HallType.deadEnd;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);

            if (starter.generatedHallway.Count > 0)
            {
                foreach (Cube targetCube in starter.generatedHallway)
                {
                    currentSize--;
                    targetCube.gameObject.SetActive(false);
                    createdHallCubes.Remove(targetCube); 
                }
            }

            starter.generatedHallway.Clear();
            StartCoroutine(StarterCubeCheck());
        }
    }

    public void MassiveSecretCheck()
    {

        List<HallStarterCube> openCubes = new List<HallStarterCube>();
        foreach (Cube starter in createdStarters)
        {
            HallStarterCube hall = starter.GetComponent<HallStarterCube>();
            if (!hall.hallBuildFin)
            {
                if (!hall.MassiveSecretChecker())
                {
                    openCubes.Add(hall);
                }
                else
                {

                }
            }
        }
        if (openCubes.Count > 0)
        {

            HallStarterCube furthestNorth = null;
            HallStarterCube furthestSouth = null;
            HallStarterCube furthestEast = null;
            HallStarterCube furthestWest = null;

            float maxNorth = float.MinValue;
            float maxSouth = float.MaxValue;
            float maxEast = float.MinValue;
            float maxWest = float.MaxValue;

            Vector3 centerPosition = sanctuary.transform.position;


            List<HiddenEndCube> massiveSecrets = new List<HiddenEndCube>();
            List<HallStarterCube> selectedMassiveStarters = new List<HallStarterCube>();

            foreach (HiddenEndCube secretCube in allDeadEnds)
            {
                if (secretCube.secretSize == HiddenEndCube.SecretSize.massive)
                {
                    massiveSecrets.Add(secretCube);
                }
            }

            foreach (HallStarterCube obj in openCubes)
            {
                Vector3 objPosition = obj.transform.position;
                float relativeX = objPosition.x - centerPosition.x;
                float relativeZ = objPosition.z - centerPosition.z;
                float yRotation = obj.transform.eulerAngles.y;

                if (relativeZ > maxNorth && !obj.hallBuildFin && IsWithinRotation(yRotation, 0)) // North (+Z)
                {
                    maxNorth = relativeZ;
                    furthestNorth = obj;                    
                }
                if (relativeZ < maxSouth && !obj.hallBuildFin && IsWithinRotation(yRotation, 180)) // South (-Z)
                {
                    maxSouth = relativeZ;
                    furthestSouth = obj;
                }
                if (relativeX > maxEast && !obj.hallBuildFin && IsWithinRotation(yRotation, 90)) // East (+X)
                {
                    maxEast = relativeX;
                    furthestEast = obj;
                }
                if (relativeX < maxWest && !obj.hallBuildFin && IsWithinRotation(yRotation, 270)) // West (-X)
                {
                    maxWest = relativeX;
                    furthestWest = obj;
                }
            }

            Debug.Log($"North: {furthestNorth?.name} at {maxNorth}", furthestNorth);
            Debug.Log($"South: {furthestSouth?.name} at {maxSouth}", furthestSouth);
            Debug.Log($"East: {furthestEast?.name} at {maxEast}", furthestEast);
            Debug.Log($"West: {furthestWest?.name} at {maxWest}", furthestWest);

            selectedMassiveStarters.Add(furthestNorth);
            selectedMassiveStarters.Add(furthestSouth);
            selectedMassiveStarters.Add(furthestWest);
            selectedMassiveStarters.Add(furthestEast);

            int targetCount = 0;
            if (selectedMassiveStarters.Count <= massiveSecrets.Count)
            {
                targetCount = selectedMassiveStarters.Count;
            }
            if (selectedMassiveStarters.Count > massiveSecrets.Count)
            {
                targetCount = massiveSecrets.Count;
            }
            Debug.Log("Target Massive Secret Count = " + targetCount + "for " + massiveSecrets.Count + " available massive secrets");
            // randomize massive list 
            ShuffleHiddenList(massiveSecrets);
            for (int i = 0; i < targetCount; i++)
            {               
                Cube bigSecret = Instantiate(massiveSecrets[i], selectedMassiveStarters[i].transform.position, selectedMassiveStarters[i].transform.rotation);
                createdSecretEnds.Add(bigSecret);
                selectedMassiveStarters[i].hallBuildFin = true;
                HiddenEndCube bigHiddenScript = bigSecret.GetComponent<HiddenEndCube>();

                if (bigHiddenScript.fakeWall != null)
                {
                    bigHiddenScript.fakeWall.controller = sceneController;
                    sceneController.distance.fakeWalls.Add(bigHiddenScript.fakeWall);
                }

                bigHiddenScript.SecretSetUp();

            }
            /*
            List<HallStarterCube> targetCubes = new List<HallStarterCube>();

            for (int i = 0; i < massiveSecrets.Count; i++)
            {
                if (i < openCubes.Count)
                {
                    targetCubes.Clear();

                    foreach (HallStarterCube emptyStarter in createdStarters)
                    {
                        if (!emptyStarter.hallBuildFin)
                        {
                            if (!emptyStarter.MassiveSecretChecker())
                            {
                                targetCubes.Add(emptyStarter);
                            }
                        }
                    }
                    int x = Random.Range(0, targetCubes.Count);

                    Cube bigSecret = Instantiate(massiveSecrets[i], targetCubes[x].transform.position, targetCubes[x].transform.rotation);
                    createdSecretEnds.Add(bigSecret);
                    targetCubes[x].hallBuildFin = true;
                }
            }
            */
        }
    }

    void ShuffleHiddenList(List<HiddenEndCube> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // Swap elements
            HiddenEndCube temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    bool IsWithinRotation(float currentRotation, float targetRotation, float margin = 1f)
    {
        float delta = Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation));
        return delta <= margin;
    }

    public void FillDeadEnds()
    {
        MassiveSecretCheck();
        int openEnds = 0;
        List<HallStarterCube> openCubes = new List<HallStarterCube>();
        foreach (Cube starter in createdStarters)
        {
            HallStarterCube hall = starter.GetComponent<HallStarterCube>();
            if (!hall.hallBuildFin)
            {
                if (!hall.SecretEndChecker())
                {
                    openEnds++;
                    openCubes.Add(hall);
                }
                if (hall.SecretEndChecker())
                {

                }
            }
        }
        foreach (Cube starter in startersBlocked)
        {
            HallStarterCube hall = starter.GetComponent<HallStarterCube>();
            hall.deadEnd.gameObject.SetActive(false);

            if (!hall.SecretEndChecker())
            {
                openEnds++;
                openCubes.Add(hall);
                Debug.Log("Adding blocked starter to secret end list", hall.gameObject);
            }
            if (hall.SecretEndChecker())
            {
                hall.deadEnd.gameObject.SetActive(true);
            }
            
        }
        openEnds = openCubes.Count;

        float secretTarget = openEnds / 4;
        secretTarget = Mathf.CeilToInt(secretTarget);
        List<HallStarterCube> secretStarters = new List<HallStarterCube>();
        for (int i = 0; i < secretTarget; i++)
        {
            int x = Random.Range(0, openCubes.Count);
            {
                secretStarters.Add(openCubes[x]);
                openCubes.Remove(openCubes[x]);
            }
        }


        foreach (HallStarterCube secretCube in secretStarters)
        {
            HiddenEndCube hidden = null;
            List<HiddenEndCube> capCubes = new List<HiddenEndCube>();
            foreach (Cube cube in allDeadEnds)
            {
                hidden = cube.GetComponent<HiddenEndCube>();
                if (hidden == null)
                {
                    Debug.Log("Failed to grab hidden cube component", cube.gameObject);
                }
                if (hidden != null)
                {
                    if (hidden.secretSize == HiddenEndCube.SecretSize.end)
                    {
                        capCubes.Add(hidden);
                    }
                }
            }
            if (capCubes.Count == 0)
            {
                Debug.Log("Builder failed to build list of available cap cubes");
            }
            if (capCubes.Count > 0)
            {

                int x = Random.Range(0, capCubes.Count);

                Cube newSecret = Instantiate(capCubes[x], secretCube.transform.position, secretCube.transform.rotation);
                createdSecretEnds.Add(newSecret);
                secretCube.hallBuildFin = true;
                secretCube.secret = true;
            }

        }
        foreach (Cube starter in startersBlocked)
        {
            HallStarterCube hall = starter.GetComponent<HallStarterCube>();
            if (!hall.secret)
            {
                if (!hall.deadEnd.gameObject.activeSelf)
                {
                    hall.deadEnd.gameObject.SetActive(true);
                }
            }
        }
    }

    IEnumerator FinalizeDungeon()
    {
        loadingBar.text.text = "Confirming Build...";     
        Debug.Log("Starting Hallway Final Check with " + createdStarters.Count + " starters");
        bool hallIssue = false;
        foreach (HallStarterCube starter in createdStarters)
        {        
            int x = createdStarters.IndexOf(starter);
            if (starter.generatedHallway.Count > 0 && starter.hallType != HallStarterCube.HallType.boss && x > 2) // x > 2 to skip the first 3 starters attached to Sanctuary.
            {
                foreach (Cube genHallCube in starter.generatedHallway)
                {
                    genHallCube.positioner.SetActive(false);
                    genHallCube.collisionChecker.enabled = true;

                    if (genHallCube.BoxChecker())
                    {
                        Debug.Log("Build ERROR, Hallway Collision, Pausing", genHallCube.gameObject);
                        hallIssue = true;        
                    }
                    if (!genHallCube.BoxChecker())
                    {
                        genHallCube.positioner.SetActive(true);
                        genHallCube.collisionChecker.enabled = false;
                    }                  
                }
            }
            yield return new WaitForNextFrameUnit();
        }
        if (hallIssue)
        {
   
            /*
            Cube firstCube = null;
            Cube lastCube = null; // could stay Null

            // clashed positions deactivated
            foreach (Cube genHallCube in starter.generatedHallway)
            {
                createdHallCubes.Remove(genHallCube); // to prevent being used for secret hall pools
                int genX = starter.generatedHallway.IndexOf(genHallCube);
                if (!genHallCube.positioner.activeSelf)
                {
                    if (firstCube == null)
                    {
                        if (genX == 0)
                        {
                            if (!starter.generatedHallway[0].positioner.activeSelf)
                            {
                                firstCube = starter.generatedHallway[0];
                            }
                        }
                        if (genX > 0 && firstCube == null)
                        {
                            if (starter.generatedHallway[genX - 1].positioner.activeSelf)
                            {
                                firstCube = starter.generatedHallway[genX - 1];
                            }
                        }
                    } 
                    if (genX < starter.generatedHallway.Count)
                    {
                        if (lastCube == null && firstCube != null)
                        {
                            if (starter.generatedHallway.Count > genX + 1)
                            {
                                if (starter.generatedHallway[genX + 1].positioner.activeSelf)
                                {
                                    lastCube = starter.generatedHallway[genX + 1];
                                }
                            }                              
                        }
                    }                       
                }
            }

            if (firstCube != null && lastCube == null) // Dead End
            {
                firstCube.positioner.SetActive(true);
                firstCube.cap.gameObject.SetActive(true);
                firstCube.cap.connectorPortal.gameObject.SetActive(true);
                DunPortal portalA = firstCube.cap.connectorPortal;
                firstCube.cap.connectorPortal.gameObject.SetActive(false);

                if (firstCube == starter.generatedHallway[0])
                {
                    starter.deadEnd.gameObject.SetActive(true);
                    firstCube.positioner.SetActive(false);
                    if (starter.endTurn != null)
                    {
                        starter.deadEnd.connectorPortal.gameObject.SetActive(true);
                        portalA = starter.deadEnd.connectorPortal;
                    }                                     
                }
                if (starter.endTurn != null)
                {   
                    starter.endTurn.flippedCap.gameObject.SetActive(true);
                    starter.endTurn.flippedCap.connectorPortal.gameObject.SetActive(true);
                    DunPortal portalB = starter.endTurn.flippedCap.connectorPortal;
                    portalA.gameObject.SetActive(true);
                    portalA.ConnectPortals(portalB);
                    sceneController.distance.portals.Add(portalA);
                    portalA.swapOnJump = true;
                }
            }
            if (firstCube != null && lastCube != null) // Portal
            {
                DunPortal portalA = null;
                if (firstCube == starter.generatedHallway[0])
                {
                    starter.deadEnd.gameObject.SetActive(true);
                    starter.deadEnd.connectorPortal.gameObject.SetActive(true);
                    portalA = starter.deadEnd.connectorPortal;

                    lastCube.positioner.SetActive(true);
                    lastCube.flippedCap.gameObject.SetActive(true);
                    lastCube.flippedCap.connectorPortal.gameObject.SetActive(true);
                    DunPortal portalB = lastCube.flippedCap.connectorPortal;

                    portalA.ConnectPortals(portalB);
                    sceneController.distance.portals.Add(portalA);
                    portalA.swapOnJump = true;

                }
                if (firstCube != starter.generatedHallway[0])
                {
                    firstCube.positioner.SetActive(true);
                    firstCube.cap.gameObject.SetActive(true);
                    firstCube.cap.connectorPortal.gameObject.SetActive(true);
                    portalA = firstCube.cap.connectorPortal;

                    lastCube.positioner.SetActive(true);
                    lastCube.flippedCap.gameObject.SetActive(true);
                    lastCube.flippedCap.connectorPortal.gameObject.SetActive(true);
                    DunPortal portalB = lastCube.flippedCap.connectorPortal;

                    portalA.ConnectPortals(portalB);
                    sceneController.distance.portals.Add(portalA);
                    portalA.swapOnJump = true;
                }  

            } */
        }
        if (!hallIssue)
        {
            Debug.Log("No ERRORs found in Finalize Dungeon Issues, Continuing Build");
            buildChecked = true;
            StartCoroutine(StarterCubeCheck());
        }
    }

    IEnumerator StarterCubeCheck() // swaps to Controller when finished
    {
        if (currentSize > targetSize)
        {
            HallStarterCube targetStarter = null;

            if (createdBossRooms.Count >= 1)
            {
                if (createdSecretEnds.Count == 0)
                {
                    FillDeadEnds();
                }
    
                foreach (HallStarterCube startCube in createdStarters)
                {
                    if (!startCube.hallBuildFin)
                    {
                        targetStarter = startCube;
                        targetStarter.hallBuildFin = true;
                        int starterIndex = createdStarters.IndexOf(targetStarter);
                        DeadEndCube newDeadEnd = targetStarter.deadEnd;
                        newDeadEnd.gameObject.SetActive(true);
                        targetStarter.hallType = HallStarterCube.HallType.deadEnd;
                        createdDeadEnds.Add(newDeadEnd);
                    }
                }
                frameBuild = true;
                Debug.Log("Dungeon Finished. Switching to SceneController Script");
                loadingBar.text.text = "Build Finished...";
                sceneController.SceneStart();
            }
            if (createdBossRooms.Count == 0)
            {
                foreach (HallStarterCube startCube in createdStarters)
                {
                    if (!startCube.hallBuildFin)
                    {
                        targetStarter = startCube;
                        break;
                    }
                }
                if (targetStarter != null)
                {
                    CreateBossRoom(targetStarter); // calls Finalize Dungeon when finished
                }
                
            }
        }
        if (currentSize <= targetSize)
        {
            HallStarterCube targetStarter = null;  
            foreach (HallStarterCube startCube in createdStarters)
            {
                if (!startCube.hallBuildFin)
                {
                    targetStarter = startCube;
                    break;
                }
            }
            if (targetStarter == null)
            {               
                loopCount++;
                if (loopCount > 10)
                {
                    Debug.Log("ERROR: No Available Starter Cubes not set to finish (hallBuildFin bool)");
                }
                if (loopCount <= 10)
                {
                    yield return new WaitForSeconds(.1f);
                    StartCoroutine(StarterCubeCheck());                    
                }
            }  
            if (targetStarter != null)
            {
                loopCount = 0;
                BuildOldHallway(targetStarter);
                yield return new WaitForSeconds(.1f);
                StartCoroutine(StarterCubeCheck());
            }
           
        }
    }



    IEnumerator CheckHallways()
    {
        HallStarterCube activeStarter = null;
        int x = 0;
        foreach (HallStarterCube starter in createdStarters)
        {
            if (starter.generatedHallway.Count == 0)
            {
                starter.testColliders = true;
            }
            if (starter.generatedHallway.Count > 1)
            {
                if (!starter.testColliders)
                {
                    starter.testColliders = true;
                    activeStarter = starter;
                    x = createdStarters.IndexOf(starter);
                    activeStarter.HallwayCheck();
                    break;
                }
            }
            yield return new WaitForSeconds(.05f);
        }
        if (x != createdStarters.Count - 1)
        {
            yield return new WaitForSeconds(.05f);
            StartCoroutine(CheckHallways());
        }
    }

    IEnumerator ResetBuild()
    {
        loadingBar.text.text = "Recalculating Build...";
        loadingBar.skullSlider.value = 0;
     
        foreach (GameObject fog in sanctuary.fogWalls)
        {
            fog.SetActive(false);
        }

        yield return new WaitForSeconds(.5f);
        Debug.Log("Build ERROR, Reseting Cubes");
        foreach (HallStarterCube starter in createdStarters)
        {
            starter.hallBuildFin = false;
            starter.generatedHallway.Clear();
            starter.hallwayNumber = 0;
            starter.gameObject.SetActive(true);
            starter.deadEnd.gameObject.SetActive(false);
        }
        foreach (HallStarterCube starter in startersBlocked)
        {
            starter.hallBuildFin = false;
            starter.generatedHallway.Clear();
            starter.hallwayNumber = 0;
            starter.gameObject.SetActive(true);
            starter.deadEnd.gameObject.SetActive(false);
        }
        foreach (CubeRoom room in createdRooms)
        {
            room.positioner.SetActive(false);
            if (!room.turnCollider.gameObject.activeSelf)
            {
                room.turnCollider.gameObject.SetActive(true);
            }  
            if (room.cubeType == Cube.CubeType.larRoom)
            {
                lRoomPreLoads.Add(room);
            }
            if (room.cubeType == Cube.CubeType.smallRoom)
            {
                sRoomPreLoads.Add(room);
            }
            room.gameObject.SetActive(false);
            room.transform.position = Vector3.zero;
        }
        Debug.Log("Used Rooms Added back to preLoad List");
        yield return new WaitForFixedUpdate();
        createdRooms.Clear();

        foreach (TurnCube turn in createdTurns)
        {
            turn.positioner.SetActive(false);
            if (!turn.turnCollider.gameObject.activeSelf)
            {
                turn.turnCollider.gameObject.SetActive(true);
            }   
            if (turn.cubeType == Cube.CubeType.lTurn)
            {
                lTurnPreLoads.Add(turn);
            }
            if (turn.cubeType == Cube.CubeType.rTurn)
            {
                rTurnPreLoads.Add(turn);
            }
            if (turn.cubeType == Cube.CubeType.tTurn)
            {
                tTurnPreLoads.Add(turn);
            }
            turn.gameObject.SetActive(false);
            turn.transform.position = Vector3.zero;
        }
        Debug.Log("Used Turns Added back to preLoad List");
        yield return new WaitForFixedUpdate();
        createdTurns.Clear();

        foreach (Cube hallCube in createdHallCubes)
        {
            hallCube.positioner.SetActive(false);
            if (!hallCube.collisionChecker.gameObject.activeSelf)
            {
                hallCube.collisionChecker.gameObject.SetActive(true);
            }
            blankExtenderPreLoads.Add(hallCube);
            hallCube.gameObject.SetActive(false);
            hallCube.transform.position = Vector3.zero;
        }
        foreach (Cube hallCube in hallcubesBlocked)
        {
            hallCube.positioner.SetActive(false);
            if (!hallCube.collisionChecker.gameObject.activeSelf)
            {
                hallCube.collisionChecker.gameObject.SetActive(true);
            }
            blankExtenderPreLoads.Add(hallCube);
            hallCube.gameObject.SetActive(false);
            hallCube.transform.position = Vector3.zero;
        }
        Debug.Log("Used blank HallCubes Added back to preLoad List");
        yield return new WaitForFixedUpdate();
        createdHallCubes.Clear();

        Debug.Log("Clearing Starter Cubes");
        createdStarters.Clear();


        foreach (BossCube bossCube in createdBossRooms)
        {
            bossCube.gameObject.SetActive(false);
        }
        Debug.Log("Clearing Boss Room");
        yield return new WaitForFixedUpdate();
        createdBossRooms.Clear();

        Debug.Log("Clearing Lists");
        startersBlocked.Clear();
        createdDeadEnds.Clear();

        yield return new WaitForFixedUpdate();

        currentSize = 0;
        Debug.Log("Dungeon Reset, Restarting Build");

        Debug.Log("Starting Build with " + sanctStarters.Count + " Santuary Starting Cubes");
        foreach (HallStarterCube starter in sanctStarters)
        {
            createdStarters.Add(starter);
            starter.builder = this;
        }
        BuildOldHallway(sanctStarters[0]);
        BuildOldHallway(sanctStarters[1]);
        BuildOldHallway(sanctStarters[2]);
        yield return new WaitForSeconds(.25f);
        foreach (GameObject fog in sanctuary.fogWalls)
        {
            fog.SetActive(true);
        }
        loadingBar.text.text = "Building Dungeon...";
        //StartCoroutine(StarterCubeCheck());
    }

}