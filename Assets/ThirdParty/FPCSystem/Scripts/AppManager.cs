using UnityEngine;

public class AppManager : PersistentSingletone<AppManager> {

    public bool reset = false;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 5f)] public float sensitivity = 1f;

    public virtual void UpdateVolume()
    {
        SoundManager.instance.masterVolume = volume;
        SoundManager.instance.UpdateAgainstMasterVolume();
    }

    //=======================================================================================================
    //
    // Load / Save / Reset data.
    //
    //=======================================================================================================
    public virtual void LoadData()
    {
        volume = PlayerPrefs.GetFloat("Volume", 1f);
        sensitivity = PlayerPrefs.GetFloat("Sensibity", 0.5f);
    }

    public virtual void SaveData()
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("Sensitivity ", sensitivity);
    }

    public virtual void ResetCubiclandData()
    {
        volume = 1f;
        sensitivity = 1f;

        SoundManager.instance.SetOriginalFXVolume(0.7f);
        SoundManager.instance.SetOriginalMusicVolume(0.7f);

        UpdateVolume();
        PlayerPrefs.DeleteAll();
    }

    //=======================================================================================================


    protected virtual void Start()
    {
        if (reset)
        {
            ResetCubiclandData();
        }
        else
        {
            LoadData();
            UpdateVolume();
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    protected void OnDestroy()
    {
        SaveData();
    }
}
