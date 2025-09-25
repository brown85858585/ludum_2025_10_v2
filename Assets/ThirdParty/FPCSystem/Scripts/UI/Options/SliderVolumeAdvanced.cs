using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderVolumeAdvanced : MonoBehaviour {

    public enum VolumeType { FX = 0, Music= 1 }
    public VolumeType volumeType = VolumeType.FX;

    private Slider mySlider;

    void Start()
    {
        mySlider = GetComponent<Slider>();
        switch (volumeType)
        {
            case VolumeType.FX:
                mySlider.value = SoundManager.instance.GetOriginalFXVolume();
                break;
            case VolumeType.Music:
                mySlider.value = SoundManager.instance.GetOriginalMusicVolume();
                break;
        }
    } 
	
	
	public void OnVolumeChange ()
    {
        switch (volumeType)
        {
            case VolumeType.FX:
                SoundManager.instance.SetOriginalFXVolume(mySlider.value);
                break;
            case VolumeType.Music:
                SoundManager.instance.SetOriginalMusicVolume(mySlider.value);
                break;
        }

        SoundManager.instance.SaveSoundMngrData();
        SoundManager.instance.UpdateAgainstMasterVolume();
    }
}
