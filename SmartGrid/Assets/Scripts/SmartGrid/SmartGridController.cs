using System.Collections.Generic;
using UnityEngine;
using SmartGrid.AI;
namespace SmartGrid
{

    public class SmartGridController : MonoBehaviour, IPathFinder
    {
        private SmartCell[,] _grid;
        [SerializeField]
        [Min(1)]
        private int _cols = 10;
        [SerializeField]
        [Min(1)]
        private int _rows = 10;
        [SerializeField]
        [Min(1)]
        private float _edge = 1;
        [SerializeField]
        private Material _gridMaterial;
        [SerializeField]
        private Shader _gridShader;
        [SerializeField]
        private bool _showGridGizmos = true;
        private bool _editable = false;
        private bool _editableLocked = false;
        private PathFinder _pathFinder;
        public float OffsetY = 0;
        private List<IAStarGridCell> _paths = new List<IAStarGridCell>();
        public int Columns { get { return _cols; } }
        public int Rows { get { return _rows; } }
        public float Edge { get { return _edge; } }
        public SmartCell[,] Grid
        {
            get
            {
                return _grid == null ? new SmartCell[_rows, _cols] : _grid;
            }
        }
        private void Start()
        {
            _editableLocked = true;
            _editable = false;
        }
        public void Awake()
        {

            Create();
        }
        public void Create(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            Create();
        }
        public void Create()
        {
            if (_pathFinder == null)
            {
                _pathFinder = new PathFinder();
                _pathFinder.Grid = this;
            }
            if (_cols <= 0 && _rows <= 0 && _edge <= 0)
                return;
            _grid = new SmartCell[_cols, _rows];

            // Set mesh centered and scaled
            for (int i = 0; i < _cols; i++)
            {
                for (int j = 0; j < _rows; j++)
                {
                    Vector3 cellPosition = ComputePosition(i, j);

                    var cell = new SmartCell(cellPosition, _edge, this, i, j);
                    cell.OffsetY = OffsetY;
                    cell.UpdateOccupationStatus();
                    _grid[i, j] = cell;
                }
            }
            ShowGrid();


        }
        private Vector3 ComputePosition(int i, int j)
        {
            float x = transform.position.x - (_cols - 1) * _edge / 2f;  // Calculate the _x position offset
            float y = transform.position.y;
            float z = transform.position.z + (_rows - 1) * _edge / 2f;  // Calculate the _z position offset
            return new Vector3(x + i * _edge, y, z - j * _edge);
        }
        public void SetIsEditable(bool isEditable)
        {
            if (_editableLocked)
            {
                Debug.LogWarning("Grid is locked!\nCells can't be changed manually");
                return;
            }
            _editable = isEditable;
        }
        public void SetCellOccupation(int x, int y, GameObject occupant)
        {
            if (_editable)
            {
                if (x >= 0 && x < _grid.GetLength(0) && y >= 0 && y < _grid.GetLength(1))
                {
                    _grid[x, y].ForceOccupation(this, occupant);
                }
            }
            else
            {
                Debug.LogWarning("Grid is not editable!\nCells can't be changed manually");
            }
        }
        public void ShowGrid()
        {
            if (gameObject.GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshRenderer>();
            if (gameObject.GetComponent<MeshFilter>() == null)
                gameObject.AddComponent<MeshFilter>();

            gameObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Plane.fbx");
            gameObject.GetComponent<MeshRenderer>().material = _gridMaterial;
            transform.localScale = new Vector3(_cols * _edge, 1, _rows * _edge);
            if (_gridMaterial != null)
            {
                if (_gridMaterial.shader.name == _gridShader.name)
                {
                    _gridMaterial.SetVector("_Tiling", new Vector4(_cols, _rows, 0, 0));
                }
            }

        }

        public bool IsOccupied(int x, int y)
        {
            if (x >= 0 && x < _grid.GetLength(0) && y >= 0 && y < _grid.GetLength(1))
            {
                _grid[x, y].UpdateOccupationStatus();
                return _grid[x, y].IsOccupied;
            }

            return true;
        }
        public void Update()
        {
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i, j] != null)
                        _grid[i, j].UpdateOccupationStatus();
                }
            }
        }

        public SmartCell GetNearest(Vector3 position)
        {
            SmartCell nearest = Grid[0, 0];
            float minDistance = float.MaxValue;
            foreach (var cell in Grid)
            {
                float distance = Vector3.Distance(cell.LocalPosition, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = cell;
                }
            }
            return nearest;
        }


        private void OnDrawGizmos()
        {
            if (_showGridGizmos)
            {
                if (_grid == null)
                    return;
                for (int i = 0; i < _grid.GetLength(0); i++)
                {
                    for (int j = 0; j < _grid.GetLength(1); j++)
                    {
                        if (_grid[i, j] != null)
                            _grid[i, j].DebugDraw();

                    }
                }


                foreach (var path in _paths)
                {

                    ((SmartCell)path).DebugDraw(Color.blue);
                }
            }
            else
            {
                _paths.Clear();
            }

        }
        public void ResetDebugPath()
        {
            _paths.Clear();
        }

        public List<IAStarGridCell> FindPath(IAStarGridCell start, IAStarGridCell end)
        {

            var output = _pathFinder.FindPath(start, end);
            _paths.AddRange(output);
            if (_paths.Count == 0)
            {
                _paths.Add(start);
                _paths.Add(end);
            }

            return output;
        }
        public void ResetAll()
        {
            _paths.Clear();
            Create();
        }
        public static GridDirection GetDirection(Transform yourObject)
        {
            float rotationOnY = yourObject.rotation.eulerAngles.y;
            float cos = Mathf.Cos(rotationOnY * Mathf.Deg2Rad);
            float sin = Mathf.Sin(rotationOnY * Mathf.Deg2Rad);

            if (cos > 0.5)
            {
                cos = 1;
            }
            else if (cos < -0.5)
            {
                cos = -1;
            }
            else
            {
                cos = 0;
            }
            if (sin > 0.5)
            {
                sin = 1;
            }
            else if (sin < -0.5)
            {
                sin = -1;
            }
            else
            {
                sin = 0;
            }
            if (cos == 1 && sin == 0)
            {
                return GridDirection.UP;

            }
            if (cos == -1 && sin == 0)
            {

                return GridDirection.DOWN;
            }
            if (cos == 0 && sin == -1)
            {

                return GridDirection.LEFT;
            }
            if (cos == 0 && sin == 1)
            {

                return GridDirection.RIGHT;
            }
            return GridDirection.NaN;
        }

        public bool IsWithinBounds(int newX, int newY)
        {
            return newX >= 0 && newX < _grid.GetLength(0) && newY >= 0 && newY < _grid.GetLength(1);
        }
    }

}