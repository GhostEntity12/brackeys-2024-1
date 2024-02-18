using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdoptionButton : MonoBehaviour
{
	[SerializeField] Button button;
	[SerializeField] TextMeshProUGUI animalName;
	[SerializeField] Image thumbnail;

	public void Setup(Animal a, List<AnimalInfo.Attributes> requirements)
	{
		animalName.text = a.AnimalInfo.name;
		string atlasName = a.AnimalInfo.spriteAtlasName;
		thumbnail.sprite = GameManager.Instance.GetSpriteAtlasByName(atlasName).GetSprite($"Spr_{atlasName}_0");
		button.interactable = false;
		foreach (AnimalInfo.Attributes r in requirements)
		{
			if (a.AnimalInfo.attributes.Contains(r))
			{
				button.interactable = true;
				return;
			}
		}
	}
}
