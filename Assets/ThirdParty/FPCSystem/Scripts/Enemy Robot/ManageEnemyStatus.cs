using UnityEngine;
using System.Collections;

public class ManageEnemyStatus : MonoBehaviour
{
    [Header("Enemy Status")]
    public int health = 100;

    [Header("Enemy Dying Splash FX")]
    public GameObject splashPrefab;

    [Header("Debug Control")]
    [Tooltip("Enable/Disable all 'Debug.Log' messages from this script.")]
    public bool showDebug = false;

    private GameObject enemyObject;
   
	void Start()
	{
		enemyObject = this.transform.parent.gameObject;	// get the hole Enemy Object so we can destroy it if he dies.
	}

	// Check emeny's health. If <= zero update enemy's variables (to die) and destroy it in X seconds
	private void KillEnemy()
    {
        if (showDebug) Debug.Log("ManageEnemyStatus -> KillEnemy() -> Enemy Die");

        if (splashPrefab)
            Instantiate(splashPrefab, transform.position, Quaternion.identity);

        SendMessage("Die", SendMessageOptions.DontRequireReceiver);
		Destroy(enemyObject, 5);
    }

    public void ApplyDamage(float damage)
    {
        health = (int) (health - damage);
        health = Mathf.Clamp(health, 0, int.MaxValue);
        if (showDebug) Debug.Log("ManageEnemyStatus -> ApplyDamage() -> Damage: "+ damage + " Enemy Health: "+ health);

        if (health > 0)
        {
            SendMessage("StartChaseStatus", SendMessageOptions.DontRequireReceiver);
        }
		else
		{
			KillEnemy();
		}
    }

}