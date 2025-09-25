using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public AudioClip musicClip;
    public bool updateVolume = false;

    public bool playOnStart = true;
    [ShowWhen("playOnStart")]
    public float waitTime = 0f;

    [SerializeField]
    [Disable]
    private float volume = 1;
    [SerializeField]
    [Disable]
    private bool enableMusic = false;

    private AudioSource musicAS;
	

	IEnumerator Start ()
    {
        musicAS = GetComponent<AudioSource>();

        // Create the source if the isn't a AudioSource
        if (musicAS == null)
        {
            musicAS = gameObject.AddComponent<AudioSource>();
        }

        volume = SoundManager.instance.musicVolume;

        yield return new WaitForSeconds(waitTime);
        PlayMusic(musicClip);

        if (updateVolume)
        {
            StartCoroutine("UpdateVolume");
        }
    }


    private IEnumerator UpdateVolume()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);
            if (volume != SoundManager.instance.musicVolume)
            {
                volume = SoundManager.instance.musicVolume;
                musicAS.volume = volume;
            }

            if (SoundManager.instance.enableSounds != enableMusic)
            {
                enableMusic = SoundManager.instance.enableSounds;
                if (enableMusic)
                    ContinuePlayingMusic();
                else
                    PauseMusic();
            }
        } 
    }

    // Plays a music clip. If it isn't any free, create one to play the effect and destroy it.
    public AudioSource PlayMusic(AudioClip _musicClip)
    {
        if (_musicClip == null) { return null; }

        musicAS.Stop();
        musicAS.clip = _musicClip;
        musicAS.volume = volume;
        musicAS.loop = true;
        musicAS.Play();

        return musicAS;
    }

    public void StopMusic()
    {
        musicAS.Stop();
    }

    public void PauseMusic()
    {
        musicAS.Pause();
    }

    public void ContinuePlayingMusic()
    {
        musicAS.Play();
    }
}
