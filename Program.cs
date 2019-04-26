using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class CastleOnTheGrid {
    // START is for when direction doesn't matter, i.e. when we start
    // searching for a path
    enum Direction {NORTH, EAST, SOUTH, WEST, START};

    struct Coords
    {
        public int x, y, numMovesToGetHere;
        public Direction directionEntered;

        public List<Coords> pathToHere;

        public Coords(int p1, int p2, int numMovesToGetHere, Direction d)
        {
            x = p1;
            y = p2;
            this.numMovesToGetHere = numMovesToGetHere;
            this.directionEntered = d;
            this.pathToHere = new List<Coords>();
        }
    }

    static int minimumMoves(string[] grid, int startX, int startY, int goalX, int goalY)     {    
        // This is BFS except we only consider the length of a path we find to increase
        // if the path turns
        //HashSet<string> visited = new HashSet<string>();
        int[,] visited = new int[grid.Length, grid.Length];
        Coords start = new Coords(startX, startY, 0, Direction.START);
        Queue<Coords> bfsQueue = new Queue<Coords>();
        bfsQueue.Enqueue(start);
        Coords end = new Coords(startX, startY, 0, Direction.START);
        int pathToFinish = Int16.MaxValue;

        while (bfsQueue.Count != 0)
        {
            Coords curr = bfsQueue.Dequeue();
                   outputPath(end, grid, startX, startY, goalX, goalY);

            // if (visited[curr.x, curr.y] != 0)
            // {
            //     // Element has been added to the queue twice and was already processed
            //     continue;
            // }
            
            //visited[curr.x, curr.y] = 1;
            //Console.WriteLine("Vistied ({0},{1})", curr.x, curr.y);

            if(curr.x == goalX && curr.y == goalY)
            {
                if (curr.numMovesToGetHere < pathToFinish)
                {
                    pathToFinish = curr.numMovesToGetHere;
                    end = curr;
                }
                
                
                //return curr.numMovesToGetHere;
            }

            // enqueue all possible moves from this position
            tryDirection(Direction.NORTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.EAST, bfsQueue, curr, grid, visited);
            tryDirection(Direction.SOUTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.WEST, bfsQueue, curr, grid, visited);
        }
            
        
        if (pathToFinish == Int16.MaxValue)
        {
            throw new Exception("No solution found");
        }
            
       outputPath(end, grid, startX, startY, goalX, goalY);
        
        return pathToFinish;
    }

    static void outputPath(Coords curr, string[] grid, int startX, int startY, int goalX, int goalY)
    {
         Console.Clear();
                
        for (int i=0; i < grid.Length; i++)
        {
            for (int j=0; j < grid[i].Length; j++)
            {
                if (i == startX && j == startY || i == goalX && j == goalY)
                {        
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("!");
                    Console.ResetColor();
                }
                else if (onPath(curr, i, j))
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("z");
                    Console.ResetColor();
                }
                // else if (visited[i,j] != 0)
                // {
                //     Console.BackgroundColor = ConsoleColor.White;
                //     Console.ForegroundColor = ConsoleColor.Blue;
                //     Console.Write("z");
                //     Console.ResetColor();
                // }
                else
                {
                    Console.Write(grid[i][j]);
                }
            }
            Console.WriteLine();
        }
    }

    static bool onPath(Coords end, int i, int j)
    {
        foreach(Coords coord in end.pathToHere)
        {
            if (coord.x == i && coord.y == j)
            {
                return true;
            }
        }

        return false;
    }
    static bool tryDirection(Direction d, Queue<Coords> bfsQueue, Coords start, string[] grid, int[,] visited)
    {
        Coords dest = getDestination(start, d);          
        if(isValidMove(dest, grid, visited))
        {
            bfsQueue.Enqueue(dest);
            return true;
        }

        return false;
    }

    static bool isValidMove(Coords destination, string[] grid, int[,] visited)
    {
        
        // Check for out of bounds end
        if ((destination.x < 0 || destination.x > grid.Length - 1) ||
            (destination.y < 0 || destination.y > grid.Length - 1))
        {
            return false;
        }

        // Check for blocked end
        if (grid[destination.x][destination.y] == 'X')
        {
            return false;
        }

        if (onPath(destination, destination.x, destination.y))
        {
            return false;
        }

        return true;
    }

    // d is the direction in which we are moving
    static Coords getDestination(Coords start, Direction directionOfMovement)
    {
        // TODO need a way to determine if we should increment start.numMovesToGetHere
        // We need to increm if the path ever turns
        Coords destination;
        // Only consider this a new move if we have to make a turn
        int numMovesToGetHere = start.directionEntered == directionOfMovement ?
            start.numMovesToGetHere : start.numMovesToGetHere + 1;

        switch (directionOfMovement)
        {
            case Direction.NORTH:
                destination = new Coords(start.x-1, start.y, numMovesToGetHere, Direction.NORTH); 
                break;
            case Direction.EAST:
                destination = new Coords(start.x, start.y+1, numMovesToGetHere, Direction.EAST);
                break;
            case Direction.SOUTH:
                destination = new Coords(start.x+1, start.y, numMovesToGetHere, Direction.SOUTH);
                break;
            case Direction.WEST:
                destination = new Coords(start.x, start.y-1, numMovesToGetHere, Direction.WEST);
                break;
            default:
                throw new ArgumentException("d is neither NORTH, EAST, SOUTH, or WEST");
        }

        foreach (Coords coord in start.pathToHere)
        {
            destination.pathToHere.Add(coord);
        }
        destination.pathToHere.Add(start);
        return destination;
    }   

    static void Main(string[] args) {
        //TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        int n = Convert.ToInt32(Console.ReadLine());

        string[] grid = new string [n];

        for (int i = 0; i < n; i++) {
            string gridItem = Console.ReadLine();
            grid[i] = gridItem;
        }

        string[] startXStartY = Console.ReadLine().Split(' ');

        int startX = Convert.ToInt32(startXStartY[0]);

        int startY = Convert.ToInt32(startXStartY[1]);

        int goalX = Convert.ToInt32(startXStartY[2]);

        int goalY = Convert.ToInt32(startXStartY[3]);

        int result = minimumMoves(grid, startX, startY, goalX, goalY);

        // textWriter.WriteLine(result);

        // textWriter.Flush();
        // textWriter.Close();
    }
}
