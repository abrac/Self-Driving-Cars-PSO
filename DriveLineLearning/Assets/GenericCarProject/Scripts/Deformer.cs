using UnityEngine;
using System.Collections;

public class Deformer : MonoBehaviour {	
	MeshCollider meshCollider;
	Mesh mesh;
	
	void Awake () {
		meshCollider = transform.Find("BodyCollider").GetComponent<MeshCollider>();
		mesh = transform.Find("Body").GetComponent<MeshFilter>().mesh;
	}
	
	void ApplyMeshOnCollider() {
		meshCollider.sharedMesh = mesh;
	}
	
	public void DeformMesh (Vector3 position, float power, float inRadius) {
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		float sqrRadius = inRadius; //* inRadius;
		
		// Calculate averaged normal of all surrounding vertices	
		Vector3 averageNormal = Vector3.zero;
		for (int i=0; i<vertices.Length; i++) {
			float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
			// Early out if too far away
			if (sqrMagnitude > sqrRadius)
				continue;
	
			float distance = Mathf.Sqrt(sqrMagnitude);
			averageNormal += normals[i];
		}
		averageNormal = averageNormal.normalized;
		
		// Deform vertices along averaged normal
		for (int i=0; i<vertices.Length; i++)	{
			float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
			// Early out if too far away
			if (sqrMagnitude > sqrRadius)
				continue;
	
			float distance = Mathf.Sqrt(sqrMagnitude);			
			vertices[i] += averageNormal * power * distance; //Random.Range(power * 0.5f, power);
			print(1);
		}
		
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		//ApplyMeshOnCollider();
	}
}
