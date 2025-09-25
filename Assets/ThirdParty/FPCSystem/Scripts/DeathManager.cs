using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum DeathResponseType
{
    DEATHPANEL = 0,
    RELOADLEVEL = 1,
    RESPAWN = 2,
    NONE = 3
}

public class DeathManager : MonoBehaviour {

    public DeathResponseType deathResponse = DeathResponseType.NONE;

    [ShowWhen("deathResponse", ShowWhenAttribute.Condition.Equals, 0)]
    public GameObject deathPanel;

    [ShowWhen("deathResponse", ShowWhenAttribute.Condition.Equals, 2)]
    public Spawner spawnSystem;

    [Space(10)]
    public float responseTime = 1f;

    protected GameObject MyObj;
    protected Status StatusSrc;


    public virtual void Start()
    {
        MyObj = GameObject.FindWithTag("Player");
        if (MyObj == null)
        {
            Debug.LogError("NOT Found!");
        }
        else
        {
            StatusSrc = MyObj.GetComponent<Status>();
        }
    }

    public virtual void OnEnable()
    {
        EventManagerv2.instance.StartListening("Death", PerformGameOver);
    }

    public virtual void OnDisable()
    {
        if (!EventManagerv2.IsDestroyed)
            EventManagerv2.instance.StopListening("Death", PerformGameOver);
    }

    public virtual void PerformGameOver (EventParam eventParam)
    {
        StartCoroutine("_PerformGameOverTimed");
	}

    protected virtual IEnumerator _PerformGameOverTimed ()
    {
        InputManager.instance.DisableInput();

        yield return new WaitForSeconds(responseTime);

        switch (deathResponse)
        {
            case DeathResponseType.DEATHPANEL:
                InputManager.instance.isActive = false;  // Deactivate the Input Managet so player can't move.
                deathPanel.SetActive(true); // Activate a panel (usually a Unity UI canvas gameobject).
                break;
            case DeathResponseType.RELOADLEVEL:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene again.
                break;
            case DeathResponseType.RESPAWN:
                MyObj.transform.position = spawnSystem.GetCurrentSpawnPoint().position;
                MyObj.transform.eulerAngles = new Vector3(0, 180, 0);
                StatusSrc.GetMouseLookSrc().SetRotationX(0);
                StatusSrc.health = StatusSrc.maxHealth;
                InputManager.instance.EnableInput();
                break;
            case DeathResponseType.NONE:
                break;
        }
    }
}
