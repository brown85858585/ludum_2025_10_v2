// [Colorize("Color", bool ColorizeAll)] // All values are optionals
// [Label("NameOfVar", "ToolTip", "Color", bool ColorizeAll)] // First value is needed.
// [Comment("Nombre","ToolTip", "Color", "Anchura")] // First var is needed.


using UnityEngine;
using System.Collections;

public class Test1 : MonoBehaviour {
	
	[Comment("Esto es una linea de texto","ToolTip de mi linea de texto, a modo de demostracion", "Color.red", 20)]
	public int Comentario1;
	
	[Colorize("Color.red", true)]
	public float speed = 40.5f;
	
	[Label("Posicion de la esfera", "Podemos cambiar la posicion de la esfera desde el inspector", "Color.gray", true)]
	public Vector3 posicion = Vector3.one;

	[Label("Texto", "", "Color.blue", false)]
	public string txt = "Mi Texto";
	
	
	[Comment("Esto es una linea2 de texto","ToolTip de mi linea2 de texto, a modo de demostracion", "Color.red", 20)]
	public int Comentario2;
	
	[Colorize]
	public int giro = 3;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = posicion;
	}
}
