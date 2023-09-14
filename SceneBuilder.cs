using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;


public class SceneBuilder : MonoBehaviour
{
    public Camera buildCam;
    public int targetSize;
    public int currentSize;
    public List<HallStarterCube> sanctStarters;
    public Cube blankCube;
    public Cube bossHallCube;
    public SideExtenderCube sideCubeExtender;
    public TrapHallCube trapHallCube;
    public List<Cube> allTurns;
    public List<Cube> allRooms;
    public List<Cube> allDeadEnds;
    public List<Cube> allBossRooms;
    public Cube sanctuary;
    public List<Cube> createdStarters;
    public List<Cube> createdHallCubes;
    public List<CubeRoom> createdRooms;
    public List<TurnCube> createdTurns;
    public List<Cube> createdDeadEnds;
    public List<Cube> createdSecretEnds;
    public List<Cube> createdHallSideCubes;
    public List<Cube> createdTrapHalls;
    public List<BossCube> createdBossRooms;

    public PlayerController playerController;
    public SceneController sceneController;

    public LoadingBarUI loadingBar;

    private void Start()
    {
        PreBuild();
    }

    public void PreBuild()
    {
        currentSize = 0;
        if (targetSize == 0) // checks for error, sets to small by default if 0
        {
            targetSize = 250;
        }
        loadingBar.skullSlider.value = 0;
    }

    public void StartBuild()
    {  
        foreach (HallStarterCube starter in sanctStarters)
        {
            createdStarters.Add(starter);
            starter.builder = this;
            BuildHallway(starter);
            
        }
        foreach (GameObject fog in sanctuary.fogWalls)
        {
            fog.SetActive(true);
        }
        loadingBar.text.text = "Building Dungeon...";

        // start character select

        StartCoroutine(StarterCubeCheck());
    }

    public void ReLoadScene()
    {
        SceneManager.LoadScene("Builder");
    }

