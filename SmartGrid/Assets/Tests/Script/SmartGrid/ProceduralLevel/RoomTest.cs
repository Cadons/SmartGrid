using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using SmartGrid.BSP;
using SmartGrid;
public class RoomTest
{

    [UnityTest]
    public IEnumerator BuildTest()
    {
        Room.ResetIdCounter();
        GameObject mainObject = new GameObject("Main");
        SmartGridController grid = mainObject.AddComponent<SmartGridController>();
        Assert.Throws<InvalidRoomException>(() => new Room(0, 20, 0, 2, grid));
        Assert.Throws<InvalidRoomException>(() => new Room(0, 20, -10, 2, grid));
        grid.SetIsEditable(true);
        grid.Create();
        Room room = new Room(0, 2, 0, 2, grid);
   
        Assert.NotNull(room.Build(new HashSet<SmartCell>(), new GameObject("wall")));
        Assert.True(SceneManager.GetActiveScene().GetRootGameObjects().Length > 1);
        Assert.AreEqual(GameObject.Find("Room "+room.Id).transform.childCount,9);


        Assert.NotNull(room.Build(null, new GameObject("wall")));
        Assert.Throws<NoElementToBuildRoomException>(() => room.Build(null, null));
        yield return null;
    }
}
