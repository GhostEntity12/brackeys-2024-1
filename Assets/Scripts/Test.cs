using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Test : MonoBehaviour
{
	[Range(0f, 1f)]
	public float value = 0;
	NeedBar bar;
	private void Start()
	{
		bar = GetComponent<NeedBar>();
	}

	private void Update()
	{
		bar.FillAmount = value;
	}
}
