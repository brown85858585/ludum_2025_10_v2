using UnityEngine;
using System.Collections;

public class SinusCurveModifier : MonoBehaviour {
	[Tooltip("High of the waves in the plane. Because the plane is so close to the camera, this value has to be a very close to zero (0.03).")]
	public float scale = 0.02f;
	[Tooltip("Speed of the wave's movement.")]
	public float speed = 2.0f;
	private bool HasMeshCollider = false;
	private Vector3[] baseHeight;

    private MeshFilter meshF;
    private Mesh mesh;
    private MeshCollider MeshC;

    private GameObject MyObj;
    private Status statusSrc;

    void Start()
    {
        MyObj = GameObject.FindWithTag("Player"); //GameObject.Find("Player");
        if (MyObj == null)
        {
            Debug.LogError("Player NOT Found!");
        }
        else
        {
            statusSrc = MyObj.GetComponent<Status>();
        }

        meshF = GetComponent<MeshFilter>();
        mesh = meshF.mesh;

        if (HasMeshCollider)
        {
            MeshC = GetComponent<MeshCollider>();
        }
    }

    void Update ()
    {
        if (true) return;

	   //MeshFilter meshF = GetComponent(typeof(MeshFilter)) as MeshFilter;
	   //Mesh mesh = meshF.mesh;
	
	   if(baseHeight == null)
	      baseHeight = mesh.vertices;

	   Vector3[] vertices = new Vector3[baseHeight.Length];
	   for (int i=0;i<vertices.Length;i++)
       {
	      Vector3 vertex = baseHeight[i];
	      //vertex.y += Mathf.Sin(Time.time * speed+ baseHeight[i].x + baseHeight[i].y + baseHeight[i].z) * scale;
	      vertex.y += Mathf.Sin(Time.time * speed + baseHeight[i].y + baseHeight[i].z) * (scale * 0.5f)
	                + Mathf.Sin(Time.time * speed + baseHeight[i].y + baseHeight[i].x) * (scale * 0.5f);
	      vertices[i] = vertex;
	   }
	   mesh.vertices = vertices;
	   mesh.RecalculateNormals();

	   if(HasMeshCollider)
        {
	   		//MeshCollider MeshC = GetComponent(typeof(MeshCollider)) as MeshCollider;
	   		MeshC.sharedMesh = mesh;
	   }
	}
}
