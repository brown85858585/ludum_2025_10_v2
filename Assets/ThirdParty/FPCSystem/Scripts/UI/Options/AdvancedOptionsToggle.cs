using UnityEngine;
using UnityEngine.UI;

public class AdvancedOptionsToggle : MonoBehaviour {

    public GameObject ObjectToActivate;
    private Toggle myToggle;

	void Start ()
    {
        myToggle = GetComponent<Toggle>();
        ObjectToActivate.SetActive(false);
    }
	
	public void OnToggleValueChange ()
    {
        ObjectToActivate.SetActive(myToggle.isOn);
    }
}
