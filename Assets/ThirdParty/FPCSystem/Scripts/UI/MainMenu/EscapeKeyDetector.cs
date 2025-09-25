//
// This script is an example about creating a pause popup.
// It detects the escape key to activate/deactivate the pause state and sows the popup (or go to the main menu). 
// 
// Take into acount that it updates the Pause script used to detect when the game is paused or not (changing the TimeScale in the process).
// Some scripts like mouseMovement script (MouseLook) or the Movement script (CharacterMotor) look if Pause is paiused to do their things.
//
// So, this script (or any other) HAS to update the Pause state (isPaused var.) in order to work properly.
//
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapeKeyDetector : MonoBehaviour {

    public bool showPausePopup = true;
    [ShowWhen("showPausePopup")]
    public GameObject pausePopup;

    [Space (5)]
    public bool isPauseInmediate = true;
    [ShowWhen("isPauseInmediate", ShowWhenAttribute.Condition.Equals, false)]
    public float delayTime = 0.5f;

    [Header("Runtime Watcher")]
    [SerializeField]
    [Disable]
    private bool isPaused = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If we are in the Main Menu, just quit the Demo
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Application.Quit();
            }
            else    // Of we are playing, it can show a popup or go directly to the Main Menu.
            {
                if (!isPaused)
                {
                    if (showPausePopup)
                    {
                        pausePopup.SetActive(true);
                        SetPause(true);
                    }
                    else
                        LoadMainMenu();
                }
                /*else  // Don't allow to exit the pause popup using Esc key. The player must click on Continue to do it.
                {
                    if (showPausePopup)
                    {
                        ContinueDemo();
                    }
                }*/
            }
        }
    }

    public void ContinueDemo()
    {
        SetPause(false);
        pausePopup.SetActive(false);
    }

    public void LoadLevel(int sceneToLoad)
    {
        SetPause(false);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadMainMenu()
    {
        SetPause(false);
        LoadLevel (0);
    }

    public void ReloadCurrentScene()
    {
        SetPause(false);
        int sceneToLoad = SceneManager.GetActiveScene().buildIndex;
        LoadLevel (sceneToLoad);
    }

    private void SetPause(bool _pause)
    {
        isPaused = _pause;

        // Method 1 - Update the Pause script state inmediatly. VERY IMPORTANT or it won't work.
        if (isPauseInmediate)
        {
            Pause.instance.SetPause(isPaused);
        }
        else // Update the Pause script state using a Delay Time. VERY IMPORTANT or it won't work.
        {
            if (isPaused)
                Pause.instance.EnablePauseDelayed(0.5f);
            else
                Pause.instance.SetPause(false);
        }


        if (_pause)
        {
            InputManager.instance.DisableInput();
            EventManagerv2.instance.TriggerEvent("UpdateScreenCursor", new EventParam(this.name, string.Empty, "showcursor"));
        }
        else
        {
            InputManager.instance.EnableInput();
            EventManagerv2.instance.TriggerEvent("UpdateScreenCursor", new EventParam(this.name, string.Empty, "hidecursor"));
        }
    }

}
