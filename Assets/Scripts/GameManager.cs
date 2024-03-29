using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	List<Animal> animals = new();

	[SerializeField] Animal animalPrefab;
	[SerializeField] int maxAnimals;

	[SerializeField] Gacha gachaScreen;
	[SerializeField] Button doorButton;

	// Replace this with Serializables
	[SerializeField] List<SpriteAtlas> animalSprites;
	[SerializeField] List<AnimalInfo> animalInfos;

	private Queue<Bed> availableBeds = new();
	private Queue<FoodBowl> availableFoodbowls = new();
	private Queue<LitterTray> availableLitterTrays = new();
	private Queue<Toy> availableToys = new();

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

	float nextAnimalTimer;
	bool doorButtonActive = false;

	private void Start()
	{
		Application.targetFrameRate = 100;

		// Make equipment available
		FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IEquipment>().ToList().ForEach(e => ReturnEquipment(e));

		nextAnimalTimer = animals.Count == 0 ? 5 : 5 * Mathf.Pow(animals.Count, 1.1f) + Random.Range(100, 150);

	}

	private void Update()
	{
		if (animals.Count < maxAnimals)
		{
			nextAnimalTimer -= Time.deltaTime;
			if (!doorButtonActive && nextAnimalTimer < 0)
			{
				doorButtonActive = true;
				doorButton.interactable = true;
				LeanTween.moveY(doorButton.gameObject, 40, 0.3f).setEaseOutBack();
			}
		}
	}

	public bool TryGetFoodBowl(out FoodBowl foodBowl)
	{
		foodBowl = null;
		if (availableFoodbowls.Count == 0) return false;

		foodBowl = availableFoodbowls.Dequeue();
		return true;

	}

	public bool TryGetLitterTray(out LitterTray litterTray)
	{
		litterTray = null;
		if (availableLitterTrays.Count == 0) return false;

		litterTray = availableLitterTrays.Dequeue();
		return true;
	}

	public bool TryGetBed(out Bed bed)
	{
		bed = null;
		if (availableBeds.Count == 0) return false;

		bed = availableBeds.Dequeue();
		return true;
	}

	public bool TryGetToy(out Toy toy)
	{
		toy = null;
		if (availableToys.Count == 0) return false;

		toy = availableToys.Dequeue();
		return true;
	}

	public void ReturnEquipment(IEquipment equipment)
	{
		switch (equipment)
		{
			case Bed b:
				availableBeds.Enqueue(b);
				break;
			case FoodBowl fb:
				if (fb.Usable)
					availableFoodbowls.Enqueue(fb);
				break;
			case LitterTray lt:
				if (lt.Usable)
					availableLitterTrays.Enqueue(lt);
				break;
			case Toy t:
				availableToys.Enqueue(t);
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

	public void SpawnAnimal(AnimalInfo info)
	{
		Animal a = Instantiate(animalPrefab);
		a.SetInfo(info);
		animals.Add(a);
		gachaScreen.onResetEvent -= SpawnAnimal;
	}

	public void OnClickDoorButton()
	{
		gachaScreen.Setup(ChooseNewAnimal());
		gachaScreen.onResetEvent += SpawnAnimal;
		LeanTween.moveY(doorButton.gameObject, -190, 0.3f).setEaseInBack();
		doorButtonActive = false;
		doorButton.interactable = false;
		nextAnimalTimer = (5 * Mathf.Pow(animals.Count, 1.1f) + Random.Range(1,5));
	}
}