    public void BuildHallway(HallStarterCube starter)
    {
        int maxLength = 10;
        bool blocked = false;
        int hallLength = Random.Range(3, maxLength);
        starter.hallType = HallStarterCube.HallType.large;

        Cube firstCube = Instantiate(blankCube, starter.transform.position, starter.transform.rotation);

        float cubeLength = firstCube.lengthMesh.bounds.size.z;

        for (int i = 1; i < hallLength; i++)
        {
            Vector3 position = firstCube.transform.position + firstCube.transform.forward * (cubeLength * i);
            Cube newCube = Instantiate(blankCube, position, firstCube.transform.rotation);
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
            foreach (Cube turn in allTurns)
            {
                int x = starter.generatedHallway.Count;
                Cube lastHallCube = starter.generatedHallway[x - 1];
                Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;
                Cube turnBase = Instantiate(turn, position, lastHallCube.transform.rotation);
                TurnCube turnCheck = turnBase.GetComponent<TurnCube>();
                turnCheck.turnCollider.gameObject.SetActive(true);
                lastHallCube.gameObject.SetActive(false);
                if (turnCheck.TurnChecker())
                {       
                    blocked = true;
                }
                lastHallCube.gameObject.SetActive(true);
                turnCheck.turnCollider.gameObject.SetActive(false);
                Destroy(turnCheck.gameObject);
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
            EndHallway(starter, starter.generatedHallway[starter.generatedHallway.Count - 1]);
        }

      

        if (blocked)
        { 
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
                    Destroy(targetCube.gameObject);
                }
            }

            starter.generatedHallway.Clear();
        }
    }

    public void EndHallway(HallStarterCube starter , Cube lastHallCube) // adds Room or Turn or Dead End depending on cubecount;
    {
        if (currentSize >= targetSize)
        {
            Debug.Log("Target Size Reached, Closing Hallway");
            loadingBar.text.text = "Closing Dungeon...";
            DeadEndCube newDeadEnd = lastHallCube.cap;
            createdDeadEnds.Add(newDeadEnd);
            newDeadEnd.gameObject.SetActive(true);
            starter.hallType = HallStarterCube.HallType.deadEnd;
        }
        if (currentSize < targetSize)
        {
            List<Cube> availTurns = new List<Cube>();
            Vector3 position = lastHallCube.transform.position + lastHallCube.transform.forward;

            int turnCount = createdTurns.Count;
            int roomCount = createdRooms.Count;

            foreach (Cube roomCube in allRooms)
            {
                Cube checkRoom = Instantiate(roomCube, position, lastHallCube.transform.rotation);
                CubeRoom roomCheck = checkRoom.GetComponent<CubeRoom>();
                roomCheck.roomCollider.gameObject.SetActive(true);
                if (!roomCheck.RoomChecker())
                {
                    availTurns.Add(roomCube);
                }
                if (roomCheck.RoomChecker())
                {

                }
                roomCheck.roomCollider.gameObject.SetActive(false);
                Destroy(checkRoom.gameObject);
            }

            if (availTurns.Count > 0)
            {
                if (turnCount < roomCount)
                {
                    foreach (Cube turn in allTurns)
                    {
                        Cube turnBase = Instantiate(turn, position, lastHallCube.transform.rotation);
                        TurnCube turnCheck = turnBase.GetComponent<TurnCube>();
                        turnCheck.turnCollider.gameObject.SetActive(true);
                        if (!turnCheck.TurnChecker())
                        {
                            availTurns.Add(turn);
                        }
                        turnCheck.turnCollider.gameObject.SetActive(false);
                        Destroy(turnCheck.gameObject);

                        availTurns.Add(turn);
                    }
                }
            }

            if (availTurns.Count > 0)
            {
                int turnNum = Random.Range(0, availTurns.Count);

                Cube newTurn = Instantiate(availTurns[turnNum], position, lastHallCube.transform.rotation);
                currentSize++;
                TurnCube turnComp = newTurn.GetComponent<TurnCube>();
                turnComp.positioner.SetActive(true);
                if (turnComp.turnCollider != null)
                {
                    turnComp.turnCollider.gameObject.SetActive(false);
                }


                CubeRoom room = newTurn.GetComponent<CubeRoom>();
                if (room != null)
                {        
                    room.roomCollider.gameObject.SetActive(false);
                    createdRooms.Add(room);
                    int x = Random.Range(0, room.starterCubes.Count);
                    room.starterCubes[x].gameObject.SetActive(false);
                    room.wallCovers[x].gameObject.SetActive(true);
                    room.openWall = room.wallCovers[x];
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
        }
    }

    public void CreateBossRoom(HallStarterCube starter)
    {
        int maxLength = 10;
        bool blocked = false;
        int hallLength = maxLength;
        Debug.Log("Building Boss Hallway on starter Cube " + createdStarters.IndexOf(starter));
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
                bossBase.collisionChecker.gameObject.SetActive(false);
                Destroy(bossBase.gameObject);

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
            Cube bossRoom = Instantiate(allBossRooms[x], bossPosition,bossRot);
            BossCube bossCube = bossRoom.GetComponent<BossCube>();
            createdBossRooms.Add(bossCube);
            bossCube.positioner.SetActive(true);
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
                    Destroy(targetCube.gameObject);
                }
            }

            starter.generatedHallway.Clear();
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
            Debug.Log("Open Starters for Massive Secrets " + openCubes.Count);
            List<HiddenEndCube> massiveSecrets = new List<HiddenEndCube>();

            foreach (HiddenEndCube secretCube in allDeadEnds)
            {
                if (secretCube.secretSize == HiddenEndCube.SecretSize.massive)
                {
                    massiveSecrets.Add(secretCube);
                }
            }

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
        }
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
                    Debug.Log("Secret Box Checker positive on starter " + createdStarters.IndexOf(hall));
                }
            }
        }    
        openEnds = openCubes.Count;
        Debug.Log("Open Starters Left - " + openEnds);
        float secretTarget = openEnds / 5;
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
        Debug.Log("Filling " + secretStarters.Count + " ends with Secrets");

        foreach (HallStarterCube secretCube in secretStarters)
        {
            Cube newSecret = Instantiate(allDeadEnds[0], secretCube.transform.position, secretCube.transform.rotation);
            createdSecretEnds.Add(newSecret);
            secretCube.hallBuildFin = true;
        }
   
    }

    IEnumerator StarterCubeCheck() // swaps to Controller when finished
    {
        if (currentSize > targetSize)
        {
            HallStarterCube targetStarter = null;

            if (createdBossRooms.Count > 0)
            {
                if (createdSecretEnds.Count == 0)
                {
                    FillDeadEnds();                    
                }
                yield return new WaitForSeconds(.25f);
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
                CreateBossRoom(targetStarter);
                StartCoroutine(StarterCubeCheck());
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
            BuildHallway(targetStarter);
            yield return new WaitForSeconds(.05f);
            StartCoroutine(StarterCubeCheck());
        }
    }    

    public void StartHallwayChecker()
    {
        StartCoroutine(CheckHallways());
    }

    IEnumerator CheckHallways()
    {
        HallStarterCube activeStarter = null;
        int x = 0;

        foreach(HallStarterCube starter in createdStarters)
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
        }
        if (x != createdStarters.Count - 1)
        {
            yield return new WaitForSeconds(.1f);
            StartCoroutine(CheckHallways());
        } 
    }

}
