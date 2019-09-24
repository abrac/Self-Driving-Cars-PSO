using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MIConvexHull;

public class MeshMaker : MonoBehaviour
{

    /// <summary>
    /// Generates a rock from a given set of points. 
    /// </summary>       
    public static Mesh MeshFromPoints(IEnumerable<Vector3> points)
    {
        Mesh m = new Mesh();
        m.name = "Mesh";

        List<int> triangles = new List<int>();

        var vertices = points.Select(x => new Vertex(x)).ToList();

        var result = ConvexHull.Create(vertices);

        m.vertices = result.Points.Select(x => x.ToVec()).ToArray();

        var resultPoints = result.Points.ToList();

        foreach (var face in result.Faces)
        {
            triangles.Add(resultPoints.IndexOf(face.Vertices[0]));
            triangles.Add(resultPoints.IndexOf(face.Vertices[1]));
            triangles.Add(resultPoints.IndexOf(face.Vertices[2]));
        }

        m.triangles = triangles.ToArray();
        m.RecalculateNormals();

        //m = LowPolyConverter.Convert(m); //Converts the generated mesh to low poly

        return m;
    }
}
