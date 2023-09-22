using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SmartGrid;

namespace SmartGrid.BSP
{
    /// <summary>
    /// This is an example of implementation of IWorldGenerator interface. This class generates a dungeon map using BSP algorithm and through Perlin Noise it fills the world with enviroment.
    /// </summary>
    [RequireComponent(typeof(SmartGridController))]
    public class DungonMapProceduralGenerator : MonoBehaviour, IWorldGenerator
    {
        #region Variables
        [Header("Rooms Count")]
        [SerializeField]
        [Min(1)]
        private int _maxRoomsCount = 1;

        [Header("Room Size")]
        [SerializeField]
        [Min(1)]
        private int _minWidth = 1;

        [SerializeField]
        [Min(1)]
        private int _minHeight = 1;

        [SerializeField]
        [Min(1)]
        private int _maxWidth = 1;

        [SerializeField]
        [Min(1)]
        private int _maxHeight = 1;
        [SerializeField]
        private SmartGridController _smartGrid;

        private List<Room> _rooms = new List<Room>();
        private List<GameObject> _worldElements = new List<GameObject>();
        private GameObject _world;
        private SmartCell _spawn;
        private SmartCell _destination;

        [Header("Map Prefabs")]
        [SerializeField]
        private GameObject _wallPrefab;
        [SerializeField]
        private GameObject _floorPrefab;
        [SerializeField]
        private GameObject _rockPrefab;
        [SerializeField]
        private GameObject _grassPrefab;
        [SerializeField]
        private GameObject _soilPrefab;
        [SerializeField]
        [Range(1, 10)]
        private int _enviromentHeightMultiplier = 1;

        [SerializeField]
        private GameObject _spawnIndicatorPrefab;
        [SerializeField]
        private GameObject _destinationIndicatorPrefab;
        [SerializeField]
        private GameObject _doorPrefab;
        #endregion
        private void Start()
        {
            if (_smartGrid == null)
                _smartGrid = GetComponent<SmartGridController>();
        }

        public void BuildWorld()
        {
            if (_smartGrid != null)
            {
                _smartGrid.SetIsEditable(true);
                Partition();

                HashSet<SmartCell> corridors = BuildCorridors();
                foreach (Room room in _rooms)
                {
                    _worldElements.Add(room.Build(corridors, _wallPrefab, _doorPrefab));
                }
                FillWorld(corridors);
                if (_spawnIndicatorPrefab != null && _destinationIndicatorPrefab != null)
                {
                    _worldElements.Add(Instantiate(_spawnIndicatorPrefab, _spawn.LocalPosition, Quaternion.identity));
                    _worldElements.Add(Instantiate(_destinationIndicatorPrefab, _destination.LocalPosition, Quaternion.identity));
                }
        
                foreach (GameObject gameObject in _worldElements)
                {
                    gameObject.transform.SetParent(_world.transform);
                }
                _smartGrid.SetIsEditable(false);
            }
            else
            {
                Debug.LogError("SmartGridController not setted!");
            }
        }

        #region Coordidors
        private HashSet<SmartCell> BuildCorridors()
        {
            List<SmartCell> roomCenters = new List<SmartCell>();
            foreach (Room room in _rooms)
            {
                roomCenters.Add(room.Center);
            }
            HashSet<SmartCell> corridors = ConnectRoom(roomCenters);
            if (corridors.Count == 0)
            {
                return new HashSet<SmartCell>();
            }
            _spawn = corridors.First();

            _destination = corridors.Last();
            GameObject corridorsObj = new GameObject("corridors");
            corridorsObj.transform.SetParent(_world.transform);

            foreach (SmartCell cell in corridors)
            {

                bool isInside = false;
                foreach (Room room in _rooms)
                {
                    isInside = room.IsInside(cell.X, cell.Y);
                    if (isInside)
                    {
                        break;
                    }
                }
                if (!isInside)
                {
                    _worldElements.Add(Instantiate(_floorPrefab, cell.LocalPosition, Quaternion.identity, corridorsObj.transform));
                }

            }
            return corridors;
        }
        private SmartCell FindClosest(SmartCell currentRoomCenter, List<SmartCell> roomCenters)
        {
            SmartCell closest = _smartGrid.Grid[0, 0];
            float distance = float.MaxValue;
            foreach (SmartCell cell in roomCenters)
            {
                //Manhattan distance
                float currentDistance = Mathf.Abs(cell.X - closest.X) + Mathf.Abs(cell.Y - closest.Y);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closest = cell;
                }
            }
            return closest;
        }
        private HashSet<SmartCell> ConnectRoom(List<SmartCell> roomCenters)
        {
            HashSet<SmartCell> corridors = new HashSet<SmartCell>();
            if (roomCenters.Count == 0)
            {
                return new HashSet<SmartCell>();
            }
            var currentRoomCenter = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];

