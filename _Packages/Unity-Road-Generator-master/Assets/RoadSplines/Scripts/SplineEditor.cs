using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurveImplementation))]
public class SplineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();   

        CurveImplementation curve = (CurveImplementation)target;
        
        if (GUILayout.Button ("Instantiate"))
        {
            curve.DrawSpline(true);
            curve.GenerateMesh();
        }
    }
}
