using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSensitivity : MonoBehaviour {

    private Slider mySlider;
	
	void Start ()
    {
        mySlider = GetComponent<Slider>();
        mySlider.value = AppManager.instance.sensitivity;
    }
	
	
	public void OnSensibityChange()
    {
        AppManager.instance.sensitivity = mySlider.value;
    }
}
