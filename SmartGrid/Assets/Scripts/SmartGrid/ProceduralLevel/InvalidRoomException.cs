using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SmartGrid.BSP
{

    public class InvalidRoomException : Exception
    {
        public InvalidRoomException() : base("Invalid Room!, Coordinates are outside the grid bounds")
        {

        }
        public InvalidRoomException(string message) : base(message)
        {
        }
    }
}