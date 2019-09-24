using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode()]
public class CurveImplementation : MonoBehaviour
{
    private List<Vector3> points = new List<Vector3>(); //All points of the spline

    private List<Vector3> inner = new List<Vector3>(); //All points of the spline
    private List<Vector3> outer = new List<Vector3>(); //All points of the spline

    public bool drawEdges = true;

    private Vector3[] CurveCoordinates;
    private Vector3[] Tangents;

    public List<GameObject> ControlPoints;
    public int CurveResolution = 10;
    public float extrude = 10;
    public float edgeWidth = 2;
    public float thickness = 1;

    public bool debug = true;

    private bool ClosedLoop = true;

    [HideInInspector]
    public List<Vector3> waypoints = new List<Vector3>();

    public bool generateWaypoints = true;
    public float waypointResolution = 5;


    void Update()
    {
        DrawSpline(false);
    }

    public void DrawSpline (bool store)
    {
        if (store)
        {
            points.Clear();
            inner.Clear();
            outer.Clear();

            if (generateWaypoints)
                waypoints.Clear();
        }

        Vector3 p0;
        Vector3 p1;
        Vector3 m0;
        Vector3 m1;

        int pointsToMake;

        if (ClosedLoop == true)
        {
            pointsToMake = (CurveResolution) * (ControlPoints.Count);
        }
        else
        {
            pointsToMake = (CurveResolution) * (ControlPoints.Count - 1);
        }

        if (pointsToMake > 0) //Prevent Number Overflow
        {
            CurveCoordinates = new Vector3[pointsToMake];
            Tangents = new Vector3[pointsToMake];

            int closedAdjustment = ClosedLoop ? 0 : 1;

            // First for loop goes through each individual control point and connects it to the next, so 0-1, 1-2, 2-3 and so on
            for (int i = 0; i < ControlPoints.Count - closedAdjustment; i++)
            {
                p0 = ControlPoints[i].transform.position;
                p1 = (ClosedLoop == true && i == ControlPoints.Count - 1) ? ControlPoints[0].transform.position : ControlPoints[i + 1].transform.position;

                // Tangent calculation for each control point
                // Tangent M[k] = (P[k+1] - P[k-1]) / 2
                // With [] indicating subscript

                // m0
                if (i == 0)
                {
                    m0 = ClosedLoop ? 0.5f * (p1 - ControlPoints[ControlPoints.Count - 1].transform.position) : p1 - p0;
                }
                else
                {
                    m0 = 0.5f * (p1 - ControlPoints[i - 1].transform.position);
                }

                // m1
                if (ClosedLoop)
                {
                    if (i == ControlPoints.Count - 1)
                    {
                        m1 = 0.5f * (ControlPoints[(i + 2) % ControlPoints.Count].transform.position - p0);
                    }
                    else if (i == 0)
                    {
                        m1 = 0.5f * (ControlPoints[i + 2].transform.position - p0);
                    }
                    else
                    {
                        m1 = 0.5f * (ControlPoints[(i + 2) % ControlPoints.Count].transform.position - p0);
                    }
                }
                else
                {
                    if (i < ControlPoints.Count - 2)
                    {
                        m1 = 0.5f * (ControlPoints[(i + 2) % ControlPoints.Count].transform.position - p0);
                    }
                    else
                    {
                        m1 = p1 - p0;
                    }
                }

                Vector3 position;
                float t;
                float pointStep = 1.0f / CurveResolution;

                if ((i == ControlPoints.Count - 2 && ClosedLoop == false) || (i == ControlPoints.Count - 1 && ClosedLoop))
                {
                    pointStep = 1.0f / (CurveResolution - 1);
                    // last point of last segment should reach p1
                }
                // Second for loop actually creates the spline for this particular segment
                for (int j = 0; j < CurveResolution; j++)
                {
                    t = j * pointStep;
                    Vector3 tangent;
                    position = CatmullRom.Interpolate(p0, p1, m0, m1, t, out tangent);
                    CurveCoordinates[i * CurveResolution + j] = position;
                    Tangents[i * CurveResolution + j] = tangent;

                    if (debug) //Normals
                    {
                        Debug.DrawLine(position + Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2 + transform.position, position - Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2 + transform.position, Color.red);
                        Debug.DrawLine(position + Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2 + transform.position, position + Vector3.Cross(tangent, Vector3.up).normalized * (extrude / 2 + edgeWidth) + transform.position, Color.green); //Edge +
                        Debug.DrawLine(position - Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2 + transform.position, position - Vector3.Cross(tangent, Vector3.up).normalized * (extrude / 2 + edgeWidth) + transform.position, Color.green); //Edge -
                    }

                    if (store)
                    {
                        points.Add(position - Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2);
                        points.Add(position);
                        points.Add(position + Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2);

                        inner.Add(position - Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2);
                        inner.Add(position - Vector3.Cross(tangent, Vector3.up).normalized * (extrude / 2 + edgeWidth));

                        outer.Add(position + Vector3.Cross(tangent, Vector3.up).normalized * extrude / 2);
                        outer.Add(position + Vector3.Cross(tangent, Vector3.up).normalized * (extrude / 2 + edgeWidth));
                    }

                }

                for (int j = 0; j < waypointResolution; j++)
                {
                    t = j * 1.0f / waypointResolution;
                    Vector3 tangent;
                    position = CatmullRom.Interpolate(p0, p1, m0, m1, t, out tangent);
                
                    if (debug && generateWaypoints) //Waypoints
                    {
                        Debug.DrawLine(position, position + new Vector3(0, 25, 0), Color.blue);
                    }

                    if (store && generateWaypoints)
                    {
                        waypoints.Add(position);
                    }
                
                }
            }

            //Curve line
            for (int i = 0; i < CurveCoordinates.Length - 1; ++i)
            {
                Debug.DrawLine(CurveCoordinates[i] + transform.position, CurveCoordinates[i + 1] + transform.position, Color.cyan);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            if (generateWaypoints)
            {
                //Waypoints
                for (int i = 0; i < waypoints.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(waypoints[i] + new Vector3(0, 25, 0), 2);

                    if (i == waypoints.Count - 1)
                    {
                        Debug.DrawLine(waypoints[i] + new Vector3(0, 25, 0), waypoints[0] + new Vector3(0, 25, 0), Color.blue);
                    }
                    else
                    {
                        Debug.DrawLine(waypoints[i] + new Vector3(0, 25, 0), waypoints[i + 1] + new Vector3(0, 25, 0), Color.blue);
                    }
                }
            }

            //Control Point Gizmo
            Gizmos.color = Color.cyan;
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                Gizmos.DrawSphere(ControlPoints[i].transform.position, extrude/2);
            }

            
        }
    }

  
    public void GenerateMesh ()
    {

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (transform.GetChild(i).gameObject.name != "Control Points")
                DestroyImmediate(transform.GetChild(i).gameObject);
        }

