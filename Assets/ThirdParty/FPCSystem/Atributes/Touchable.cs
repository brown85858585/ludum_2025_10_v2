// file Touchable.cs
// Correctly backfills the missing Touchable concept in Unity.UI's OO chain.

using UnityEngine;
using UnityEngine.UI;

public class Touchable:Text
{ protected override void Awake() { base.Awake(); } }