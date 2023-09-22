using SmartGrid;
using System.Collections.Generic;
using UnityEngine;
namespace SmartGrid.BSP
{

    public class WorldPartitioner
    {
        #region BSP Variables

        private SmartGridController _smartGrid;
        private List<Room> _rooms = new List<Room>();

        private int _maxRoomsCount = 1;
        private int _minWidth = 1;
        private int _minHeight = 1;
        private int _maxWidth = 1;
        private int _maxHeight = 1;
        #endregion
        #region BSP Properties
        private int MinArea { get { return (_minWidth - 1) * (_minHeight - 1); } }

        private int MaxArea { get { return (_maxWidth - 1) * (_maxHeight - 1); } }
        #endregion
        //Singleton
        private static WorldPartitioner _instance;
        public static WorldPartitioner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorldPartitioner();
                }
                return _instance;
            }
        }
        private WorldPartitioner()
        {
            //Initialize
        }

        #region BSP
        private Room Split(Room room)
        {

            if (_rooms.Count >= _maxRoomsCount)
            {
                return null;
            }
            Room output = null;
            if (room.Width < _minWidth || room.Height < _minHeight)
            {
                return null;
            }
            if ((room.Area <= MaxArea && room.Area >= MinArea && room.Width <= _maxWidth && room.Width >= _minWidth && room.Height <= _maxHeight && room.Height >= _minHeight))
            {
                output = room;
                _rooms.Add(room);
            }
            else
            {
                if (Random.Range(0, 2) == 0)
                {
                    output = SplitHorizontally(room);
                }
                else
                {
                    output = SplitVertically(room);
                }
            }
            return output;
        }
        private Room SplitHorizontally(Room room)
        {
            //Split it
            int splitPoint = Random.Range(room.Left + _minWidth, room.Right - _minWidth);

            Room leftChild = new Room(room.Left, splitPoint, room.Bottom, room.Top, _smartGrid);
            Room rightChild = new Room(splitPoint, room.Right, room.Bottom, room.Top, _smartGrid);




            room.LeftChild = Split(leftChild);
            room.RightChild = Split(rightChild);
            if (room.LeftChild != null)
                leftChild.Parent = room;
            if (room.RightChild != null)
                rightChild.Parent = room;

            //Return it
            return room;
        }
        private Room SplitVertically(Room room)
        {
            //Split it
            int splitPoint = Random.Range(room.Bottom + _minHeight, room.Top - _minHeight);

            Room bottomChild = new Room(room.Left, room.Right, room.Bottom, splitPoint, _smartGrid);
            Room topChild = new Room(room.Left, room.Right, splitPoint, room.Top, _smartGrid);



            room.LeftChild = Split(bottomChild);
            room.RightChild = Split(topChild);

            if (room.LeftChild != null)
                bottomChild.Parent = room;
            if (room.RightChild != null)
                topChild.Parent = room;
            //Return it
            return room;
        }

        public List<Room> BuildMap(Room root, SmartGridController grid, int maxRoomsCount, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            if (maxRoomsCount <= 0)
            {
                throw new MapCreationException("Invalid Room number, must be greater than 0!");

            }
            if (minWidth > maxWidth)
            {
                throw new MapCreationException("minWidth can't be bigger then maxWidth");
            }
            if (minHeight > maxHeight)
            {
                throw new MapCreationException("minHeight can't be bigger then maxHeight");
            }
            if (minHeight < 0)
            {
                throw new MapCreationException("minHeight can't be negative");
            }
            if (minWidth < 0)
            {
                throw new MapCreationException("minWidth can't be negative");
            }
            if (maxWidth < 0)
            {
                throw new MapCreationException("maxWidth can't be negative");

            }
            if (maxHeight < 0)
            {
                throw new MapCreationException("maxHeight can't be negative");

            }
            if (maxWidth > root.Width || maxHeight > root.Height || minHeight > root.Height || minWidth > root.Width)
            {
                throw new MapCreationException("Room sized are bigger then root, put smaller values");
            }
            _maxRoomsCount = maxRoomsCount;
            _minWidth = minWidth;
            _minHeight = minHeight;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;

            _smartGrid = grid;
            if (_smartGrid == null)
                return null;

            _rooms.Clear();
            Split(root);
            if (_rooms.Count <= 1)
            {
                throw new MapCreationException(string.Format("I couldn't create rooms inside root with these characteristics, retry! minWidth:{0} minHeight: {1} maxWidth:{2} maxHeight: {3}", minWidth, minHeight, maxWidth, maxHeight));
            }
            return _rooms;
        }
        #endregion

    }
}