        Mesh combinedMesh = EdgeExtruder.LinearMesh(points, CurveResolution * ControlPoints.Count, thickness, 3);

        GetComponent<MeshFilter>().sharedMesh = combinedMesh;


        if (drawEdges)
        {
            DrawInner();
            DrawOuter();
        }

    }


    private void DrawInner()
    {
        Mesh mesh = EdgeExtruder.LinearMesh(inner, CurveResolution * ControlPoints.Count, thickness, 2);

        GameObject piece = new GameObject("Inner Edge");

        piece.transform.SetParent(transform);
        piece.transform.position = transform.position;

        piece.AddComponent<MeshFilter>().sharedMesh = mesh;
        MeshRenderer rend = piece.AddComponent<MeshRenderer>();

        rend.sharedMaterial = new Material(Shader.Find("Standard"));
        rend.sharedMaterial.color = Color.white;
    }

    private void DrawOuter()
    {
        Mesh mesh = EdgeExtruder.LinearMesh(outer, CurveResolution * ControlPoints.Count, thickness, 2);

        GameObject piece = new GameObject("Outer Edge");

        piece.transform.SetParent(transform);
        piece.transform.position = transform.position;

        piece.AddComponent<MeshFilter>().sharedMesh = mesh;
        MeshRenderer rend = piece.AddComponent<MeshRenderer>();

        rend.sharedMaterial = new Material(Shader.Find("Standard"));
        rend.sharedMaterial.color = Color.white;
    }
}

