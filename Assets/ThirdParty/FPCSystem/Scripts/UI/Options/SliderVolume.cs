using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderVolume : MonoBehaviour {

    private Slider mySlider;
	
	void Start ()
    {
        mySlider = GetComponent<Slider>();
        mySlider.value = AppManager.instance.volume;
    }
	
	
	public void OnVolumeChange ()
    {
        AppManager.instance.volume = mySlider.value;
        AppManager.instance.UpdateVolume();
    }
}
