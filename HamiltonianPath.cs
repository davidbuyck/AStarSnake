using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamiltonianPath : MonoBehaviour
{
    List<Vector3> path = new List<Vector3>();
    Grid grid;

    public List<Vector3> Path { get => path; set => path = value; }

    // Start is called before the first frame update
    void Awake()
    {
        bool down = true;
        grid = FindObjectOfType<Grid>();
        for(int i = 0; i < Grid.WIDTH; i++)
        {
            if(i == 0 || i == Grid.WIDTH - 1)
            {
                if (down)
                {
                    for (int j = Grid.WIDTH - 1; j >= 0; j--)
                    {
                        Path.Add(new Vector3(i, 0, j));
                    }
                }
                else
                {
                    for (int j = 0; j < Grid.WIDTH; j++)
                    {
                        Path.Add(new Vector3(i, 0, j));
                    }
                }

                // end pieces
            }
            else
            {
                if (down)
                {
                    for (int j = Grid.WIDTH - 2; j >= 0; j--)
                    {
                        Path.Add(new Vector3(i, 0, j));
                    }
                }
                else
                {
                    for (int j = 0; j < Grid.WIDTH - 1; j++)
                    {
                        Path.Add(new Vector3(i, 0, j));
                    }
                }
                // center cuts
            }
            down = !down;
        }
        // last top strip
        for (int i = Grid.WIDTH - 2; i > 0; i--)
        {
            Path.Add(new Vector3(i, 0, Grid.WIDTH - 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
