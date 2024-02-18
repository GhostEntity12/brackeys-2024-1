using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdoptionButton : MonoBehaviour
{
	[SerializeField] Button button;
	[SerializeField] TextMeshProUGUI animalName;
	[SerializeField] Image thumbnail;
	[SerializeField] Animal animal;
	public UnityAction onButtonPress;

	public void Setup(Animal a, List<AnimalInfo.Attributes> requirements, UnityAction action)
	{
		animal = a;
		animalName.text = animal.AnimalInfo.name;
		string atlasName = animal.AnimalInfo.spriteAtlasName;
		thumbnail.sprite = GameManager.Instance.GetSpriteAtlasByName(atlasName).GetSprite($"Spr_{atlasName}_0");
		button.interactable = false;
		foreach (AnimalInfo.Attributes r in requirements)
		{
			if (animal.AnimalInfo.attributes.Contains(r))
			{
				button.interactable = true;
				return;
			}
		}
		onButtonPress = action;
	}
}
