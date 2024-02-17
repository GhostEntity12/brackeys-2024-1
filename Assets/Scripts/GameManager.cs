using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class GameManager : Singleton<GameManager>
{
	List<Animal> animals = new();

	// Replace this with Serializables
	[SerializeField] List<SpriteAtlas> animalSprites;
	[SerializeField] List<AnimalInfo> animalInfos;

	private Queue<Bed> AvailableBeds;
	private Queue<FoodBowl> AvailableFoodbowls;
	private Queue<LitterTray> AvailableLitterTrays;
	private Queue<Toy> AvailableToys;

	[Header("Global Modifiers")]
	public float GlobalFoodDecayModifier = 1;
	public float GlobalEntertainmentDecayModifier = 1;
	public float GlobalAttentionDecayModifier = 1;
	public float GlobalSleepDecayModifier = 1;
	public float GlobalBladderDecayModifier = 1;
	public float GlobalGroomingDecayModifier = 1;

	public readonly List<string> names = new()
	{
		"Fido",
		"Oreo",
		"Cookie",
		"Fluffy",
		"Patches",
		"Leo",
		"Goldie",
		"Raymond",
		"Kirby",
		"Princess",
		"Doggo",
		"Benji",
		"Eevee",
		"Lady",
		"Floof",
		"Luna",
		"Max",
		"Milo",
		"Lola",
		"Artemis",
		"Freya",
		"Angel",
		"Ghost",
		"Marshmallow",
		"Snowy",
		"Sunny",
		"Casper",
		"Daisy",
		"Jelly Bean",
		"Chibi",
		"Chocolate",
		"Mocha",
		"Midnight",
		"Nibbles",
		"Charlie",
		"Pickle",
		"Fifi",
		"Snoodle",
		"Bella",
		"Monty",
		"Alfie",
		"Ralph",
		"Penny",
		"Rosie",
		"Rocky",
		"Nala",
		"Wraccoon",
		"Buddy",
		"Bear",
		"Duke",
		"Thor",
		"Brock"
	};

	private void Start()
	{
		Application.targetFrameRate = 100;

		// Make equipment available
		FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEquipment>().ToList().ForEach(e => ReturnEquipment(e));
	}

	public bool TryGetFoodBowl(out FoodBowl foodBowl)
	{
		foodBowl = null;
		if (AvailableFoodbowls.Count == 0) return false;

		foodBowl = AvailableFoodbowls.Dequeue();
		return true;

	}

	public bool TryGetLitterTray(out LitterTray litterTray)
	{
		litterTray = null;
		if (AvailableLitterTrays.Count == 0) return false;

		litterTray = AvailableLitterTrays.Dequeue();
		return true;
	}

	public bool TryGetBed(out Bed bed)
	{
		bed = null;
		if (AvailableBeds.Count == 0) return false;

		bed = AvailableBeds.Dequeue();
		return true;
	}

	public bool TryGetToy(out Toy toy)
	{
		toy = null;
		if (AvailableBeds.Count == 0) return false;
		
		toy = AvailableToys.Dequeue();
		return true;
	}

	public void ReturnEquipment(IEquipment equipment)
	{
		switch (equipment)
		{
			case Bed b:
				AvailableBeds.Enqueue(b);
				break;
			case FoodBowl fb:
				if (fb.Usable)
					AvailableFoodbowls.Enqueue(fb);
				break;
			case LitterTray lt:
				if (lt.Usable)
					AvailableLitterTrays.Enqueue(lt);
				break;
			case Toy t:
				AvailableToys.Enqueue(t);
				break;
			default:
				Debug.LogError("Unknown Equipment type");
				break;

		}
	}

	//public void ChooseNewAnimal(AnimalInfo.Rarities? rarity = null)
	//{
	//	// If no chosen rarity, choose
	//	if (rarity == null)
	//	{
	//		float roll = Random.value;
	//		rarity =
	//			roll < 0.01f ? AnimalInfo.Rarities.Mythic :
	//			roll < 0.11f ? AnimalInfo.Rarities.Legendary :
	//			roll < 0.31f ? AnimalInfo.Rarities.Rare :
	//			roll < 0.61f ? AnimalInfo.Rarities.Uncommon :
	//			AnimalInfo.Rarities.Common;
	//	}

	//	switch (rarity)
	//	{
	//		case AnimalInfo.Rarities.Common:
	//			break;
	//		case AnimalInfo.Rarities.Uncommon:
	//			break;
	//		case AnimalInfo.Rarities.Rare:
	//			break;
	//		case AnimalInfo.Rarities.Legendary:
	//			break;
	//		case AnimalInfo.Rarities.Mythic:
	//			break;
	//	}
	//}

	public AnimalInfo ChooseNewAnimal()
	{
		return new AnimalInfo(names[Random.Range(0, names.Count)], animalSprites[Random.Range(0, animalSprites.Count)].name);
	}

	public SpriteAtlas GetSpriteAtlasByName(string name)
	{
		foreach (SpriteAtlas atlas in animalSprites)
		{
			if (atlas.name == name) return atlas;
		}
		return null;
	}

	[ContextMenu("JsonTest")]
	public void TestJson()
	{
		Debug.Log(ChooseNewAnimal().ToJson());
	}
}
