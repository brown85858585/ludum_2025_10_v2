using UnityEngine;
using System.Collections;

public class LadderOptions : MonoBehaviour
{
	public bool isLateralMovement = true;
	public float jumpSpeed = 8.0f;
	public Vector2 cameraRotationLimits = new Vector2(-360, 360);
}