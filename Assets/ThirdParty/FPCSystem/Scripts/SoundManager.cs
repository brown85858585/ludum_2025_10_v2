
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : PersistentSingletone<SoundManager>
{
    public bool enableSounds = true;
    public bool useVolumeUpdate = false;

    [Space(5)]
    [Range(0.1f, 1f)] public float masterVolume = 1.0f;
    [Range(0.1f, 1f)] public float effectsVolume = 0.7f;
	[Range(0.1f, 1f)] public float musicVolume = 0.7f;
    //[Range(0.1f, 1f)] public float underwaterVolume = 0.4f;

    protected float masterVolumePrev = 0f;
    protected float effectsVolumeOrig = 0f;
    protected float musicVolumeOrig = 0f;
    //private float underwaterVolumeOrig = 0f;

    protected AudioSource[] audioSources;

    protected AudioSource[] effectsAS;
    protected Vector3[] effectsTranforms;
    protected AudioSource musicAS;
    protected AudioSource underwaterAtmAS;

    protected int effectsASCount;
    protected bool isSoundActive;
    protected int currentAS;
    protected bool stopMonitoringVolumeChanges; // used to allow to change manually the volume in Fly Mode.

    protected bool isInitialized = false;

    protected SoundPlayer SoundPlayer;



    public virtual float GetOriginalFXVolume() { return effectsVolumeOrig; }
    public virtual void SetOriginalFXVolume(float _newFXVol) { effectsVolumeOrig = _newFXVol; }
    public virtual float GetOriginalMusicVolume() { return musicVolumeOrig; }
    public virtual void SetOriginalMusicVolume(float _newMusicVol) { musicVolumeOrig = _newMusicVol; }

    public virtual SoundPlayer GetSoundPlayer(){ return SoundPlayer; }

	public virtual void PlayUnderwaterAtmosphere(){ underwaterAtmAS.Play(); }

	public virtual void StopUnderwaterAtmosphere(){ underwaterAtmAS.Stop(); }

	public virtual void SetActive(bool _active){ enableSounds = _active; }	 // Public functions to enable/disable sounds in soundmanager.
    
	public virtual bool IsActive(){ return enableSounds; }		// Public functions that tell us if sounds are or not enabled.
		
    public virtual void DisableMasterVolumeInpector(bool _disable){ stopMonitoringVolumeChanges = _disable; }

    public virtual bool IsAnyEffectBeingPlayed()
    {
        bool nResult = false;
        int i = 0;
        while (i < effectsAS.Length)
        {
            if (effectsAS[i].isPlaying)
            {
                nResult = true;
                break;
            }
            i++;
        }
        return nResult;
    }

    //=======================================================================================================
    //
    // Load / Save / Reset data.
    //
    //=======================================================================================================
    public virtual void LoadSoundMngrData()
    {
        effectsVolumeOrig = PlayerPrefs.GetFloat("FXVolume", 0.7f);
        musicVolumeOrig = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        //Debug.Log("Loading: "+effectsVolumeOrig + " , " + musicVolumeOrig);
    }

    public virtual void SaveSoundMngrData()
    {
        PlayerPrefs.SetFloat("FXVolume", effectsVolumeOrig);
        PlayerPrefs.SetFloat("MusicVolume", effectsVolumeOrig);
        //Debug.Log("Saving: " + effectsVolumeOrig + " , " + musicVolumeOrig);
    }
    //=======================================================================================================

    public virtual void Start()
    {
        Init();
    }

    protected virtual void OnEnable()
    { 
        Init();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    protected virtual void InitializeVolume()
    {
        if(!isInitialized)
        {
            LoadSoundMngrData();
            effectsVolumeOrig = effectsVolume;
            musicVolumeOrig = musicVolume;
            //underwaterVolumeOrig = underwaterVolume;
            isInitialized = true;
        }

        masterVolume = AppManager.instance.volume;
        UpdateAgainstMasterVolume();
    }

    public virtual void UpdateAgainstMasterVolume()
    {
        effectsVolume = effectsVolumeOrig * masterVolume;
        effectsVolume = Mathf.Clamp01(effectsVolume);
        musicVolume = musicVolumeOrig * masterVolume;
        musicVolume = Mathf.Clamp01(musicVolume);

        //underwaterVolume = underwaterVolumeOrig * masterVolume;
    }

    protected virtual void Init()
    {
        InitializeVolume();
        masterVolumePrev = masterVolume;

        SoundPlayer = FindObjectOfType<SoundPlayer>();

        if (SoundPlayer == null) return; // Sentencia de seguridad

		audioSources = SoundPlayer.gameObject.GetComponentsInChildren<AudioSource>();
        isSoundActive = false;

        int i = 0;
        while (i < audioSources.Length)
        {
            if (audioSources[i].name.Contains("Effect"))
                effectsASCount++;
            i++;
        }

        if (effectsAS == null)
        {
            effectsAS = new AudioSource[effectsASCount];
            effectsTranforms = new Vector3[effectsASCount];
        }

        // We get our AudioSources and disable it at start.
        // If sound is active, they will be enabled in Update
        i = 0;
        int j = 0;
        while (i < audioSources.Length)
        {
            if (audioSources[i].name.Contains("Effect"))
            {
                effectsAS[j] = audioSources[i];
                effectsAS[j].volume = effectsVolume;
                effectsAS[j].enabled = false;
                effectsTranforms[j++] = audioSources[i].transform.position;
            }
            else  if (audioSources[i].name.Contains("Music"))
            {
                musicAS = audioSources[i];
                musicAS.volume = musicVolume;
            }
            else if (audioSources[i].name.Contains("Atmosphere"))
            {
                underwaterAtmAS = audioSources[i];
                underwaterAtmAS.volume = effectsVolume;
                underwaterAtmAS.gameObject.SetActive(false);
            }

            i++;
        }
    }


    protected virtual void Update()
    {
        // Check changes in master volume in the inspector.!
        if (useVolumeUpdate && masterVolumePrev != masterVolume)
        {
            UpdateAgainstMasterVolume();
            masterVolumePrev = masterVolume;
        }

        if (SoundPlayer == null) return; // Sentencia de seguridad

        if (enableSounds && !isSoundActive)
            EnableSounds(true);
        else if (!enableSounds && isSoundActive)
            EnableSounds(false);

        // Update volume changes in runtime.
        // it will do it if the sound is enabled and monitoring volume flag is active.
        // the Monitoring flag will be disabled in fly mode.
        if (!enableSounds || stopMonitoringVolumeChanges)
            ChangeVolume();
    }

    public virtual void ChangeVolume()
    {
        if (SoundPlayer == null) return; // Sentencia de seguridad

        if (musicAS.volume != musicVolume)
            musicAS.volume = musicVolume;

        /*if (underwaterAtmAS.volume != underwaterVolume)
            underwaterAtmAS.volume = underwaterVolume;*/

        int i = 0;
        while (i < effectsAS.Length)
        {
            if (effectsAS[i].volume != effectsVolume)
                effectsAS[i].volume = effectsVolume;
            i++;
        }
    }

    // Debug purposes to know how many AudioSources are playing at once.
    public virtual int CountPlayingAS()
    {
        if (SoundPlayer == null) return -1; // Sentencia de seguridad

        int count = 0;
        int i = 0;
        while (i < effectsAS.Length)
        {
            if (effectsAS[i].isPlaying)
                count++;
            i++;
        }
        return count;
    }

    public virtual void EnableSounds(bool _active)
    {
        if (SoundPlayer == null) return; // Sentencia de seguridad

        int i = 0;
        while (i < effectsAS.Length)
        {
            if (effectsAS[i].isPlaying)
                effectsAS[i].Stop();
            effectsAS[i].enabled = _active;
            i++;
        }

        if (_active)
        {
            musicAS.Play();
            underwaterAtmAS.gameObject.SetActive(true);
        }
        else
        {
            musicAS.Stop();
            underwaterAtmAS.gameObject.SetActive(false);
        }
        isSoundActive = _active;
    }

    public virtual void StopPlayingEffects()
    {
        if (SoundPlayer == null) return; // Sentencia de seguridad

        int i = 0;
        while (i < effectsAS.Length)
        {
            effectsAS[i].Stop();
            i++;
        }
    }

    public virtual AudioSource FindFreeAS()
    {
        if (SoundPlayer == null) return null; // Sentencia de seguridad
        if (!enableSounds) { return null; }

        // First way to play effects : just select the next audiosource and use it. NOT USED.
        /*currentAS++;
		if(currentAS == effectsAS.Length) currentAS = 0;
		return effectsAS[currentAS];*/

        //// Second way to play effects : search an inactive audiosource and use it.
        int i = 0;
        while (i < effectsAS.Length)
        {
            if (!effectsAS[i].isPlaying)
                return effectsAS[i];
            i++;
        }

		return null;
    }

    // Force playing an effect creating an audiosource in runtime.
    public virtual AudioSource ForceAS()
    {
        if (SoundPlayer == null) return null; // Sentencia de seguridad
        if (!enableSounds) { return null; }

        //Create an empty game object
        GameObject go = new GameObject("Effect_Forced");
        go.transform.position = transform.position;
        go.transform.parent = transform;

        //Create the source
        AudioSource source = (AudioSource) go.AddComponent(typeof(AudioSource));
        source.volume = effectsVolume;
        return source;
    }

    // Look for the closest 'not playing' AS to a given point. NOT USED.
    public virtual AudioSource FindCosestAS(Vector3 _pos)
    {
        if (SoundPlayer == null) return null; // Sentencia de seguridad

        if (isSoundActive)
        {
            float MinDist = 10000;
            AudioSource Result = effectsAS[0];
            int i = 0;
            while (i < effectsAS.Length)
            {
                if (!effectsAS[i].isPlaying)
                {
                    float ActualDist = Vector3.Distance(_pos, effectsTranforms[i]);
                    if (ActualDist < MinDist)
                    {
                        Result = effectsAS[i];
                        MinDist = ActualDist;
                    }
                }
                i++;
            }

            //Debug.Log("SoundMngr. AudioSource chosen: "+Result.name);
            return Result;
        }
        else
            return null;
    }

}