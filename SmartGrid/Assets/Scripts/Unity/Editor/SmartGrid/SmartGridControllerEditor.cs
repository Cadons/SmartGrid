#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SmartGrid;
[CustomEditor(typeof(SmartGridController))]
public class SmartGridControllerEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        string buttonTxt="Create";
        if (((SmartGridController)target).Grid[0,0]!=null)
        {
            if (GUILayout.Button("Update"))
            {
                SmartGridController grid = (SmartGridController)target;
                grid.Create();

            }
        }
        else
        {
            if (GUILayout.Button("Create"))
            {
                SmartGridController grid = (SmartGridController)target;
                grid.Create();

            }
        }
       
        
    }

}
#endif