using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    public float speedDown = 2;
    public float speedUp = 2;
    public Transform DestPosition;

    private Transform myTransform;
	private bool isGoingDown = true;
    private bool HasMoved;
    private Vector3 OriginalPosition;
    private bool IsOver;
    
	private GameObject MyObj;
	private Status statusSrc;


	private void GetPlayer(Collider other)
	{
		if(other.transform.tag.Contains("Player"))
		{
			MyObj = other.gameObject;
			statusSrc = MyObj.GetComponent<Status>();
		}
		else
		{
			MyObj = null;
			statusSrc = null;
		}
	}

    void Start()
    {
        myTransform = transform.parent;
		//MyObj = GameObject.Find("Player");
        OriginalPosition = myTransform.position;

		isGoingDown = (DestPosition.position.y < OriginalPosition.y);
        /*if (DestPosition.position.y < OriginalPosition.y)
            isGoingDown = true;
        else
            isGoingDown = false;*/
    }

    // Return the platform to its original position.
    void Update()
    {
        if (HasMoved && !IsOver)
        {
            if (isGoingDown)
            {
                if (myTransform.position.y < OriginalPosition.y)
                    myTransform.Translate((Vector3.up * speedUp) * Time.deltaTime, Space.World);
                else if (myTransform.position.y > OriginalPosition.y)
                {
					Vector3 posAux = myTransform.position;
					myTransform.position = new Vector3(posAux.x, OriginalPosition.y, posAux.z);
                    HasMoved = false;
                }
            }
            else
            {
                if (myTransform.position.y > OriginalPosition.y)
                    myTransform.Translate((-Vector3.up * speedDown) * Time.deltaTime, Space.World);
                else if (myTransform.position.y < OriginalPosition.y)
                {
					Vector3 posAux = myTransform.position;
					myTransform.position = new Vector3(posAux.x, OriginalPosition.y, posAux.z);
                    HasMoved = false;
                }
            }
        }
    }

    // Move the platform to the dest position.
    void OnTriggerStay(Collider other)
    {
        //  If pl doesn't touch the platform itself, do nothing.
		if (statusSrc != null && !statusSrc.isOverPlatform)  { return; }

		if (other.transform.tag.Contains("Player"))
        {
			if(MyObj == null) 
				GetPlayer (other);
			
            HasMoved = true;
            IsOver = true;
			statusSrc.GetSoundPlayerSrc().SetPlatformSound(tag.ToLower());

            if (isGoingDown)
            {
                if (myTransform.position.y > DestPosition.position.y)
                {
                    myTransform.Translate((-Vector3.up * speedDown) * Time.deltaTime, Space.World);
                }
                else if (myTransform.position.y < DestPosition.position.y)
                {
					Vector3 posAux = myTransform.position;
					myTransform.position = new Vector3(posAux.x, DestPosition.position.y, posAux.z);
                }
            }
            else
            {
                if (myTransform.position.y < DestPosition.position.y)
                {
                    myTransform.Translate((Vector3.up * speedUp) * Time.deltaTime, Space.World);
                }
                else if (myTransform.position.y > DestPosition.position.y)
                {
					Vector3 posAux = myTransform.position;
					myTransform.position = new Vector3(posAux.x, DestPosition.position.y, posAux.z);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
		if (other.transform.tag.Contains("Player"))
        {
			GetPlayer (other);
            IsOver = false;
            statusSrc.isOverPlatform = false;
        }
    }

}