            roomCenters.Remove(currentRoomCenter);

            while (roomCenters.Count > 0)
            {
                SmartCell closest = FindClosest(currentRoomCenter, roomCenters);
                roomCenters.Remove(closest);
                HashSet<SmartCell> newCorridor = CreateCorridor(currentRoomCenter, closest);
                currentRoomCenter = closest;
                corridors.UnionWith(newCorridor);


            }


            return corridors;
        }
        #endregion
        #region WorldGeneration
        private void FillWorld(HashSet<SmartCell> cooridors)
        {
            float scale = 0.1f; // Adjust the scale to control the Perlin noise pattern
            float xOffset = UnityEngine.Random.Range(0f, 1000f); // Randomize the offset for variety
            float yOffset = UnityEngine.Random.Range(0f, 1000f);
            GameObject terrain = new GameObject("Terrain");
            terrain.transform.SetParent(_world.transform);
            for (int i = 0; i < _smartGrid.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < _smartGrid.Grid.GetLength(1); j++)
                {
                    bool zombieCell = true;
                    if (cooridors.Contains(_smartGrid.Grid[i, j]))
                    {
                        zombieCell = false;
                    }
                    else
                    {
                        foreach (Room room in _rooms)
                        {
                            if (room.IsInside(i, j))
                            {
                                zombieCell = false;
                            }
                        }
                    }
                    if (zombieCell)
                    {
                        if (_smartGrid.Grid[i, j] != null)
                        {
                            // Calculate Perlin noise value using scaled and offset indices
                            float height = Mathf.PerlinNoise((float)i * scale + xOffset, (float)j * scale + yOffset);

                            GameObject obj = null;
                            if (height < 0.2f)
                                obj = Instantiate(_grassPrefab, _smartGrid.Grid[i, j].LocalPosition, Quaternion.identity);
                            else if (height < 0.5f)
                                obj = Instantiate(_soilPrefab, _smartGrid.Grid[i, j].LocalPosition, Quaternion.identity);
                            else
                                obj = Instantiate(_rockPrefab, _smartGrid.Grid[i, j].LocalPosition, Quaternion.identity);

                            obj.transform.localScale = new Vector3(obj.transform.localScale.x, _enviromentHeightMultiplier * height, obj.transform.localScale.z);
                            obj.transform.SetParent(terrain.transform);
                            _worldElements.Add(obj);

                        }
                    }
                }
            }
        }


        private HashSet<SmartCell> CreateCorridor(SmartCell currentRoomCenter, SmartCell destination)
        {
            HashSet<SmartCell> corridor = new HashSet<SmartCell>();
            var position = currentRoomCenter;
            corridor.Add(position);
            while (position.Y != destination.Y)
            {
                if (destination.Y > position.Y)
                {

                    position = _smartGrid.Grid[position.X, position.Y + 1];//is guaranteed that elements are in the grid bounds because we used bsp
                }
                else
                {
                    position = _smartGrid.Grid[position.X, position.Y - 1];

                }
                corridor.Add(position);
            }
            while (position.X != destination.X)
            {
                if (destination.X > position.X)
                {

                    position = _smartGrid.Grid[position.X + 1, position.Y];//is guaranteed that elements are in the grid bounds because we used bsp
                }
                else
                {
                    position = _smartGrid.Grid[position.X - 1, position.Y];

                }
                corridor.Add(position);

            }

            return corridor;
        }
        private void Partition()
        {
            Room.ResetIdCounter();

            if (_smartGrid.Grid[0, 0] == null)
                return;

            try
            {
                Clean();



                _world = new GameObject("World");
                //BSP start from this big room
                Room startingRoom = new Room(0, _smartGrid.Columns - 1, 0, _smartGrid.Rows - 1, _smartGrid);//Create a room with the grid's sizes
                                                                                                            //Build it



                _rooms = WorldPartitioner.Instance.BuildMap(startingRoom, _smartGrid, _maxRoomsCount, _minWidth, _minHeight, _maxWidth, _maxHeight);



            }
            catch (MapCreationException e)
            {
                Partition();
            }



        }
        private void Clean()
        {
            GameObject.FindObjectsOfType<GameObject>().ToList().FindAll(e => e.name.Equals("World")).ForEach(e => DestroyImmediate(e));

            if (_smartGrid != null)
                _smartGrid.ResetAll();
            _rooms.Clear();
            _worldElements.Clear();
        }

        #endregion

    }

}