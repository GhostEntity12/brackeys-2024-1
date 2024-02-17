using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class AnimalInfo
{
	public enum Attributes { Hungry = 1, Cuddly, Lonely, Sleepy, Messy, Clean, Playful, Active, Lazy }
	public enum Rarities { Common, Uncommon, Rare, Legendary, Mythic }

	public string name;
	public string spriteAtlasName;
	public List<Attributes> attributes = new();

	// Modifiers
	public float foodMod = 1;
	public float entertainmentMod = 1;
	public float attentionMod = 1;
	public float sleepMod = 1;
	public float bladderMod = 1;
	public float groomingMod = 1;

	// Values
	public float speed;
	public Vector2 timeBetweenMovements = new(3, 8);

	public float food;
	public float entertainment;
	public float attention;
	public float sleep;
	public float bladder;
	public float grooming;

	public AnimalInfo(string name, string spriteAtlas)
	{
		this.name = name;
		spriteAtlasName = spriteAtlas;

		int attributeCount = Random.value switch
		{
			< 0.1f => 3,
			< 0.3f => 2,
			_ => 1
		};

		// Generate attributes
		int[] attributeIndexes = new int[attributeCount];
		int numberOfAttributes = System.Enum.GetNames(typeof(Attributes)).Length;
		for (int i = 0; i < attributeCount; i++)
		{
			int index;
			do
			{
				index = Random.Range(1, numberOfAttributes + 1);
			} while (attributeIndexes.Contains(index));
			attributeIndexes[i] = index;
			attributes.Add((Attributes)index);
		}

		// Apply attributes
		foreach (Attributes attribute in attributes)
		{
			switch (attribute)
			{
				case Attributes.Hungry:
					foodMod *= 1.2f;
					break;
				case Attributes.Cuddly:
					attentionMod *= 1.2f;
					break;
				case Attributes.Lonely:
					attentionMod *= 0.8f;
					break;
				case Attributes.Sleepy:
					sleepMod *= 1.2f;
					break;
				case Attributes.Messy:
					groomingMod *= 1.2f;
					break;
				case Attributes.Clean:
					groomingMod *= 0.8f;
					break;
				case Attributes.Playful:
					entertainmentMod *= 1.2f;
					break;
				case Attributes.Active:
					speed *= 1.3f;
					timeBetweenMovements -= Vector2.one * 2;
					break;
				case Attributes.Lazy:
					speed *= 0.8f;
					timeBetweenMovements += Vector2.one * 2;
					break;
				default:
					break;
			}
		}

		// Initialize needs
		food = 0.25f + (Random.value * 0.5f);
		entertainment = 0.25f + (Random.value * 0.5f);
		attention = 0.25f + (Random.value * 0.5f);
		sleep = 0.25f + (Random.value * 0.5f);
		bladder = 0.25f + (Random.value * 0.5f);
		grooming = 0.25f + (Random.value * 0.5f);
	}

	public string ToJson()
	{
		return JsonUtility.ToJson(this);
	}
}
