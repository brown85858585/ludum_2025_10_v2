using System.Collections;
using UnityEngine;


public class LevelCompleted : MonoBehaviour
{
    public GameObject endGamePanel;

    [Space(10)]
    public float responseTime = 1f;


    public virtual void OnEnable()
    {
        EventManagerv2.instance.StartListening("LevelCompleted", PerformLevelCompleted);
    }

    public virtual void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
            EventManagerv2.instance.StopListening("LevelCompleted", PerformLevelCompleted);
    }

    public virtual void PerformLevelCompleted(EventParam eventParam)
    {
        StartCoroutine("_PerformLevelCompletedTimed");
	}

    protected virtual IEnumerator _PerformLevelCompletedTimed()
    {
        InputManager.instance.DisableInput();

        yield return new WaitForSeconds(responseTime);

        InputManager.instance.isActive = false;  // Deactivate the Input Managet so pl can't move.
        EventManagerv2.instance.TriggerEvent("UpdateScreenCursor", new EventParam(this.name, string.Empty, "showcursor"));
        endGamePanel.SetActive(true);               // Activate a panel (usually a Unity UI canvas gameobject).
    }

}
