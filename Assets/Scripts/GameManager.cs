using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
	public float GlobalFoodDecayModifier = 1;
	public float GlobalEntertainmentDecayModifier = 1;
	public float GlobalAttentionDecayModifier = 1;
	public float GlobalSleepDecayModifier = 1;
	public float GlobalBladderDecayModifier = 1;
	public float GlobalGroomingDecayModifier = 1;

	[SerializeField] private List<Bed> allBeds;
	[SerializeField] private List<FoodBowl> allFoodbowls;
	[SerializeField] private List<LitterTray> allLitterTrays;
	[SerializeField] private List<Toy> allToys;

	[SerializeField] NeedsWindow needsWindow;

	private Queue<Bed> AvailableBeds;
	private Queue<FoodBowl> AvailableFoodbowls;
	private Queue<LitterTray> AvailableLitterTrays;
	private Queue<Toy> AvailableToys;

	private Camera c;
	List<Animal> animals = new();

	//List<>

	private void Start()
	{
		Application.targetFrameRate = 100;

		AvailableBeds = new Queue<Bed>(allBeds);
		AvailableFoodbowls = new Queue<FoodBowl>(allFoodbowls);
		AvailableLitterTrays = new Queue<LitterTray>(allLitterTrays);
		AvailableToys = new Queue<Toy>(allToys);
		c = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << 6) &&
				hit.transform.TryGetComponent(out Animal a))
			{
				needsWindow.Show(a);
			}
			else if (!EventSystem.current.IsPointerOverGameObject())
			{
				needsWindow.Hide();
			}
		}
	}

	public void ShowWindow(Animal a) => needsWindow.Show(a);

	public bool TryGetFoodBowl(out FoodBowl foodBowl)
	{
		foodBowl = null;
		if (AvailableFoodbowls.Count == 0) return false;

		{
			foodBowl = AvailableFoodbowls.Dequeue();
			return true;
		}
	}

	public bool TryGetLitterTray(out LitterTray litterTray)
	{
		litterTray = null;
		if (AvailableLitterTrays.Count == 0) return false;
		else
		{
			litterTray = AvailableLitterTrays.Dequeue();
			return true;
		}
	}

	public bool TryGetBed(out Bed bed)
	{
		bed = null;
		if (AvailableBeds.Count == 0) return false;
		else
		{
			bed = AvailableBeds.Dequeue();
			return true;
		}
	}

	public bool TryGetToy(out Toy toy)
	{
		toy = null;
		if (AvailableBeds.Count == 0) return false;
		else
		{
			toy = AvailableToys.Dequeue();
			return true;
		}
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
				Debug.LogError("Unkown Equipment type");
				break;

		}
	}

	public void ChooseNewAnimal(AnimalInfo.Rarities? rarity = null)
	{
		// If no chosen rarity, choose
		if (rarity == null)
		{
			float roll = Random.value;
			rarity =
				roll < 0.01f ? AnimalInfo.Rarities.Legendary :
				roll < 0.11f ? AnimalInfo.Rarities.Legendary :
				roll < 0.31f ? AnimalInfo.Rarities.Rare :
				roll < 0.61f ? AnimalInfo.Rarities.Uncommon :
				AnimalInfo.Rarities.Common;
		}

		switch (rarity)
		{
			case AnimalInfo.Rarities.Common:
				break;
			case AnimalInfo.Rarities.Uncommon:
				break;
			case AnimalInfo.Rarities.Rare:
				break;
			case AnimalInfo.Rarities.Legendary:
				break;
			case AnimalInfo.Rarities.Mythic:
				break;
		}
	}
}
