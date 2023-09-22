using SmartGrid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SmartGrid.BSP
{


public class Room : IBSPNode, IRoom
{
    #region Static
    private static int _cnt = 0;
    public static void ResetIdCounter()
    {
        _cnt = 0;
    }
    #endregion
    #region Variables
    private int _id = 0;
    protected SmartCell _vertexLL;//Vertex Lower Left
    protected SmartCell _vertexUL;//Vertex Upper Left
    protected SmartCell _vertexLR;//Vertex Lower Right
    protected SmartCell _vertexUR;//Vertex Upper Right
    protected List<List<SmartCell>> _walls = new List<List<SmartCell>>();
    private SmartCell _center;
    protected SmartGridController _grid;
    private bool _hasADoor = false;
    private GameObject _wallPrefab;
    #endregion
    #region Properties
    public int Id
    {
        get
        {
            return _id;
        }
    }
    public int Left { get; private set; }
    public int Right { get; private set; }
    public int Top { get; private set; }
    public int Bottom { get; private set; }

    public SmartCell Center
    {
        get
        {
            return _center;
        }
    }
    public int Width
    {
        get;
        private set;
    }
    public int Height
    {
        get;
        private set;
    }
    public int Area
    {
        get
        {
            return (Width - 1) * (Height - 1);
        }

    }

    public IBSPNode LeftChild { get; set; }
    public IBSPNode RightChild { get; set; }
    public IBSPNode Parent { get; set; }
    public bool IsLeaf
    {
        get
        {
            return LeftChild == null && RightChild == null;
        }
    }
    public int Value
    {
        get
        {
            return Id;
        }
    }
    #endregion

    #region Constructor
    public Room(int left, int right, int bottom, int top, SmartGridController grid)
    {
        _id = ++_cnt;
        //if top is smaller than bottom, swap them
        if (top < bottom)
        {
            int temp = top;
            top = bottom;
            bottom = temp;

        }
        //if left is bigger than right, swap them
        if (left > right)
        {
            int temp = left;
            left = right;
            right = temp;
        }
        //Check if the room is valid
        if (left < 0 || right > grid.Columns || top > grid.Rows || bottom < 0)
        {
            Debug.Log("Invalid room");
            Debug.Log("Left: " + left + " Right: " + right + " Top: " + top + " Bottom: " + bottom);
            throw new InvalidRoomException();
        }
        _grid = grid;
        _vertexLL = _grid.Grid[left, bottom];
        _vertexUL = _grid.Grid[left, top];
        _vertexLR = _grid.Grid[right, bottom];
        _vertexUR = _grid.Grid[right, top];

        _walls.Add(new List<SmartCell>());//UP
        _walls.Add(new List<SmartCell>());//DOWN
        _walls.Add(new List<SmartCell>());//LEFT
        _walls.Add(new List<SmartCell>());//RIGHT


        LoadHorizzontally(left, right, top, 0);

        LoadHorizzontally(left, right, bottom, 1);
        LoadVertically(bottom, top, left, 2);

        LoadVertically(bottom, top, right, 3);

        Height = Mathf.Abs((top - bottom));
        Width = Mathf.Abs((right - left));

        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
        //Get center
        int x = (left + right) / 2;
        int y = (top + bottom) / 2;
        _center = _grid.Grid[x, y];

    }
    #endregion

    #region IRoom
    public GameObject Build(HashSet<SmartCell> wallsToExclude, GameObject wallObject, GameObject door = null)
    {
        _wallPrefab = wallObject;
        GameObject roomContainer = new GameObject("Room " + Id);

        GameObject center = new GameObject("Center");
        center.transform.position = _center.LocalPosition;
        center.transform.parent = roomContainer.transform;
        if (wallsToExclude == null)
            wallsToExclude = new HashSet<SmartCell>();
        if (wallObject == null)
        {
            throw new NoElementToBuildRoomException();
        }
        //create the walls horizzontally
        //open wall to the near room removing 2 cells
        for (int i = 0; i < _walls.Count; i++)
        {

            BuildWall(i, roomContainer, wallsToExclude, door);
        }

        return roomContainer;
    }
    public bool IsInside(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (x >= Left && x <= Right && y >= Bottom && y <= Top)
            return true;
        return false;
    }
    #endregion

    #region Private Methods
    private void LoadHorizzontally(int from, int to, int column, int index)
    {
        for (int i = from; i <= to; i++)
        {
            if (!_walls[index].Contains(_grid.Grid[i, column]))
                _walls[index].Add(_grid.Grid[i, column]);
        }

    }
    private void LoadVertically(int from, int to, int row, int index)
    {
        for (int i = from; i <= to; i++)
        {
            if (!_walls[index].Contains(_grid.Grid[row, i]))
                _walls[index].Add(_grid.Grid[row, i]);

        }

    }
    private void BuildWall(int index, GameObject roomContainer, HashSet<SmartCell> wallsToExclude, GameObject door = null)
    {
        for (int i = 0; i < _walls[index].Count; i++)
        {
            if (!wallsToExclude.Contains(_walls[index][i]))
            {
                if (!_grid.IsOccupied(_walls[index][i].X, _walls[index][i].Y) && _walls[index][i].OccupiedBy == null)
                {

                    GameObject tile = CreateObject(roomContainer.transform);
                    tile.transform.position = _walls[index][i].LocalPosition;
                    _grid.SetCellOccupation(_walls[index][i].X, _walls[index][i].Y, tile);
                }
                else
                {
                    Debug.Log("Wall Already Exists!");
                }

            }
            else
            {
                if (door != null)
                {
                    if (UnityEngine.Random.Range(0, 2) == 1 && _hasADoor == false)
                    {
                        if (!_grid.IsOccupied(_walls[index][i].X, _walls[index][i].Y) && _walls[index][i].OccupiedBy == null)
                        {
                            if (door != null)
                            {
                                var corridorPath = wallsToExclude.ToList();
                                int corridorIndex = corridorPath.IndexOf(_walls[index][i]);
                                SmartCell previous = corridorPath[corridorIndex - 1];

                                GameObject tile = UnityEngine.Object.Instantiate(door);
                                tile.transform.SetParent(roomContainer.transform);

                                tile.transform.position = _walls[index][i].LocalPosition;
                                if (previous != null)
                                {
                                    if (previous.X < _walls[index][i].X)
                                    {
                                        tile.transform.rotation = Quaternion.Euler(0, -90f, 0);
                                    }
                                    else if (previous.X > _walls[index][i].X)
                                    {
                                        tile.transform.rotation = Quaternion.Euler(0, 90f, 0);
                                    }
                                    if (previous.Y < _walls[index][i].Y)
                                    {
                                        tile.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    }
                                    else if (previous.Y > _walls[index][i].Y)
                                    {
                                        tile.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                }
                                _hasADoor = true;
                                _grid.SetCellOccupation(_walls[index][i].X, _walls[index][i].Y, tile);
                            }

                        }
                    }


                }

            }

        }
    }
    private GameObject CreateObject(Transform roomContainer)
    {
        if (_wallPrefab != null)
        {
            GameObject tile = UnityEngine.Object.Instantiate(_wallPrefab);

            tile.transform.SetParent(roomContainer.transform);

            return tile;
        }
        return null;

    }
    #endregion
}
}