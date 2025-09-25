using UnityEngine;
using System.Collections;

public class DisableAtStart : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        AudioSource UnderWaterAtm = (AudioSource) GetComponentInChildren(typeof(AudioSource));
        UnderWaterAtm.Stop();
    }

}