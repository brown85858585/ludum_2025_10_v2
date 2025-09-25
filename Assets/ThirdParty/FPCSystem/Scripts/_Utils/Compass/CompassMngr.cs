using UnityEngine;
using System.Collections;

public enum CompassTypes
{
    None = 0,
    Neddle = 1,
    Bubble = 2,
    Linear = 3,
    Standard = 4
}

[System.Serializable]
public partial class CompassMngr : MonoBehaviour
{
    [Tooltip("Can the compass change by pressing F10?")]
    public bool canBeChanged;

    [Tooltip("Choose the compass type to be used in the screen")]
    public CompassTypes compassActive;

    public GameObject Compass2DBubble;

    public GameObject Compass2DNeddle;

    public GameObject Compass2DLinear;

    public GameObject Compass3D;

    public virtual void SetCompass(CompassTypes _compassActive)
    {
        this.compassActive = _compassActive;
        this.ActivateCompass();
    }

    public virtual void ActivateCompass()
    {
        switch (this.compassActive)
        {
            case CompassTypes.None:
                if (this.Compass2DNeddle.activeSelf)
                {
                    this.Compass2DNeddle.SetActive(false);
                }
                if (this.Compass2DBubble.activeSelf)
                {
                    this.Compass2DBubble.SetActive(false);
                }
                if (this.Compass2DLinear.activeSelf)
                {
                    this.Compass2DLinear.SetActive(false);
                }
                if (this.Compass3D.activeSelf)
                {
                    this.Compass3D.SetActive(false);
                }
                break;
            case CompassTypes.Neddle:
                if (!this.Compass2DNeddle.activeSelf)
                {
                    this.Compass2DNeddle.SetActive(true);
                }
                if (this.Compass2DBubble.activeSelf)
                {
                    this.Compass2DBubble.SetActive(false);
                }
                if (this.Compass2DLinear.activeSelf)
                {
                    this.Compass2DLinear.SetActive(false);
                }
                if (this.Compass3D.activeSelf)
                {
                    this.Compass3D.SetActive(false);
                }
                break;
            case CompassTypes.Bubble:
                if (this.Compass2DNeddle.activeSelf)
                {
                    this.Compass2DNeddle.SetActive(false);
                }
                if (!this.Compass2DBubble.activeSelf)
                {
                    this.Compass2DBubble.SetActive(true);
                }
                if (this.Compass2DLinear.activeSelf)
                {
                    this.Compass2DLinear.SetActive(false);
                }
                if (this.Compass3D.activeSelf)
                {
                    this.Compass3D.SetActive(false);
                }
                break;
            case CompassTypes.Linear:
                if (this.Compass2DNeddle.activeSelf)
                {
                    this.Compass2DNeddle.SetActive(false);
                }
                if (this.Compass2DBubble.activeSelf)
                {
                    this.Compass2DBubble.SetActive(false);
                }
                if (!this.Compass2DLinear.activeSelf)
                {
                    this.Compass2DLinear.SetActive(true);
                }
                if (this.Compass3D.activeSelf)
                {
                    this.Compass3D.SetActive(false);
                }
                break;
            case CompassTypes.Standard:
                if (this.Compass2DNeddle.activeSelf)
                {
                    this.Compass2DNeddle.SetActive(false);
                }
                if (this.Compass2DBubble.activeSelf)
                {
                    this.Compass2DBubble.SetActive(false);
                }
                if (this.Compass2DLinear.activeSelf)
                {
                    this.Compass2DLinear.SetActive(false);
                }
                if (!this.Compass3D.activeSelf)
                {
                    this.Compass3D.SetActive(true);
                }
                break;
        }
    }

    void Awake()
    {
        this.Compass3D.transform.parent = Camera.main.transform;
    }

    void Start()
    {
        this.ActivateCompass();
    }

    void Update()
    {
        if (!this.canBeChanged)
            this.enabled = false;

        if (InputManager.instance.f10Key.isDown)
        {
            this.compassActive = this.compassActive + 1;
            if (this.compassActive == (CompassTypes) 5)
            {
                this.compassActive = (CompassTypes) 0;
            }
        }
        this.ActivateCompass();
    }

    public CompassMngr()
    {
        this.compassActive = CompassTypes.Neddle;
    }

}