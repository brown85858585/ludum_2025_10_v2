using UnityEngine;
using System.Collections;

public class DisableRender : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = false;
    }

}