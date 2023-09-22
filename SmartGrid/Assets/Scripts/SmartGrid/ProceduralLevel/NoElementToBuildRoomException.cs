using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SmartGrid.BSP
{

    public class NoElementToBuildRoomException : Exception
    {


        public NoElementToBuildRoomException()
            : base("The builder did not receive any game items to build the room.")
        {
        }


    }
}