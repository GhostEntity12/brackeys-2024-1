using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
	[SerializeField] float lightBeamsRotateSpeed = 15f;
	[SerializeField] CanvasGroup lightBeamGroup;
	[SerializeField] Animator doorAnimator;
	[SerializeField] CanvasGroup fade;
	bool active;
	RectTransform lightBeamsTransform;
	Button doorButton;

	private void Start()
	{
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

	public void Setup(Animal a)
	{

	}

	public void Activate()
	{
		active = true;
		doorButton.enabled = false;
		doorAnimator.SetTrigger("Bounce");
		LeanTween.alphaCanvas(lightBeamGroup, 1f, 0.5f).setDelay(0.8f);
		LeanTween.alphaCanvas(fade, 1, 0.2f).setDelay(2.2f).setOnComplete(() => lightBeamsTransform.anchoredPosition = new(0, -341));

		//After flash
		LeanTween.alphaCanvas(fade, 0, 0.5f).setDelay(2.8f);
	}

	public void Reset()
	{
		active = false;
		doorButton.enabled = true;
		lightBeamGroup.alpha = 0;
		lightBeamsTransform.anchoredPosition = Vector3.zero;
		doorAnimator.SetTrigger("Reset");
	}
}
