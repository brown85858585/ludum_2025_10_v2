using UnityEngine;

public class CursorUnlocker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}