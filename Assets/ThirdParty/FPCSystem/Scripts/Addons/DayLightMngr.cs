
using UnityEngine;
using System.Collections;


public class DayLightMngr : MonoBehaviour
{
    [Tooltip("Night skybox you want to use in 'night mode'.")]
    public Material NightSkyBox;

    [Tooltip("The ambient light color you want to use in 'night mode'.")]
	public Color NightColor = Color.black;

    [Tooltip("Usually, the directional light used in the scene (to turn it off when entering in 'night mode'.")]
    public Light DayLight;

	private bool isDayLightOn = true;
    private Material DaySkyBox;
    private Color AmbienceLightOriginal;
    private Renderer[] children;
    private int[] childrenIndex;

    void Start()
    {
        DaySkyBox = RenderSettings.skybox;
        AmbienceLightOriginal = RenderSettings.ambientLight;

        // Get all renderers in the scene
        Renderer[] childRenderer = (Renderer[]) UnityEngine.Object.FindObjectsOfType(typeof(Renderer));
        
		// Count all the renderers that have a lightmap (all non-static gameobjects)
        // Some non static gameobjets like pickups can be destroyed while playing,
        // so we need to make sure we dont store all those renderers that later maybe be destroyed
        // Only static objects will be processed.
        int count = 0;
        int i = 0;
        while (i < childRenderer.Length)
        {
            if (childRenderer[i].GetComponent<Renderer>().lightmapIndex != -1)
            {
                count++;
            }
            i++;
        }

        // Create our arrays to store all lightmapped renderers and their indexes
        children = new Renderer[count];
        childrenIndex = new int[count];

        // Assign all lightmapped renderers to our new array
        int j = 0;
        i = 0;
        while (i < childRenderer.Length)
        {
            if (childRenderer[i].GetComponent<Renderer>().lightmapIndex != -1)
            {
                children[j++] = childRenderer[i];
            }
            i++;
        }

        // Assign the index to the index array.
        i = 0;
        while (i < children.Length)
        {
            childrenIndex[i] = children[i].GetComponent<Renderer>().lightmapIndex;
            i++;
        }
    }

    void Update()
    {
		// Set when we can sprint at a given Player status. Notice that you can sprint in water (Swimming) if you want.
        if (InputManager.instance.dayLightKey.isDown)
        {
            isDayLightOn = !isDayLightOn;
			SetDay(isDayLightOn);
        }
    }

    // Function to change the scene betwen Day/Night.
    public void SetDay(bool _value)
    {
        if (_value)
        {
            RenderSettings.skybox = DaySkyBox; // Change the skybox, enable/disable the light of the scene.
            RenderSettings.ambientLight = AmbienceLightOriginal; // Change the ambientLight setting of the project Render.
            DayLight.enabled = true; // Enable/disable the directional light in the scene.
            TurnLightMapsOn(); // Change the lightmap index of all the lightmapped objects (the static ones).
        }
        else
        {
            RenderSettings.skybox = NightSkyBox;
            RenderSettings.ambientLight = NightColor;
            DayLight.enabled = false;
            TurnLightMapsOff();
        }
    }

    public void TurnLightMapsOff()
    {
        int i = 0;
        while (i < children.Length)
        {
            if (children[i] != null)
            {
                children[i].lightmapIndex = -1;
            }
            i++;
        }
    }

    public void TurnLightMapsOn()
    {
        int i = 0;
        while (i < children.Length)
        {
            if (children[i] != null)
            {
                children[i].lightmapIndex = childrenIndex[i];
            }
            i++;
        }
    }

}