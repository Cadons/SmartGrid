using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// This class implements the A* algorithm to find the shortest path between two nodes
/// </summary>
/// 


namespace SmartGrid.AI
{
    public class PathFinder : IPathFinder
    {
        /// This is the pseudocode of the algorithm:
        /*function A*(start, end)
            // Create lists for open and closed nodes
            openList = { }
        closedList = { }

        // Add start node to openList
        add start to openList

            while openList is not empty
                // Find the node with the minimum F Value in the openList
                current = node in openList with the lowest F Value

                // If current node is the end node, then we have found the path
                if current is equal to end
                    return rebuild_path(current)

                // Otherwise, move the current node from the openList to the closedList
                remove current from openList
                add current to closedList

                // Check all adjacent nodes
                foreach node in neighbors(current)
                    if node in closedList
                        continue to next node

                    // Calculate the G Value of the node
                    attempt_G = current.G + distance_between(current, node)

                    if node not in openList or attempt_G < node.G
                        // This is a better path. Update the node
                        node.previous = current
                        node.G = attempt_G
                        node.H = distance_euclidean(node, end)
                        node.F = node.G + node.H

                        if node not in openList
                            add node to openList

            // No path was found
            returns null

        rebuild_path(node) function
            // Rebuilds the path starting from the final node and going back to the starting node
            path = { }
            current = node
            while current.previous exists
                add current to _top of path
                current = current.previous
            return path

        Translated with www.DeepL.com/Translator (free version)
        */
        // This represents the _grid on which the A* algorithm will operate
        public SmartGridController Grid;

        // This is the main method which finds the path between the start node and the end node
        public List<IAStarGridCell> FindPath(IAStarGridCell start, IAStarGridCell end)
        {
            if (Grid == null)
            {
                Debug.LogError("Grid is not set");
                return null;
            }
            if (end.IsOccupied)
            {
                List<IAStarGridCell> endNeighbourhood = GetNeighbourhood(end);
                List<IAStarGridCell> visited = new List<IAStarGridCell>();
                IAStarGridCell lastCell = null;
                bool unoccupiedCellFound = false;

                while (end.IsOccupied)
                {
                    foreach (var cell in endNeighbourhood)
                    {
                        if (!cell.IsOccupied)
                        {
                            end = cell;
                            unoccupiedCellFound = true;
                            break;
                        }
                        else
                        {
                            if (!visited.Contains(cell))
                                lastCell = cell;
                            else
                                lastCell = null;
                        }
                        visited.Add(cell);
                    }

                    if (unoccupiedCellFound)
                    {
                        break;
                    }

                    if (lastCell == start || lastCell == null)
                    {
                        Debug.LogError("No path found");
                        return new List<IAStarGridCell>();
                    }
                    else
                    {
                        endNeighbourhood = GetNeighbourhood(lastCell);
                    }
                }
                endNeighbourhood.Clear();
            }

            // OpenCells list contains nodes that need to be explored.
            // ClosedCells list contains nodes that have already been explored.
            List<IAStarGridCell> openCells = new List<IAStarGridCell>();
            List<IAStarGridCell> closedCells = new List<IAStarGridCell>();
            openCells.Add(start);

            while (openCells.Count > 0)
            {
                // In the main part of the while loop, the algorithm takes the cell with 
                // the minimum F Value from openCells, removes it from openCells, and adds it to closedCells.
                IAStarGridCell currentCell = openCells.OrderBy(x => x.F).First();
                openCells.Remove(currentCell);
                closedCells.Add(currentCell);

                // If the current cell is the end cell, then the algorithm has found the path
                if (currentCell == end)
                {

                    return GetFinishedList(start, end);
                }


                // Otherwise, the algorithm examines the neighbors of the current cell.
                // If a neighbor is not occupied, is not in closedCells, and is not too far from the current cell,
                // then it is added to openCells.
                foreach (var cell in GetNeighbourhood(currentCell))
                {

                    if (cell.IsOccupied || closedCells.Contains(cell) ||
                     Mathf.Abs(currentCell.X - cell.X) > 1 || Mathf.Abs(currentCell.Y - cell.Y) > 1)
                    {
                        continue;
                    }

                    cell.G = GetManhattenDistance(start, cell);
                    cell.H = GetManhattenDistance(end, cell);
                    cell.Previous = currentCell;

                    if (!openCells.Contains(cell))
                    {
                        openCells.Add(cell);
                    }
                }
            }

            // Returns an empty list if no path is found
            Debug.LogWarning("No path found");
            return new List<IAStarGridCell>();
        }

        // This private method builds and returns the list of the path found by the algorithm
        private List<IAStarGridCell> GetFinishedList(IAStarGridCell start, IAStarGridCell end)
        {
            List<IAStarGridCell> finishedList = new List<IAStarGridCell>();

            IAStarGridCell currentTile = end;

            // Starts from the end node and goes back to the start node following 
            // the links to the previous node saved in the Previous field of each node
            while (currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.Previous;
            }

            finishedList.Reverse();

            return finishedList;
        }

        // This private method calculates and returns the Manhattan distance between two nodes
        // The Manhattan distance is the sum of the absolute differences of the x and y coordinates
        private float GetManhattenDistance(IAStarGridCell start, IAStarGridCell tile)
        {
            return Mathf.Abs(start.X - tile.X) + Mathf.Abs(start.Y - tile.Y);
        }

        // This private method returns a list of neighbor nodes to the given node.
        // It checks if there exist neighbors on _top, _bottom, _left, and _right, and if there are, 
        // they are added to the list
        private List<IAStarGridCell> GetNeighbourhood(IAStarGridCell cell)
        {
            List<IAStarGridCell> neighbourhood = new List<IAStarGridCell>();

            //_top
            if (cell.Y >= 0 && cell.Y + 1 < Grid.Grid.GetLength(1))
                neighbourhood.Add(Grid.Grid[cell.X, cell.Y + 1]);
            //down
            if (cell.Y - 1 >= 0 && cell.Y < Grid.Grid.GetLength(1))
                neighbourhood.Add(Grid.Grid[cell.X, cell.Y - 1]);
            //_right
            if ((cell.X + 1) < Grid.Grid.GetLength(0) && cell.X >= 0)
                neighbourhood.Add(Grid.Grid[cell.X + 1, cell.Y]);
            //_left
            if ((cell.X - 1) >= 0 && cell.X < Grid.Grid.GetLength(0))
                neighbourhood.Add(Grid.Grid[cell.X - 1, cell.Y]);

            return neighbourhood;
        }
    }
}