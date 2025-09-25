// Copy the tranform og our player to be used in this object
// It will be used in the water Splash when the player gets the water surface (from diving to swimming)

using UnityEngine;


public class CopyTransform : MonoBehaviour {

    [SerializeField]
    [Disable]
	private Transform from;
	private Transform to;
	public Vector3 offset = Vector3.zero;

    private GameObject MyObj;
    private Vector3 Pos;


    private void SearForPlayer()
    {
        if (MyObj == null)
            MyObj = GameObject.FindWithTag("Player");
        if (MyObj != null)
            from = MyObj.transform;
    }

	void Start()
    {
        SearForPlayer();
        to = this.transform;
	}

    private void OnEnable()
    {
        SearForPlayer();
    }

    void LateUpdate ()
    {
		Pos = from.position + offset;
		if(Pos != to.position)
			to.position = Pos;
	}

}
