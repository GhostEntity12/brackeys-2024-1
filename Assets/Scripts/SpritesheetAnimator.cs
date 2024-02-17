using UnityEngine;
using UnityEngine.U2D;

public class SpritesheetAnimator : MonoBehaviour
{
	public int spriteIndex;
	private SpriteRenderer spriteRenderer;
	[SerializeField] SpriteAtlas atlas;
	int indexCache;
	// Start is called before the first frame update
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = atlas.GetSprite($"Spr_{atlas.name}_0");
	}

	// Update is called once per frame
	void Update()
	{
		// Only update the sprite if the value has changed
		if (indexCache != spriteIndex)
		{
			indexCache = spriteIndex;
			spriteRenderer.sprite = atlas.GetSprite($"Spr_{atlas.name}_{spriteIndex}");
		}
	}

	public void SetSpritesheet(string name) => atlas = GameManager.Instance.GetSpriteAtlasByName(name);
}
