using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class KeyboarduGUI : MonoBehaviour
{
    private bool isActive = false;


	void Update()
	{
		CheckInput();
	}

	// Player Status keyboard detector, to show the keyboard map.
	void CheckInput()
	{
		if (InputManager.instance.f5Key.isDown){
			isActive = !isActive;
			GetComponent<Image>().enabled = isActive;
		}
	}

}