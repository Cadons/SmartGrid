using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SmartGrid;
public class SmartGridTest
{
    private SmartGridController grid;
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        grid = new GameObject().AddComponent<SmartGridController>();
        yield return null;

    }
    [UnityTest]
    public IEnumerator CreateTest()
    {
  
        grid.Create();
        Assert.AreEqual(10, grid.Grid.GetLength(0));
        Assert.AreEqual(10, grid.Grid.GetLength(1));
      
       yield return null;
    }

    [UnityTest]
    public IEnumerator ShowGridTest()
    {
        grid.ShowGrid();
        Assert.NotNull(grid.gameObject.GetComponent<MeshRenderer>());
        Assert.NotNull(grid.gameObject.GetComponent<MeshFilter>());
        Assert.AreEqual(grid.transform.localScale, new Vector3(10,1,10));
        yield return null;

    }
    [UnityTest]
    public IEnumerator IsOccupiedTest()
    {
        grid.transform.position= Vector3.zero;
        grid.Create();
        Assert.IsFalse(grid.IsOccupied(0, 0));
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoxCollider>();
        cube.transform.position = new Vector3(-4.50f, 0.1f, 4.50f);
       
        for(int i = 0;i < 10; i++)
        {
            for(int j= 0;j<10;j++)
            {
                grid.Grid[i,j].UpdateOccupationStatus();
            }
            yield return null;

        }
        grid.Grid[0,0].UpdateOccupationStatus();
        Assert.IsTrue(grid.IsOccupied(0,0));

        yield return null;

    }

}
