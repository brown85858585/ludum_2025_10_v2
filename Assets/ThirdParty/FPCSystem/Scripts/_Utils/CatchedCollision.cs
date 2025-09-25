using UnityEngine;
using System.Collections;

public class CatchedCollision : MonoBehaviour
{
	public bool isCatched = true;

    void OnCollisionEnter(Collision collision)
    {
        if (isCatched)
        {
            transform.root.SendMessage("DropCachedObject");
            isCatched = false;
        }
    }

}