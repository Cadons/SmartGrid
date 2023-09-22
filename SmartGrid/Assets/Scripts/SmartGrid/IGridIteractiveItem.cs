using UnityEngine;
namespace SmartGrid
{

    public interface IGridIteractiveItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool SnapPosition { get; set; }
        SmartGridController GetGrid();
        void SetGrid(SmartGridController grid);


    }
}