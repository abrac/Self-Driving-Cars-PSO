using System.Collections.Generic;
using UnityEngine;

public class EdgeExtruder : MonoBehaviour
{
    public static Mesh LinearMesh(List<Vector3> points, int iterations, float thickness, int test)
    {
        List<GameObject> objects = new List<GameObject>();

        int[] previous = new int[2];

        for (int i = 0; i < iterations - 1; i++)
        {
            List<Vector3> temp = new List<Vector3>();

            if (i == 0)
            {
                temp.Add(points[0]);
                temp.Add(points[1]);
                temp.Add(points[2]);
                temp.Add(points[3]);


                if (test == 2)
                {
                    previous = new int[]
                    {
                        2, 3
                    };
                }
                else
                {
                    temp.Add(points[0]);
                    temp.Add(points[4]);
                    temp.Add(points[5]);

                    previous = new int[]
                    {
                        3, 4, 5
                    };
                }                
            }
            else if (i < points.Count - 2)
            {
                if (test == 2)
                {
                    temp.Add(points[previous[0]]);
                    temp.Add(points[previous[1]]);
                    temp.Add(points[previous[1] + 1]);
                    temp.Add(points[previous[1] + 2]);

                    previous = new int[]
                    {
                        previous[1] + 1, previous[1] + 2
                    };
                }
                else
                {
                    temp.Add(points[previous[0]]);
                    temp.Add(points[previous[1]]);
                    temp.Add(points[previous[2]]);
                    temp.Add(points[previous[2] + 1]);
                    temp.Add(points[previous[2] + 2]);
                    temp.Add(points[previous[2] + 3]);

                    previous = new int[]
                    {
                        previous[2] + 1, previous[2] + 2, previous[2] + 3
                    };
                }
            }
            else
            {

                temp.Add(points[points.Count - 1]);
                temp.Add(points[points.Count]);
                temp.Add(points[0]);
                temp.Add(points[1]);
                
                if (test == 2)
                {
                    temp.Add(points[points.Count - 2]);
                    temp.Add(points[2]);
                }

            }

            List<Vector3> lower = new List<Vector3>(temp);

            for (int z = 0; z < lower.Count; z++)
            {
                lower[z] -= new Vector3(0, thickness, 0);
            }

            temp.AddRange(lower);

            Mesh mesh = new Mesh();

            mesh = MeshMaker.MeshFromPoints(temp);

            GameObject obj = new GameObject();

            MeshFilter filter = obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();

            filter.sharedMesh = LowPolyConverter.Convert(mesh);

            objects.Add(obj);
                        
        }

        Mesh combinedMesh = CombineMeshes.Combine(objects);
        
        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }

        return combinedMesh;
    }
	
}
