using UnityEngine;

    public class LowPolyConverter
    {

        /// <summary>
        /// Gives the generated rock a more low-poly look.
        /// </summary>
        /// <param name="mesh">Mesh to convert.</param>
        /// <returns></returns>
        public static Mesh Convert(Mesh mesh)
        {
            //Process the triangles
            int[] triangles = mesh.triangles;

            Vector3[] oldVerts = mesh.vertices;
            Vector3[] vertices = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = oldVerts[triangles[i]];
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
