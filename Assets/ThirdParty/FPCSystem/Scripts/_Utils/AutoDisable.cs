
using UnityEngine;
using System.Collections;


public class AutoDisable : MonoBehaviour {

    public float disableTime = 1f;

    void OnEnable()
    {
        StartCoroutine("_CheckIfAlive");
    }

    IEnumerator _CheckIfAlive()
    {
        yield return new WaitForSeconds(disableTime);
        this.gameObject.SetActive(false);
        //Debug.Log (this.name + ": has been despawned!");
    }

}
