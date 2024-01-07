using System;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    private static WallState GetOppositeWall(WallState wall) {

        switch (wall) {

            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;

            default: return WallState.RIGHT;
        }
    }

    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height) {

        System.Random range = new System.Random();
        Stack<Vector2Int> positionStack = new Stack<Vector2Int>();

        Vector2Int position = new Vector2Int
        {
            x = range.Next(0, width),
            y = range.Next(0, height)
        };

        maze[position.x, position.y] |= WallState.VISITED;
        positionStack.Push(position);

        while (positionStack.Count > 0) {

            Vector2Int current = positionStack.Pop();
            List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0) {

                positionStack.Push(current);

                int randomIndex = range.Next(0, neighbours.Count);
                Neighbour randomNeighbour = neighbours[randomIndex];

                Vector2Int nPos = randomNeighbour.position;
                maze[current.x, current.y] &= ~randomNeighbour.sharedWall;

                maze[nPos.x, nPos.y] &= ~GetOppositeWall(randomNeighbour.sharedWall);
                maze[nPos.x, nPos.y] |= WallState.VISITED;

                positionStack.Push(nPos);
            }
        }

        CreateExit(maze, width, height);

        return maze;
    }

    private static void CreateExit(WallState[,] maze, int width, int height) {

        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                maze[UnityEngine.Random.Range(0, width), 0] ^= WallState.DOWN;
                break;

            case 1:
                maze[UnityEngine.Random.Range(0, width), height - 1] ^= WallState.UP;
                break;

            case 2:
                maze[0, UnityEngine.Random.Range(0, height)] ^= WallState.LEFT;
                break;

            case 3:
                maze[width - 1, UnityEngine.Random.Range(0, height)] ^= WallState.RIGHT;
                break;

            default: break;
        }

    }


    private static List<Neighbour> GetUnvisitedNeighbours(Vector2Int position, WallState[,] maze, int width, int height) {

        List<Neighbour> neighbours = new List<Neighbour>();

        if (position.x < width - 1)
        {

            if (!maze[(int)position.x + 1, (int)position.y].HasFlag(WallState.VISITED))
            {

                neighbours.Add(new Neighbour
                {
                    position = new Vector2Int(position.x + 1, position.y),
                    sharedWall = WallState.RIGHT
                });


            }

        }

        if (position.y > 0)
        {

            if (!maze[(int)position.x, (int)position.y - 1].HasFlag(WallState.VISITED))
            {

                neighbours.Add(new Neighbour
                {
                    position = new Vector2Int(position.x, position.y - 1),
                    sharedWall = WallState.DOWN
                });


            }

        }

        if (position.x > 0) {

            if (!maze[(int)position.x - 1, (int)position.y].HasFlag(WallState.VISITED)) {

                neighbours.Add(new Neighbour { 
                    position = new Vector2Int(position.x - 1, position.y), 
                    sharedWall = WallState.LEFT });
            
            }
        
        }

        


        if (position.y < height - 1)
        {

            if (!maze[(int)position.x, (int)position.y + 1].HasFlag(WallState.VISITED))
            {

                neighbours.Add(new Neighbour
                {
                    position = new Vector2Int(position.x, position.y + 1),
                    sharedWall = WallState.UP
                });


            }

        }


        



        return neighbours;
    }


    public static WallState[,] Generate(int width, int height) {

        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        for (int i = 0; i < width; i++) {

            for (int j = 0; j < height; j++) {

                maze[i, j] = initial;            
            }        
        }

        return ApplyRecursiveBacktracker(maze, width, height);
    }

   
}

public struct Neighbour {

    public Vector2Int position;
    public WallState sharedWall;

}



[Flags]
public enum WallState
{

    // 0000 - NO WALLS
    // 1111 - LEFT, RIGHT, UP, DOWN

    RIGHT = 2, // 0010
    LEFT = 1, // 0001    

    DOWN = 8, // 1000  
    UP = 4,  // 0100
      
    VISITED = 128
}