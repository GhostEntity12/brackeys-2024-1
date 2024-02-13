using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpritesheetAnimator : MonoBehaviour
{
	public int spriteIndex;
	private SpriteRenderer spriteRenderer;
	[SerializeField] SpriteAtlas atlas;
	// Start is called before the first frame update
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		spriteRenderer.sprite = atlas.GetSprite($"{atlas.name}_{spriteIndex}");
	}
}
