using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode()]
public class CatmullRom
{
    public GameObject[] Points = new GameObject[4];
    public int CurveResolution = 20;
    public Vector3[] CurveCoordinates;
    public Vector3[] Tangents;
    public bool ClosedLoop = false;

    public enum Uniformity
    {
        Uniform,
        Centripetal,
        Chordal
    }

    public static Vector3 Interpolate(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
    {
        // Catmull-Rom splines are Hermite curves with special tangent values.
        // Hermite curve formula:
        // (2t^3 - 3t^2 + 1) * p0 + (t^3 - 2t^2 + t) * m0 + (-2t^3 + 3t^2) * p1 + (t^3 - t^2) * m1
        // For points p0 and p1 passing through points m0 and m1 interpolated over t = [0, 1]
        // Tangent M[k] = (P[k+1] - P[k-1]) / 2
        // With [] indicating subscript
        Vector3 position = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * start
            + (t * t * t - 2.0f * t * t + t) * tanPoint1
            + (-2.0f * t * t * t + 3.0f * t * t) * end
            + (t * t * t - t * t) * tanPoint2;

        return position;
    }

    public static Vector3 Interpolate(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t, out Vector3 tangent)
    {
        // Calculate tangents
        // p'(t) = (6t² - 6t)p0 + (3t² - 4t + 1)m0 + (-6t² + 6t)p1 + (3t² - 2t)m1
        tangent = (6 * t * t - 6 * t) * start
            + (3 * t * t - 4 * t + 1) * tanPoint1
            + (-6 * t * t + 6 * t) * end
            + (3 * t * t - 2 * t) * tanPoint2;
        return Interpolate(start, end, tanPoint1, tanPoint2, t);
    }
       
}