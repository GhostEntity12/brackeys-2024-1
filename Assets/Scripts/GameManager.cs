using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	public List<Animal> animals = new();
	public Phone phone;

	[SerializeField] Animator cardboardBox;

	[Header("Buttons")]
	[SerializeField] Button doorButton;


	[Header("Animals")]
	[SerializeField] Animal animalPrefab;
	[SerializeField] int maxAnimals;
	// Replace this with Serializables
	[SerializeField] List<SpriteAtlas> animalSprites;
	[SerializeField] List<AnimalInfo> animalInfos;

	[Space(20)]
	[SerializeField] Gacha gachaScreen;

	[Header("Global Modifiers")]
	public float GlobalFoodDecayModifier = 1;
	public float GlobalEntertainmentDecayModifier = 1;
	public float GlobalAttentionDecayModifier = 1;
	public float GlobalSleepDecayModifier = 1;
	public float GlobalBladderDecayModifier = 1;
	public float GlobalGroomingDecayModifier = 1;

	public static List<string> animalNames = new()
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
	public static List<string> humanNames = new()
	{
		"Ada",
		"Maria",
		"Jake",
		"Nick",
		"Roy",
		"Alvin",
		"Shay",
		"Taylor",
		"Billie",
		"Lisa",
		"Eric",
		"Elliot",
		"Michelle",
		"Casey",
		"Jenna",
		"Ash",
		"Natalie",
		"Shane",
		"Phillip",
		"Yuki",
		"Sam",
		"Lily",
		"Flynn",
		"Amy",
		"Levi",
		"Jackson",
		"Rose",
		"Aiden",
		"Amara",
		"Quinn"
	};
	public static List<string> requestMessages = new()
	{
		"Hi! I was looking for a pet who is {0}. Do you have any?",
		"You're a shelter, right? I want a {0} animal!",
		"Omg hi! You wouldn't happen to have a {0} pet there, would you?",
		"Hey. I was looking for an animal who is {0}! Please say you have one!",
		"Hai! I really want a pet! I would happily take one that is {0}!",
		"Mum says I can have a pet but it has to be {0}. Please?",
		"I want to get my kids a pet for Christmas! They'd love one that is {0}! Please send it soon!",
		"I need a {0} animal ASAP! Seriously!",
		"Hello there, I require one of your finest {0} animals. Thank you for your cooperation"
	};
	public static List<string> confirmationMessages = new()
	{
		"I have the perfect pet for you, just you wait!",
		"Absolutely! Sending one to you now!",
		"They are on their way! I can't wait for you to meet them!",
		"Take good care of them! They are on their way now!"
	};

	private Queue<Bed> availableBeds = new();
	private Queue<FoodBowl> availableFoodbowls = new();
	private Queue<LitterTray> availableLitterTrays = new();
	private Queue<Toy> availableToys = new();

	float nextAnimalTimer;
	bool doorButtonActive = false;

	public static System.Random rand = new();
	private AudioSource audioSource;

	protected override void Awake()
	{
		base.Awake();

		SaveData d = Save.Read();
		phone.requests = d.requests;
		foreach (AnimalInfo a in d.animals)
		{
			SpawnAnimal(a);
		}
	}

	private void Start()
	{
		Application.targetFrameRate = 100;
		audioSource = GetComponent<AudioSource>();

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

	public AnimalInfo ChooseNewAnimal() => new(animalNames[Random.Range(0, animalNames.Count)], animalSprites[Random.Range(0, animalSprites.Count)].name);

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
		nextAnimalTimer = (5 * Mathf.Pow(animals.Count, 1.1f) + Random.Range(100, 150));
	}

	public void ClearForAdoption(Animal a)
	{
		a.GetComponent<NavMeshAgent>().enabled = false;
		animals.Remove(a);
		cardboardBox.transform.parent = a.transform;
		cardboardBox.transform.localPosition = new Vector3(0f, 0.625f, -0.01f);
		cardboardBox.SetTrigger("BoxUp");
		LeanTween.scale(a.gameObject, Vector3.zero, 0.25f).setDelay(1f).setOnComplete(() =>
		{
			LeanTween.delayedCall(0.3f, () =>
			{
				cardboardBox.transform.SetParent(null);
				Destroy(a.gameObject);
			});
		});
	}

	public void PlayAudioClip(AudioClip ac, float vol = 1) => audioSource.PlayOneShot(ac, vol);

	public void SaveGame() => Save.Write();

}
