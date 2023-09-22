using UnityEngine;
namespace SmartGrid
{

    public interface ISmartCell
    {
        public int X { get; }
        public int Y { get; }
        public bool IsOccupied { get; }
        public GameObject OccupiedBy { get; }
        void Free();
        void ForceOccupation(object caller, GameObject occupant);
    }
}