using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour {

    public float timeToWait = 1f;


    private void Start()
    {
        LoadMainMenuScene();
    }

    private void LoadMainMenuScene()
    {
        LoadLevel(1);
    }

    private void LoadLevel(int sceneToLoad)
    {
        StartCoroutine("_LoadLevelTimed", sceneToLoad);
    }

    private IEnumerator _LoadLevelTimed(int sceneToLoad)
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(sceneToLoad);
    }

}
