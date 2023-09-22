using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;
using SmartGrid;
public class SmartCellTests
{
    private SmartCell smartCell;
    private float edge = 1f;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Vector3 localPosition = Vector3.zero;
        smartCell = new SmartCell(localPosition, edge, new SmartGridController(),0,0);
        yield return null;

    }

    [UnityTest]
    public IEnumerator TestUpdatePosition()
    {
        Vector3 newPosition = new Vector3(3, 5, 2);

        smartCell.UpdatePosition(newPosition);
        Assert.AreEqual(3, smartCell.LocalPosition.x);
        Assert.AreEqual(5, smartCell.LocalPosition.y);
        Assert.AreEqual(2, smartCell.LocalPosition.z);
        yield return null;

    }

    [UnityTest]
    public IEnumerator TestUpdateOccupationStatus()
    {
        smartCell.UpdateOccupationStatus();
        Assert.IsFalse(smartCell.IsOccupied);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<BoxCollider>();
        cube.transform.position= Vector3.zero;
        smartCell.UpdateOccupationStatus();
        Assert.IsTrue(smartCell.IsOccupied);
        yield return null;
    }



}
