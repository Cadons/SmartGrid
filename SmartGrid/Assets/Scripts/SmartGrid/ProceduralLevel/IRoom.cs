using System.Collections.Generic;
using UnityEngine;
namespace SmartGrid.BSP
{
    public interface IRoom
    {
        GameObject Build(HashSet<SmartCell> wallsToExclude, GameObject wallObject, GameObject door = null);
        bool IsInside(int x, int y);
    }


}