using UnityEngine;
using System.Collections;

public class Compass3D : MonoBehaviour
{
    public float neddleSpeed = 5; // neddle rotation speed
    public float maxNeedleVibration = 2; // Maximum neddle ramdom innacuracy.

	public bool setCameraChild = true;		// Position the camera 3d as a child of pl.
	public Vector3 compassLocalPosition = new Vector3(0f, -0.4613f, 1.094f); // local position of the compass
	public Vector3 compassLocalRotation = new Vector3(30f, 180f, 0f);		// Local Rotation of the compass

    private Transform PlayerTransform;
    private float rot; // Players actual rotation
    private float internalAngle; // Player lerped rotation speed using the  speedneddle).


    public float GetCurrentAngle()
    {
        return internalAngle;
    }

    void Start()
    {
        PlayerTransform = Camera.main.transform.root.transform;

		if (setCameraChild){
			transform.parent.parent = Camera.main.transform;
			transform.parent.transform.localPosition = compassLocalPosition;
			transform.parent.transform.localEulerAngles = compassLocalRotation;
		}
    }

    void Update()
    {
        rot = PlayerTransform.eulerAngles.y; // Player Rotation.
        
		float velo = 0;
        internalAngle = Mathf.SmoothDampAngle(internalAngle, rot, ref velo, Time.deltaTime * neddleSpeed); // Rotation lerped value.
        internalAngle = internalAngle + Random.Range(-maxNeedleVibration, maxNeedleVibration); // neddle innacuracy.
        
		Vector3 rotAux = PlayerTransform.eulerAngles;
        rotAux.y = internalAngle;
        transform.localEulerAngles = rotAux;
    }

}