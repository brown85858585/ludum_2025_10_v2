
using UnityEngine;
using System.Collections;

public class Pause : Singletone<Pause>
{
    [SerializeField]
    [Disable]
    private bool isPaused = false;

    private float TimeToScale = 0f;
    private float TimeScaleOriginal = 1f;
    private float waitTime = 0f;


    public bool IsPaused() { return isPaused; }

    // Execute Inmediate pause (with no delay)
    public void SetPause(bool _paused)
    {
        isPaused = _paused;

        if (isPaused && Time.timeScale != TimeToScale)
        {
            Time.timeScale = TimeToScale;
        }
        else if (!isPaused && Time.timeScale != TimeScaleOriginal)
        {
            Time.timeScale = TimeScaleOriginal;
        }
    }

    // Pause the game after waiting some time.
    public void EnablePauseDelayed(float _waitTime)
    {
        waitTime = _waitTime;
        StartCoroutine("_SetPauseTimed", true);
    }

    // Unpause the game after waiting some time.
    public void DisablePauseDelayed(float _waitTime)
    {
        waitTime = _waitTime;
        StartCoroutine("_SetPauseTimed", false);
    }

    void Start()
    {
        TimeScaleOriginal = Time.timeScale;
    }

    // Internal timed function to set pause on/off
    private IEnumerator _SetPauseTimed(bool _paused)
    {
        yield return new WaitForSeconds(waitTime);
        SetPause(_paused);
    }

}