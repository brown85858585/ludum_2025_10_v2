using UnityEngine;
using System.Collections;

//[AddComponentMenu( "Utilities/AutoSpin")]
public class AutoSpin : MonoBehaviour {
	
	public Vector3 speed = new Vector3 (0, 60, 0); // Speed of the rotation in each axis.
	public bool local = false;		// Is the rotation in local space (or in global)
    public bool allowNegativeValues = true;
	
	private Transform MyTransform;
	
	void Start()
    {
		MyTransform = this.transform;
	}
	
	void Update() {
		if(speed.x > 0 || (allowNegativeValues && speed.x < 0))
        {
			if(local)
				MyTransform.Rotate(Vector3.right, speed.x * Time.deltaTime, Space.Self);
			else
				MyTransform.Rotate(Vector3.right, speed.x * Time.deltaTime, Space.World);
		}

		if(speed.y > 0 || (allowNegativeValues && speed.y < 0))
        {
			if(local)
				MyTransform.Rotate(Vector3.up, speed.y * Time.deltaTime, Space.Self);
			else
				MyTransform.Rotate(Vector3.up, speed.y * Time.deltaTime, Space.World);
		}

		if(speed.z > 0 || (allowNegativeValues && speed.z < 0))
        {
			if(local)
				MyTransform.Rotate(Vector3.forward, speed.z * Time.deltaTime, Space.Self);
			else
				MyTransform.Rotate(Vector3.forward, speed.z * Time.deltaTime, Space.World);
		}
	}
	
}