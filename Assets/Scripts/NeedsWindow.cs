using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NeedsWindow : MonoBehaviour
{
	private Camera c;
	private bool active;
	private Animal animal;
	private RectTransform rectTransform;
	
	[SerializeField] private TextMeshProUGUI animalName;
	[SerializeField] private NeedBar foodBar, entertainmentBar, attentionBar, bladderBar, sleepBar, groomingBar;
	[SerializeField] private Button petButton, groomButton;

	private void Start()
	{
		c = Camera.main;
		rectTransform = transform as RectTransform;
	}

	// Update is called once per frame
	void Update()
	{
		if (animal)
		{
			rectTransform.position = c.WorldToScreenPoint(animal.transform.position + Vector3.up * 2);
			(float food, float entertainment, float attention, float sleep, float bladder, float grooming) = animal.GetStats();
			foodBar.FillAmount = food;
			entertainmentBar.FillAmount = entertainment;
			attentionBar.FillAmount = attention;
			sleepBar.FillAmount = sleep;
			bladderBar.FillAmount = bladder;
			groomingBar.FillAmount = grooming;
			petButton.interactable = animal.CanBePet;
			groomButton.interactable = animal.CanBeGroomed;
		}
	}

	public void Show(Animal a)
	{
		if (animal == a) return;

		if (active)
		{
			active = false;
			// Vanish the element and call this function recursively
			LeanTween.scaleY(gameObject, 0f, 0.3f).setEaseInBack().setOnComplete(a => Show(a as Animal));
		}
		else
		{
			active = true;
			animal = a;
			animalName.text = animal.name;
			LeanTween.scaleY(gameObject, 1f, 0.3f).setEaseOutBack();
		}
	}

	public void Hide()
	{
		Debug.Log("Hiding");
		active = false;
		LeanTween.scaleY(gameObject, 0f, 0.3f).setEaseInBack().setOnComplete(() => animal = null);
	}

	public void Pet() => animal.Pet();

	public void Groom() => animal.Groom();
}