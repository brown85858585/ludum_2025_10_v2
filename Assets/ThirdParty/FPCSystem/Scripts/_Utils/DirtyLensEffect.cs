
using UnityEngine;
using System.Collections;

public class DirtyLensEffect : MonoBehaviour
{
	[Disable] 
	public float fadeTime = 5;
	[Disable]
	public bool isDirtyLens = true;
	[Disable]
	public bool isWaterDrop = true;

	public Material DirtyMaterial;
	[Disable]
	public Material WaterDropsMaterial;

	//private float OrigDirtyAlpha;
	private Color OrigDirtyColor = Color.white;
	private float OrigDropAlpha;
	private Color OrigDropColor = Color.white;
	private float rate;
	private float i;
	private bool isWaterDropActive;
	private Renderer myRenderer;


	public void SetWaterDropColor()
	{
		if (WaterDropsMaterial != null)
		{
			OrigDropColor = WaterDropsMaterial.GetColor("_Color");
			OrigDropAlpha = OrigDropColor.a;
		}
	}

	void Awake()
	{
		myRenderer = GetComponent<Renderer>();
	}

	// GUIs Width and Heigth adjust. Desactivate the GUI. Get the variables needed by the this script to work.
	void Start()
	{
		//yield return new WaitForSeconds(0.1f);		// Esperamos a que se inicialize el material de waterdrop desde Swim
		DirtyMaterial = myRenderer.sharedMaterial;
		OrigDirtyColor = DirtyMaterial.GetColor("_Color");
		//OrigDirtyAlpha = OrigDirtyColor.a;
		//SetWaterDropColor();
		rate = 1f / fadeTime;
		i = 1;
		ActivateDirtyLens();
	}

	// Realiza el fade y desactiva el GUI cuando el alpha ya es cero (no se ve con alpha cero).
	void Update()
	{
		if (myRenderer.enabled)
		{
			if (i < 1f)
			{
				i = i + (Time.deltaTime * rate);
				Color ColorAux = OrigDropColor;
				ColorAux.a = Mathf.Lerp(OrigDropAlpha, 0, i);
				myRenderer.sharedMaterial.SetColor("_Color", ColorAux);
			}
			else
				if (isWaterDropActive)
				{
					if (WaterDropsMaterial != null) WaterDropsMaterial.color = OrigDropColor;
					HideGUIWaterDrop();
				}
		}
	}

	// Function will be called from the Underwater script when leaving the water.
	public void ActivateDirtyLens()
	{
		if (isDirtyLens) { myRenderer.sharedMaterial = DirtyMaterial; }

		myRenderer.enabled = isDirtyLens;
	}

	// Function will be called from the Underwater script when leaving the water.
	public void ShowGUIWaterDrop()
	{
		if (!isWaterDrop) { return; }

		i = 0;
		//renderer.sharedMaterial.SetColor("_Color", OrigDirtyColor);
		ActivateWaterDrop();
		isWaterDropActive = true;
	}

	// Function will be called from the Underwater script when exiting the water surface.
	public void HideGUIWaterDrop()
	{
		i = 1;
		//renderer.sharedMaterial.SetColor("_Color", OrigColor);
		ActivateDirtyLens();
		isWaterDropActive = false;
	}

	// Function will be called from the Underwater script when entering into the water.
	public void SetFadeTime(float _fadeTime)
	{
		fadeTime = _fadeTime;
		rate = 1f / _fadeTime;
		i = 1;
	}

	// Function will be called from the Underwater script when entering into the water surface.
	public void ActivateWaterDrop()
	{
		if (isWaterDrop && WaterDropsMaterial != null) { myRenderer.sharedMaterial = WaterDropsMaterial; }
		//gameObject.SetActive(isWaterDrop);
		myRenderer.enabled = isWaterDrop;
	}

	public void OnDestroy()
	{
		DirtyMaterial.color = OrigDirtyColor;
		if (WaterDropsMaterial != null) WaterDropsMaterial.color = OrigDropColor;
	}

}