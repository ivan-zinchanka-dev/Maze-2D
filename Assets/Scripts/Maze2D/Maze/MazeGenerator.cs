using System.Collections.Generic;
using Maze2D.Configs;
using Maze2D.Game;
using UnityEngine;

namespace Maze2D.Maze
{
    public class MazeGenerator
    {
        /*private DifficultyConfigContainer _configContainer;

        public MazeGenerator(DifficultyConfigContainer configContainer)
        {
            _configContainer = configContainer;
        }*/
        

        public WallState[,] Generate(int width, int height) {

            WallState[,] maze = new WallState[width, height];
            WallState initial = WallState.Right | WallState.Left | WallState.Up | WallState.Down;

            for (int i = 0; i < width; i++) {

                for (int j = 0; j < height; j++) {

                    maze[i, j] = initial;            
                }        
            }

            return ApplyRecursiveBackTracker(maze, width, height);
        }
        
        private WallState[,] ApplyRecursiveBackTracker(WallState[,] maze, int width, int height) {
            
            Stack<Vector2Int> positionStack = new Stack<Vector2Int>();

            Vector2Int position = new Vector2Int
            {
                x = Random.Range(0, width),
                y = Random.Range(0, height)
            };

            maze[position.x, position.y] |= WallState.Visited;
            positionStack.Push(position);

            while (positionStack.Count > 0) {

                Vector2Int current = positionStack.Pop();
                List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

                if (neighbours.Count > 0) {

                    positionStack.Push(current);

                    int randomIndex = Random.Range(0, neighbours.Count);
                    Neighbour randomNeighbour = neighbours[randomIndex];

                    Vector2Int nPos = randomNeighbour.Position;
                    maze[current.x, current.y] &= ~randomNeighbour.SharedWall;

                    maze[nPos.x, nPos.y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                    maze[nPos.x, nPos.y] |= WallState.Visited;

                    positionStack.Push(nPos);
                }
            }

            CreateExit(maze, width, height);

            return maze;
        }
        
        private static List<Neighbour> GetUnvisitedNeighbours(Vector2Int position, WallState[,] maze, int width, int height) {

            List<Neighbour> neighbours = new List<Neighbour>();

            if (position.x < width - 1)
            {
                if (!maze[position.x + 1, position.y].HasFlag(WallState.Visited))
                {
                    neighbours.Add(new Neighbour
                    {
                        Position = new Vector2Int(position.x + 1, position.y),
                        SharedWall = WallState.Right
                    });
                }
            }

            if (position.y > 0)
            {
                if (!maze[position.x, position.y - 1].HasFlag(WallState.Visited))
                {
                    neighbours.Add(new Neighbour
                    {
                        Position = new Vector2Int(position.x, position.y - 1),
                        SharedWall = WallState.Down
                    });
                }
            }

            if (position.x > 0) {

                if (!maze[position.x - 1, position.y].HasFlag(WallState.Visited)) {

                    neighbours.Add(new Neighbour { 
                        Position = new Vector2Int(position.x - 1, position.y), 
                        SharedWall = WallState.Left });
                }
            }

            if (position.y < height - 1)
            {
                if (!maze[position.x, position.y + 1].HasFlag(WallState.Visited))
                {
                    neighbours.Add(new Neighbour
                    {
                        Position = new Vector2Int(position.x, position.y + 1),
                        SharedWall = WallState.Up
                    });
                }
            }
            
            return neighbours;
        }

        private WallState GetOppositeWall(WallState wall) {

            switch (wall) {

                case WallState.Right: return WallState.Left;
                case WallState.Left: return WallState.Right;
                case WallState.Up: return WallState.Down;
                case WallState.Down: return WallState.Up;

                default: return WallState.Right;
            }
        }
        
        private static void CreateExit(WallState[,] maze, int width, int height) {

            switch (Random.Range(0, 4))
            {
                case 0:
                    maze[Random.Range(0, width), 0] ^= WallState.Down;
                    break;

                case 1:
                    maze[Random.Range(0, width), height - 1] ^= WallState.Up;
                    break;

                case 2:
                    maze[0, Random.Range(0, height)] ^= WallState.Left;
                    break;

                case 3:
                    maze[width - 1, Random.Range(0, height)] ^= WallState.Right;
                    break;
            }
        }
    }
}