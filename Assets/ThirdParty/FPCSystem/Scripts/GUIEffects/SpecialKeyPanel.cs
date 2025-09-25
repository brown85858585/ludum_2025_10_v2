using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TweenerBasic;

public class SpecialKeyPanel : MonoBehaviour
{
	public bool isHelpActive = true;
	public float moveTime = 0.5f;
	public Transform keysPanel;
	public Transform keysAnchor;

	private Vector3 visiblePos;
	private Vector3 invisiblePos;
	[SerializeField]
	[Disable]
    private bool isPanelVisible = false;



	void Start ()
	{
		visiblePos = keysAnchor.localPosition;
		invisiblePos = keysPanel.localPosition;
	}

	void Update (){
		if (!isHelpActive) return;

		if(InputManager.instance.f1Key.isDown)
		{
			MovePanel ();
		}
	}

	public void MovePanel()
	{
		isPanelVisible = !isPanelVisible;
		SetPanelVisible(isPanelVisible);
	}

	private void SetPanelVisible (bool _visibility)
	{
		Vector3 _pos = _visibility ? visiblePos : invisiblePos;
		keysPanel.gameObject.AddTween(new TweenMoveLocal(_pos.x, _pos.y, _pos.z, moveTime));
	}
}