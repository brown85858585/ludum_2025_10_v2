
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ObjectPushOptions : MonoBehaviour
{
	[Header("Push Override Options")]
	[Tooltip("Enable/Disable pushing this object.")]
	public bool canPush = true;				// This object can be pushed
	[Tooltip("Use a key to push the object or do it just walking towards it.")]
	[ShowWhen("canPush")]
	public bool useKey = true;				// Need to press a key to push this object
	[Tooltip("Override the force aplied when pushing.")]
	[ShowWhen("canPush")]
	public bool overridePushPower = false;
	[Tooltip("Force applied to the object when kicked.")]
	[ShowWhen("overridePushPower")]
	public Vector2 pushPower = new Vector2(6f, 0f);	// push force

	[Header("Kick Override Options")]
	[Tooltip("Enable/Disable kicking this object.")]
	public bool canKick = true;			// Can this object be kicked?
	[Tooltip("Override the force aplied when kicking.")]
	[ShowWhen("canKick")]
	public bool overrideKickPower = false;
	[Tooltip("Force applied to the object when kicked.")]
	[ShowWhen("overrideKickPower")]
	public Vector2 kickPower = new Vector2(10f, 0f);	// kick Force

	[Header("SideKick Override Options")]
	[Tooltip("Enable/Disable side kicking this object.")]
	public bool canSideKick = true;			// Can this object be kicked?
	[Tooltip("Override the force aplied when side kicking.")]
	[ShowWhen("canKick")]
	public bool overrideSideKickPower = false;
	[Tooltip("Force applied to the object when side kicked.")]
	[ShowWhen("overrideSideKickPower")]
	public Vector2 kickSidePower = new Vector2(10f, 0f);	// kick Force
}