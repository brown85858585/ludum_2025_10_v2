using UnityEngine;
using System.Collections;

[System.Serializable]
public class OceanTexMovement : MonoBehaviour
{
    public bool isUnderwaterPlane = false;

	[Tooltip("Speed and direction of the scroll movement of the texture. Can have a negative value.")]
	public Vector2 UvSpeed = new Vector2(0.05f, 0.15f);

	[Tooltip("Has the material a texture assigned to the Main Texture?")]
	public bool hasMainMap = true;

	[Tooltip("Has the material a texture assigned to the bumpmap Texture (if it's bumpmapped at all)?.")]
	public bool hasBumpMap = true;

	private float offset1;
	private float offset2;
	private Material _myMaterial;

    private GameObject MyObj;
    private Status statusSrc;

    IEnumerator Start()
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

        yield return new WaitForSeconds(0.1f);		// Esperamos a que se inicialize el material de waterdrop desde Swim
		_myMaterial = GetComponent<Renderer>().material;
	}

	void Update()
	{
        if (isUnderwaterPlane) return;

        if (_myMaterial == null) { return; }
		if (!hasMainMap && !hasBumpMap) { return; }

		offset1 = Time.time * UvSpeed.x;
		offset2 = Time.time * UvSpeed.y;
		if (hasMainMap)
			_myMaterial.SetTextureOffset("_MainTex", new Vector2(-offset1, -offset2));

		if (hasBumpMap)
			_myMaterial.SetTextureOffset("_BumpMap", new Vector2(-offset1, -offset2));
	}

}