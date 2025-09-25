// [Colorize("Color", bool ColorizeAll)] // All values are optionals
// [Label("NameOfVar", "ToolTip", "Color", bool ColorizeAll)] // First value is needed.
// [Comment("Nombre","ToolTip", "Color", "Anchura")] // First var is needed.

using UnityEngine;


public class ColorizeDemo : MonoBehaviour {
	
	// Comentarios. Una linea en el inspector a modo de label solitario.
	[Comment("Probando LABEL","Label de prueba", "Color.red", 15)]
	public int Comment1;
	
	// Label : Permiten escribir nombre de la variable, tooltips y color a usar en el label de la varibale.
	
	[Label("Var1 int", "Esto es un tooltip de var1", "Color.green", true)]
	public int var1;
	[Label("Var2 bool", "Esto es un tooltip de var2", "Color.red")]
	public bool var2;
	[Label("Var3 float", "Esto es un tooltip de var3", "Color.black")]
	public float var3;
	[Label("Var3 Color", "Esto es un tooltip de var4", "Color.magenta")]
	public Color var4;
	[Label("Var5 Vector2", "Esto es un tooltip de var5", "Color.yellow", true)]
	public Vector2 var5;
	[Label("Cube Postion (var 6) --> Vector3", "Esto es un tooltip de var6", "Color.red", true)]
	public Vector3 var6;
	[Label("Var7 AnimationCurve", "Esto es un tooltip de var7", "Color.red")]
	public AnimationCurve var7;
	[Label("Var8 Bounds", "Esto es un tooltip de var8", "Color.yellow")]
	public Bounds var8;
	[Label("Var9 LayerMask", "Esto es un tooltip de var9", "Color.gray")]
	public LayerMask var9;
	[Label("Var10 GameObject", "Esto es un tooltip de var10", "Color.blue")]
	public GameObject var10;
	[Label("Var11 Rect", "Esto es un tooltip de var11", "Color.red")]
	public Rect var11;
	[Label("Var12 string", "Esto es un tooltip de var12", "Color.red")]
	public string var12 = "Test";
	public enum Days {Sat, Sun, Mon, Tue, Wed, Thu, Fri};
	[Label("Var13 enum", "Esto es un tooltip de var13", "Color.gray", true)]
	public Days var13;

	[Comment("Probando COLOR","Label de prueba de Color", "Color.magenta", 20)]
	public int Comment2;
	
	
	[Colorize("Color.red", true)]
	public int var20;
	[Colorize("Color.magenta", false)]
	public bool var21;
	[Colorize("Color.blue", true)]
	public float var22;
	[Colorize("Color.green", true)]
	public Color var23;
	[Colorize("Color.yellow", true)]
	public Vector2 var24;
	[Colorize("Color.magenta", false)]
	public Vector3 var25;
	[Colorize("Color.red", true)]
	public AnimationCurve var26;
	[Colorize("Color.gray", true)]
	public Bounds var27;
	[Colorize("Color.cyan", false)]
	public LayerMask var28;
	[Colorize("Color.magenta")]
	public GameObject var29;
	[Colorize]
	public Rect var30;
	[Colorize("Color.magenta", true)]
	public string var31 = "Test";
	public enum Days1 {Sat, Sun, Mon, Tue, Wed, Thu, Fri};
	[Colorize("Color.magenta", true)]
	public Days1 var32;
	
	void Update()
	{
		this.transform.position = var6;
	}
	
}
