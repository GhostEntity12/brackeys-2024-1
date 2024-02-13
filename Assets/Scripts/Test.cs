using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Test : MonoBehaviour
{
	[Range(0, 15)]
	public int index = 0;
	public SpriteAtlas atlas;
	SpriteRenderer spriteRenderer;
	private void Start()
	{
		spriteRenderer= GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		spriteRenderer.sprite = atlas.GetSprite("TileTest_" + index);
	}
}
