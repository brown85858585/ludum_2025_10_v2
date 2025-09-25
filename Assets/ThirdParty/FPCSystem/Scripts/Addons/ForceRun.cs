
using UnityEngine;
using System.Collections;

public class ForceRun : MonoBehaviour
{
    public bool autoMove = false;
    public bool autoRun = true;

    private void Start()
    {
        InputManager.instance.SetForceForwardButton(autoMove);
        InputManager.instance.SetForceRunButton(autoRun);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            autoMove = !autoMove;
            InputManager.instance.SetForceForwardButton(autoMove);
        }
    }
}