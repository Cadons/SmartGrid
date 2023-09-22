using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SmartGrid.BSP
{

    public class MapCreationException : Exception
    {
        public MapCreationException(string message) : base(message)
        {
        }

    }
}