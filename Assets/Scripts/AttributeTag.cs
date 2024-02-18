using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributeTag : MonoBehaviour
{
	readonly Dictionary<AnimalInfo.Attributes, Color> attributeColors = new()
	{
		{AnimalInfo.Attributes.Hungry, new Color(0.97f, 0.68f, 0.27f)},
		{AnimalInfo.Attributes.Cuddly, new Color(0.98f, 0.55f, 0.85f)},
		{AnimalInfo.Attributes.Lonely, new Color(0.64f, 0.5f, 1f)},
		{AnimalInfo.Attributes.Sleepy, new Color(0.5f, 0.5f, 0.97f)},
		{AnimalInfo.Attributes.Messy, new Color(0.96f, 0.24f, 0.32f)},
		{AnimalInfo.Attributes.Clean, new Color(0.38f, 9.98f, 9.96f)},
		{AnimalInfo.Attributes.Playful,new Color(0.83f, 0.4f, 0.97f)},
		{AnimalInfo.Attributes.Active, new Color(0.45f, 0.99f, 0.46f)},
		{AnimalInfo.Attributes.Lazy, new Color(1f, 0.94f, 0.37f)},
	};

	[SerializeField] Image background;
	[SerializeField] TextMeshProUGUI label;

	public void Setup(AnimalInfo.Attributes attribute)
	{
		background.color = attributeColors[attribute];
		label.text = attribute.ToString();
	}
}
