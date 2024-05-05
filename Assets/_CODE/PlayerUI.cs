using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	[SerializeField]
	RectTransform HealthUI;

	private void OnEnable()
	{
		GetComponent<NetworkHealthState>().healthPoint.OnValueChanged += HealthChange;
	}

	private void OnDisable()
	{
		GetComponent<NetworkHealthState>().healthPoint.OnValueChanged -= HealthChange;
	}

	private void HealthChange( int previousValue, int newValue )
	{
		HealthUI.transform.localScale = new Vector3( newValue / 100f, 1, 1 );
	}
}