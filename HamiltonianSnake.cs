using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HamiltonianSnake : MonoBehaviour
{
    [SerializeField] GameObject snakeTail;
    Grid grid;
    List<Vector3> previousPositions = new List<Vector3>();

    List<GameObject> tail = new List<GameObject>();

    HamiltonianPath hamPath;

    static int wins = 0;
    static int losses = 0;
    static List<float> times = new List<float>();

    float time;
    float waitTime = .001f;

    List<Vector3> path;

    bool removeWorked = false;

    bool isGameOver = false;

    float startTime;

    public List<GameObject> Tail { get => tail; set => tail = value; }


    // Start is called before the first frame update
    void Awake()
    {
        hamPath = GetComponent<HamiltonianPath>();
        grid = FindObjectOfType<Grid>();
    }

    void Start()
    {
        List<GameObject> tailCopy = new List<GameObject>(tail);
        path = new List<Vector3>(hamPath.Path);
        time = Time.time;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //print("Path size: " + path.Count);
        if (!isGameOver)
        {
            if (Time.time > time + waitTime)
            {
                if (path.Count > 0)
                {
                    time = Time.time;
                    MoveSnake(gameObject, path, previousPositions, true);
                    if (tail.Count < Grid.WIDTH * Grid.WIDTH - 1)
                    {
                        IsGameLost();
                    }
                    TailAndGoalState(gameObject, grid.Goal.transform.position, path, tail, previousPositions, true, new List<GameObject>());
                }
                else
                {
                    List<GameObject> tailCopy = new List<GameObject>(tail);
                    path = new List<Vector3>(hamPath.Path);
                }
            }
        }

        if (tail.Count == 99)
        {
            print("You win!");
            isGameOver = true;
            times.Add((Time.time - startTime));
            wins++;
            File.AppendAllText("HamPathData.txt", " Time for competion: " + (Time.time - startTime) + ", with waitime: " + waitTime + ", Win. Win/loss: " + wins + "/" + losses + ", average time: " + times.Sum() / times.Count + ", Max time: " + times.Max() + ", Min time: " + times.Min() + "\n");

            SceneManager.LoadScene("SampleScene");
        }
    }

    public void SimulateSnake(GameObject simSnake, Vector3 goalPosition, List<GameObject> simTail, List<Vector3> simPrevPositions, List<Vector3> simPath, List<GameObject> addedTailsToDelete)
    {
        while (simPath.Count > 0)
        {
            MoveSnake(simSnake, simPath, simPrevPositions, false);
            TailAndGoalState(simSnake, goalPosition, simPath, simTail, simPrevPositions, false, addedTailsToDelete);
        }
    }

    private void IsGameLost()
    {
        foreach (GameObject t in tail)
        {
            if (t.transform.position == transform.position)
            {
                isGameOver = true;
                print("Game Over");
                times.Add((Time.time - startTime));
                losses++;
                File.AppendAllText("HamPathData.txt", " Time for competion: " + (Time.time - startTime) + ", with waitime: " + waitTime + ", Fail. Win/loss: " + wins + "/" + losses + ", average time: " + times.Sum()/times.Count + ", Max time: " + times.Max() + ", Min time: " + times.Min() + "\n");
                SceneManager.LoadScene("SampleScene");
            }
        }
    }


    private void TailAndGoalState(GameObject localSnake, Vector3 goalPosition, List<Vector3> localPath, List<GameObject> localTail, List<Vector3> localPreviousPositons, bool isRealSnake, List<GameObject> addedTailsToDelete)
    {
        if (localSnake.transform.position == goalPosition)
        {
            GameObject firstTail;
            if (isRealSnake)
            {
                firstTail = Instantiate(snakeTail, localPreviousPositons[0], Quaternion.identity);
            }
            else
            {
                firstTail = new GameObject();
                firstTail.name = "First tail";
                firstTail.transform.position = localPreviousPositons[0];
                addedTailsToDelete.Add(firstTail);
            }
            localTail.Add(firstTail);
            firstTail.transform.position = localPreviousPositons[0];
            if (isRealSnake)
            {
                //List<Vector3> pathReceiver;
                grid.PlaceFood();
                CheckIfFoodOverlapsTail();
                //List<GameObject> tailCopy = new List<GameObject>(localTail);

                //(pathReceiver, isChasingTail) = aStar.AStarSearch(transform.position, grid.Goal.transform.position, tailCopy, localSnake, false, false);
                //localPath.AddRange(pathReceiver);
            }
        }
        else
        {
            if (localTail.Count > 0)
            {
                if (isRealSnake)
                {
                    grid.AddSpace(localTail[0].transform.position);
                }
                localTail[0].transform.position = localPreviousPositons[0];
                GameObject recycledTail = localTail[0];
                localTail.RemoveAt(0);
                localTail.Add(recycledTail);
            }
        }
    }

    private void CheckIfFoodOverlapsTail()
    {
        foreach (GameObject t in tail)
        {
            if (t.transform.position == grid.Goal.transform.position)
            {
                print("Food overlapped tail!!!!!!!!!!!!!!!!");
                isGameOver = true;
            }
        }

        if (grid.Goal.transform.position == transform.position)
        {
            print("Food overlapped snake!!!!!!!!!!!!!!!!");
            isGameOver = true;
        }
    }

    private void MoveSnake(GameObject localSnake, List<Vector3> localPath, List<Vector3> localPreviousPositons, bool isRealSnake)
    {
        localPreviousPositons.Insert(0, localSnake.transform.position);
        if (localPreviousPositons.Count > Grid.WIDTH * Grid.HEIGHT)
        {
            localPreviousPositons.RemoveAt(localPreviousPositons.Count - 1);
        }


        localSnake.transform.position = localPath[0];
        if (isRealSnake && tail.Count == 0 && transform.position != grid.Goal.transform.position)
        {
            grid.AddSpace(localPreviousPositons[0]);
        }
        localPath.RemoveAt(0);
        if (isRealSnake)
        {
            removeWorked = grid.RemoveSpace(localSnake.transform.position);
            if (!removeWorked)
            {
                print("Remved failed");
            }
        }
    }
}
