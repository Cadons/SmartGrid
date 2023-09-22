using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SmartGrid.BSP;
using SmartGrid;

public class WorldPartitionerTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void BuildMapTest()
    {
        WorldPartitioner bsp=WorldPartitioner.Instance;
        Assert.IsNotNull(bsp);
   
        int maxRoomsCount = 5;
        int minWidth = 2;
        int minHeight = 2;
        int maxWidth = 5;
        int maxHeight = 5;
        SmartGridController grid = new GameObject("Grid").AddComponent<SmartGridController>();
        grid.SetIsEditable(true);
        grid.Create();
        Room room = new Room(0, 2, 0, 2, grid);
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, minHeight, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, minHeight, maxWidth, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, minHeight, int.MaxValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, minHeight, int.MaxValue, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, int.MaxValue, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, int.MaxValue, maxWidth, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, int.MaxValue, int.MaxValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, minWidth, int.MaxValue, int.MaxValue, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, minHeight, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, minHeight, maxWidth, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, minHeight, int.MaxValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, minHeight, int.MaxValue, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, int.MaxValue, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, int.MaxValue, maxWidth, int.MaxValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, int.MaxValue, int.MaxValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue));

        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, minHeight, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, minHeight, maxWidth, int.MinValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, minHeight, int.MinValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, minHeight, int.MinValue, int.MinValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, int.MinValue, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, int.MinValue, maxWidth, int.MinValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, int.MinValue, int.MinValue, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, maxRoomsCount, int.MinValue, int.MinValue, int.MinValue, int.MinValue));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, 0, minWidth, minHeight, maxWidth, maxHeight));
        Assert.Throws<MapCreationException>(() => bsp.BuildMap(room, grid, -1, minWidth, minHeight, maxWidth, maxHeight));

        grid.ResetAll();
       
        grid.Create(20,20);
        room = new Room(0, 19, 0, 19, grid);

        List<Room> rooms=bsp.BuildMap(room,grid, maxRoomsCount, minWidth, minHeight, maxWidth, maxHeight);
        Assert.IsNotNull(rooms);
        Assert.True(rooms.Count>0);
        Assert.True(rooms.Count<=maxRoomsCount);
        foreach (Room r in rooms)
        {
            Assert.True(r.Width>=minWidth);
            Assert.True(r.Height>=minHeight);
            Assert.True(r.Width<=maxWidth);
            Assert.True(r.Height<=maxHeight);
        }



    }

  
}
