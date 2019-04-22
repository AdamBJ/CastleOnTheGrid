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

        public Coords(int p1, int p2, int numMovesToGetHere, Direction d)
        {
            x = p1;
            y = p2;
            this.numMovesToGetHere = numMovesToGetHere;
            this.directionEntered = d;
        }
    }

    static int minimumMoves(string[] grid, int startX, int startY, int goalX, int goalY)     {
        // This is BFS except we only consider the length of a path we find to increase
        // if the path turns
        HashSet<string> visited = new HashSet<string>();
        Coords start = new Coords(startX, startY, 0, Direction.START);
        Queue<Coords> bfsQueue = new Queue<Coords>();
        bfsQueue.Enqueue(start);

        while (bfsQueue.Count != 0)
        {
            Coords curr = bfsQueue.Dequeue();
            if (!visited.Add(curr.x.ToString() + curr.y.ToString()))
            {
                // Element has been added to the queue twice and was already processed
                continue;
            }
            Console.WriteLine("Vistied ({0},{1})", curr.x, curr.y);

            if(curr.x == goalX && curr.y == goalY)
            {
                return curr.numMovesToGetHere;
            }

            // enqueue all possible moves from this position
            tryDirection(Direction.NORTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.EAST, bfsQueue, curr, grid, visited);
            tryDirection(Direction.SOUTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.WEST, bfsQueue, curr, grid, visited);
        }

        throw new Exception("No solution found");
    }

    static bool tryDirection(Direction d, Queue<Coords> bfsQueue, Coords start, string[] grid, HashSet<string> visited)
    {
        Coords dest = getDestination(start, d);          
        if(isValidMove(dest, grid, visited))
        {
            bfsQueue.Enqueue(dest);
            return true;
        }

        return false;
    }

    static bool isValidMove(Coords destination, string[] grid, HashSet<string> visited)
    {
        if (visited.Contains(destination.x.ToString() + destination.y.ToString()))
        {
            return false;
        }
        
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
