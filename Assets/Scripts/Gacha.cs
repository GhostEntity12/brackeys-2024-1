using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
	[SerializeField] float lightBeamsRotateSpeed = 15f;
	[SerializeField] CanvasGroup lightBeamGroup;
	[SerializeField] Animator doorAnimator;
	[SerializeField] CanvasGroup fade;
	[SerializeField] CanvasGroup closeButton;
	[SerializeField] Image animal;
	[SerializeField] RectTransform info;

	AnimalInfo animalInfo;

	bool active;
	RectTransform lightBeamsTransform;
	Button doorButton;
	CanvasGroup canvas;

	public delegate void OnReset(AnimalInfo a);
	public OnReset onResetEvent;

	private void Start()
	{
		canvas = GetComponent<CanvasGroup>();
		lightBeamsTransform = lightBeamGroup.transform as RectTransform;
		doorButton = doorAnimator.GetComponent<Button>();
	}

	// Update is called once per frame
	void Update()
	{
		if (active)
		{
			lightBeamsTransform.Rotate(Vector3.forward, lightBeamsRotateSpeed * Time.deltaTime);
		}
	}

	public void Setup(AnimalInfo a)
	{
		animal.sprite = GameManager.Instance.GetSpriteAtlasByName(a.spriteAtlasName).GetSprite($"Spr_{a.spriteAtlasName}_0");
		canvas.blocksRaycasts = true;
		canvas.interactable = true;
		canvas.alpha = 1;
		animalInfo = a;
	}

	public void Activate()
	{
		active = true;
		doorButton.enabled = false;
		doorAnimator.SetTrigger("Bounce");
		LeanTween.alphaCanvas(lightBeamGroup, 1f, 0.5f).setDelay(0.55f);
		LeanTween.alphaCanvas(fade, 1, 0.2f).setDelay(2.2f).setOnComplete(() => lightBeamsTransform.anchoredPosition = new(0, -341));

		//After flash
		LeanTween.alphaCanvas(fade, 0, 0.5f).setDelay(2.8f);

		// Show Info
		LeanTween.moveX(info, 600, 0.4f).setDelay(3f).setOnComplete(() => closeButton.interactable = closeButton.blocksRaycasts = true);
	}

	public void Reset()
	{
		active = false;
		canvas.blocksRaycasts = false;
		canvas.interactable = false;
		closeButton.interactable = false;
		closeButton.blocksRaycasts = false;
		canvas.alpha = 0;
		doorButton.enabled = true;
		lightBeamGroup.alpha = 0;
		lightBeamsTransform.anchoredPosition = Vector3.zero;
		doorAnimator.SetTrigger("Reset");
		info.anchoredPosition = new(1240, -225);
		onResetEvent.Invoke(animalInfo);
	}
}
