using System.Collections.Generic;
using UnityEngine;
using SmartGrid;
namespace SmartGrid
{
    namespace AI
    {
        public interface IPathFinder
        {
            List<IAStarGridCell> FindPath(IAStarGridCell start, IAStarGridCell end);
        }
    }
  
}
