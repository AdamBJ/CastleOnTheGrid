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
    enum Direction {NORTH, EAST, SOUTH, WEST};

    struct Coords
    {
        public int x, y, numMovesToGetHere;

        public Coords(int p1, int p2, int numMovesToGetHere)
        {
            x = p1;
            y = p2;
            this.numMovesToGetHere = numMovesToGetHere;
        }
    }

    static int minimumMoves(string[] grid, int startX, int startY, int goalX, int goalY)     {
        // This is BFS except we aren't moving by one node at a time. Rather, a move
        // continues until we have to make a turn or we find the target. BFS uses
        // a queue
        HashSet<string> visited = new HashSet<string>();
        Coords start = new Coords(startX, startY, 0);
        Queue<Coords> bfsQueue = new Queue<Coords>();
        bfsQueue.Enqueue(start);

        while (bfsQueue.Count != 0)
        {
            Coords curr = bfsQueue.Dequeue();
            visited.Add(curr.x.ToString() + curr.y.ToString());
            if(curr.x == goalX && curr.y == goalY)
            {
                return curr.numMovesToGetHere;
            }

            // Attempt to enqueue all possible moves from this position
            tryDirection(Direction.NORTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.EAST, bfsQueue, curr, grid, visited);
            tryDirection(Direction.SOUTH, bfsQueue, curr, grid, visited);
            tryDirection(Direction.WEST, bfsQueue, curr, grid, visited);
        }

        throw new Exception("No solution found");
    }

    static bool tryDirection(Direction d, Queue<Coords> bfsQueue, Coords start, string[] grid, HashSet<string> visited)
    {
        if(isValidMove(getDestination(start, d), grid, visited))
        {
            Coords end = getDestination(start, d);          
            bfsQueue.Enqueue(end);
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

    static Coords getDestination(Coords start, Direction d)
    {
        Coords destination;

        switch (d)
        {
            case Direction.NORTH:
                destination = new Coords(start.x-1, start.y, start.numMovesToGetHere);
                break;
            case Direction.EAST:
                destination = new Coords(start.x, start.y+1, start.numMovesToGetHere);
                break;
            case Direction.SOUTH:
                destination = new Coords(start.x+1, start.y, start.numMovesToGetHere);
                break;
            case Direction.WEST:
                destination = new Coords(start.x, start.y-1, start.numMovesToGetHere);
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
