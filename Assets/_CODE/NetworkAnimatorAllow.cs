using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class NetworkAnimatorAllow : NetworkAnimator
{
	protected override bool OnIsServerAuthoritative()
	{
		return false;
	}
}
