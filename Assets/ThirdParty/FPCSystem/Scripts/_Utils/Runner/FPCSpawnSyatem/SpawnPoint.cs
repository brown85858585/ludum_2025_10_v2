using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool IsStartPoint = false;
    [ShowWhen("IsStartPoint", ShowWhenAttribute.Condition.Equals, false)]
    public bool IsEndPoint = false;

    [Header("Runtime Watcher")]
    [SerializeField]
    [Disable]
    private bool hasBeenActivated = false;  // Flag to know if this trigger has been activated (pl has entered/exited it).

    private MeshRenderer meshRenderer;

    private void Start ()
    {
        if (IsStartPoint) { IsEndPoint = false; }
        meshRenderer = GetComponent<MeshRenderer>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenActivated) return;
        if (!other.tag.Contains("Player")) return;

        hasBeenActivated = true;
        meshRenderer.enabled = false;
        EventManagerv2.instance.TriggerEvent("SpawnPointReached");
    }

    void OnTriggerStay(Collider other)
    {
        if (hasBeenActivated) return;
        if (!other.tag.Contains("Player")) return;

        hasBeenActivated = true;
        meshRenderer.enabled = false;
        EventManagerv2.instance.TriggerEvent("SpawnPointReached");
    }

}
