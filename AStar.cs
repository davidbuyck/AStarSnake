using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://en.wikipedia.org/wiki/A*_search_algorithm ---------------------------------------------------------------------------

public class AStar : MonoBehaviour
{
    private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> totalPath = new List<Vector3>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        totalPath.RemoveAt(0);
        return totalPath;
    }

    private float Heuristic(Vector3 current, Vector3 goal)
    {
        float manhattanDistance = Vector3.Distance(current, goal);
        return manhattanDistance;
    }

    private List<Vector3> GetOpenNeighbors(Vector3 position, List<GameObject> tail)
    {
        List<Vector3> neighbors = new List<Vector3>();

        Vector3 up = new Vector3(0, 0, 1);
        Vector3 right = new Vector3(1, 0, 0);


        // check right
        bool isRightAvailabe = true;
        if (Grid.WIDTH > position.x + 1)
        {
            foreach (GameObject t in tail)
            {
                if (Vector3.Distance(position + right, t.transform.position) < 0.1)
                {
                    isRightAvailabe = false;
                }
            }
            if (isRightAvailabe)
            {
                neighbors.Add(position + right);
            }
        }

        // check left
        bool isLeftAvailabe = true;
        if (position.x > 0)
        {
            foreach (GameObject t in tail)
            {
                if (Vector3.Distance(position - right, t.transform.position) < 0.1)
                {
                    isLeftAvailabe = false;
                }
            }
            if (isLeftAvailabe)
            {
                neighbors.Add(position - right);
            }
        }

        // check left
        bool isUpAvailabe = true;
        if (Grid.HEIGHT > position.z + 1)
        {
            foreach (GameObject t in tail)
            {
                if (Vector3.Distance(position + up, t.transform.position) < 0.1)
                {
                    isUpAvailabe = false;
                }
            }
            if (isUpAvailabe)
            {
                neighbors.Add(position + up);
            }
        }

        // check left
        bool isDownAvailabe = true;
        if (position.z > 0)
        {
            foreach (GameObject t in tail)
            {
                if (Vector3.Distance(position - up, t.transform.position) < 0.1)
                {
                    isDownAvailabe = false;
                }
            }
            if (isDownAvailabe)
            {
                neighbors.Add(position - up);
            }
        }

        return neighbors;
    }

    private float EdgeWeight(Vector3 current, Vector3 neighbor)
    {
        // If I add weights to the edges I will do so here
        return 1;
    }

    /*private void RemoveUnreachableTail(List<GameObject> tail, GameObject snake)
    {
        List<GameObject> segmentsToRemove = new List<GameObject>();
        foreach (GameObject t in tail)
        {
            float distance = Vector3.Distance(snake.transform.position, t.transform.position);
            if(distance > tail.IndexOf(t) + 1)
            {
                segmentsToRemove.Add(t);
            }
        }

        foreach(GameObject t in segmentsToRemove)
        {
            tail.Remove(t);
        }
    }*/

    /* private (List<GameObject>, GameObject, List<GameObject>) SimulatedTail(List<Vector3> newPath, GameObject snake, List<GameObject> tail)
     {
         List<GameObject> simulatedTail = new List<GameObject>(tail);
         List<GameObject> gameObjectsToDelete = new List<GameObject>();
         GameObject simulatedSnake = new GameObject();
         simulatedSnake.transform.position = snake.transform.position;
         gameObjectsToDelete.Add(simulatedSnake);

         foreach (Vector3 v in newPath)
         {
             GameObject newGO = new GameObject();
             gameObjectsToDelete.Add(newGO);
             newGO.transform.position = simulatedSnake.transform.position;
             simulatedTail.Add(newGO);
             if (newPath.IndexOf(v) != newPath.Count - 1)
             {
                 simulatedTail.RemoveAt(0);
             }
             simulatedSnake.transform.position = v;
         }
         return (simulatedTail, simulatedSnake, gameObjectsToDelete);
     }*/

    private List<Vector3> CollectAllOpenMoves(GameObject snake, List<GameObject> originalTailCopy, Vector3 tailTip)
    {
        List<Vector3> openMoves = GetOpenNeighbors(snake.transform.position, originalTailCopy);
        openMoves.Sort((a, b) => Vector3.Distance(b, tailTip).CompareTo(Vector3.Distance(a, tailTip)));
        if (openMoves.Count > 0)
        {
            foreach (Vector3 v in openMoves)
            {
                List<Vector3> newPath = new List<Vector3>();
                newPath.Add(v);

                if (CheckIfTailIsReachableFromGoalPosition(newPath, originalTailCopy, snake))
                {
                    return newPath;
                }
            }
        }
        return new List<Vector3>();
    }

    private bool CheckIfTailIsReachableFromGoalPosition(List<Vector3> newPath, List<GameObject> tail, GameObject snake)
    {
        List<Vector3> simulatedPath = new List<Vector3>(newPath);
        List<GameObject> simulatedTail = new List<GameObject>();
        List<GameObject> gameObjectsToDelete = new List<GameObject>();
        List<Vector3> simPreviousPositions = new List<Vector3>();
        foreach (GameObject t in tail)
        {
            GameObject tailToDel = new GameObject();
            tailToDel.name = "Tail to del1";
            tailToDel.transform.position = t.transform.position;
            gameObjectsToDelete.Add(tailToDel);
            simulatedTail.Add(tailToDel);
            simPreviousPositions.Add(t.transform.position);
        }
        GameObject simulatedSnake = new GameObject();
        simulatedSnake.name = "Simulated snake";
        simulatedSnake.transform.position = snake.transform.position;
        gameObjectsToDelete.Add(simulatedSnake);
        AStarSnake s = new AStarSnake();
        s.SimulateSnake(simulatedSnake, simulatedPath[simulatedPath.Count - 1], simulatedTail, simPreviousPositions, simulatedPath, gameObjectsToDelete);
        Destroy(s);
        List<Vector3> pathToTailTip;
        bool isChasingTail;
        (pathToTailTip, isChasingTail) = AStarSearch(simulatedSnake.transform.position, simulatedTail[0].transform.position, simulatedTail, simulatedSnake, false, true);

        foreach (GameObject g in gameObjectsToDelete)
        {
            Destroy(g);
        }

        if (pathToTailTip.Count != 0)
        {
           // print("Passed");
            return true;
        }
        //print("Failed");
        return false;
    }

    private void RemoveUnreachableTail(Dictionary<Vector3, Vector3> cameFrom, Vector3 current, List<GameObject> tail)
    {
        List<Vector3> currentPath;
        currentPath = ReconstructPath(cameFrom, current);

        foreach (Vector3 v in currentPath)
        {
            if (tail.Count > 0)
            {
                tail.RemoveAt(0);
            }
        }
    }

    public (List<Vector3>, bool) AStarSearch(Vector3 start, Vector3 goal, List<GameObject> tail, GameObject snake, bool isChasingTail, bool isTailCheck)
    {
        /*GameObject snakeOriginal = new GameObject();
        snakeOriginal.name = "Snake original";
        snakeOriginal.transform.position = snake.transform.position;*/
        List<GameObject> tailOriginal = new List<GameObject>(tail);
        //RemoveUnreachableTail(tail, snake);

        List<Vector3> openSet = new List<Vector3>();
        openSet.Add(start);

        //List<Vector3> cameFrom = new List<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();

        // gScores not in list should be treated as infinity
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float>();
        gScore.Add(start, 0);

        //List<float> fScore = new List<float>();
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float>();
        fScore.Add(start, Heuristic(start, goal));

        while (openSet.Count > 0)
        {
            Vector3 current = openSet[0]; // This should be the vector3 with the lowest fScore in the list
            tail = new List<GameObject>(tailOriginal);
            RemoveUnreachableTail(cameFrom, current, tail);


            if (Vector3.Distance(current, goal) < 0.1f)
            {
                // This section is where the goal is reachable
                //-------------------------------------------------------------

                List<Vector3> newPath = ReconstructPath(cameFrom, current);

                bool isTailReachable = true;
                if (!isChasingTail && !isTailCheck && tail.Count > 0)
                {
                    isTailReachable = CheckIfTailIsReachableFromGoalPosition(newPath, tailOriginal, snake);
                }
                if (isTailReachable)
                {
                    if (!isTailCheck && snake.GetComponent<AStarSnake>().Tail.Count > Grid.WIDTH * Grid.HEIGHT * UniversalConstans.HOMESTRETCH_TAIL_LENGTH)
                    {
                        List<GameObject> originalTailCopy = new List<GameObject>(tailOriginal);
                        List<Vector3> deviation = CollectAllOpenMoves(snake, originalTailCopy, tailOriginal[0].transform.position);
                        if (deviation.Count > 0) // && Vector3.Distance(snake.transform.position, tailOriginal[tailOriginal.Count - 1].transform.position) < 1.1f)
                        {
                            return (deviation, true);
                        }
                    }

                    return (newPath, isChasingTail);
                }
                else
                {
                    // This is where the goal is reachable but the tail is not
                    List<GameObject> originalTailCopy = new List<GameObject>(tailOriginal);
                    List<Vector3> deviation = CollectAllOpenMoves(snake, originalTailCopy, tailOriginal[0].transform.position);
                    if (deviation.Count > 0)
                    {
                        return (deviation, true);
                    }
                    return AStarSearch(snake.transform.position, tailOriginal[0].transform.position, tailOriginal, snake, true, false);
                }
            }

            openSet.Remove(current);

            foreach (Vector3 neighbor in GetOpenNeighbors(current, tail))
            {
                float tenativeGScore = gScore[current] + EdgeWeight(current, neighbor);
                if (!gScore.ContainsKey(neighbor) || tenativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;

                    if (gScore.ContainsKey(neighbor))
                    {
                        gScore[neighbor] = tenativeGScore;
                    }
                    else
                    {
                        gScore.Add(neighbor, tenativeGScore);
                    }
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // This is when there is no route to the goal

        if (isTailCheck)
        {
            return (new List<Vector3>(), false);
        }
        else
        {
            List<GameObject> originalTailCopy = new List<GameObject>(tailOriginal);
            List<Vector3> deviation = CollectAllOpenMoves(snake, originalTailCopy, tailOriginal[0].transform.position); ;
            if (deviation.Count > 0) // && Vector3.Distance(snake.transform.position, tailOriginal[tailOriginal.Count - 1].transform.position) < 1.1f)
            {
                return (deviation, true);
            }
            //print("Tail not reachable2");

            return AStarSearch(snake.transform.position, tailOriginal[0].transform.position, tailOriginal, snake, true, false); // this empty list represents failure
        }
    }
}
