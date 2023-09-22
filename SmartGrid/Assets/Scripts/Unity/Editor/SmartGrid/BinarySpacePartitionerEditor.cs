#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SmartGrid.BSP;
[CustomEditor(typeof(DungonMapProceduralGenerator))]
public class BinarySpacePartitionerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            //Create grid


            DungonMapProceduralGenerator bsp = (DungonMapProceduralGenerator)target;
            
            bsp.BuildWorld();
        }
    }
}

#endif