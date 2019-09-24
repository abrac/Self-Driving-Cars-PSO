using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineMeshes : MonoBehaviour
{
    public static Mesh Combine(List<GameObject> meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];

        for (int i = 0; i < meshes.Count; i++)
        {
            MeshFilter filter = meshes[i].GetComponent<MeshFilter>();
            combine[i].mesh = filter.sharedMesh;
            combine[i].transform = filter.transform.localToWorldMatrix;
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combine);

        return newMesh;
    }
}