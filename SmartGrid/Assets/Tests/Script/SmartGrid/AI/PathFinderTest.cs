using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using SmartGrid;

public class PathFinderTest
{
    // A Test behaves as an ordinary method
    [UnityTest]
    public IEnumerator FindPathTest()
    {
        GameObject go = new GameObject();
        SmartGridController grid = go.AddComponent<SmartGridController>();
        yield return null;

        SmartCell start = grid.Grid[0, 0];
        SmartCell end = grid.Grid[0, 8];

        yield return null;
        var path = grid.FindPath(start, end);

        // Predict the path
        List<SmartCell> predictPath1 = new List<SmartCell>
    {
        grid.Grid[0, 1],
        grid.Grid[0, 2],
        grid.Grid[0, 3],
        grid.Grid[0, 4],
        grid.Grid[0, 5],
        grid.Grid[0, 6],
        grid.Grid[0, 7],
        end
    };

        for (int i = 0; i < predictPath1.Count; i++)
        {
            Assert.AreEqual(predictPath1[i], path[i]);
        }

        yield return null;

        // Test the second case
        start = grid.Grid[0, 0];
        end = grid.Grid[3, 2];

        yield return null;
        path = grid.FindPath(start, end);

        // Predict the second path
        List<SmartCell> predictPath2 = new List<SmartCell>
    {
        grid.Grid[1, 0],
        grid.Grid[2, 0],
        grid.Grid[3, 0],
        grid.Grid[3, 1],
        end
    };

        for (int i = 0; i < predictPath2.Count; i++)
        {
            Assert.AreEqual(predictPath2[i], path[i]);
        }

        yield return null;
    }



}
