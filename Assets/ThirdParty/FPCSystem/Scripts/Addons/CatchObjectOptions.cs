
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CatchObjectOptions : MonoBehaviour
{
	[Tooltip("Enable/Disable catching this object.")]
	public bool canBeCatched = true;
	[Tooltip("Enable/Disable dragging this object.")]
	public bool canBeDragged = true;
	[Tooltip("The force used to drag this object. Making it coming to us more or less quickly")]
	[ShowWhen("canBeDragged")]
	public float dragForce = 10f;
	[Tooltip("The force used to launch this object.")]
	public float launchForce = 25f;
}