using System.Collections.Generic;
using UnityEngine;

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

	private Queue<Bed> AvailableBeds;
	private Queue<FoodBowl> AvailableFoodbowls;
	private Queue<LitterTray> AvailableLitterTrays;
	private Queue<Toy> AvailableToys;

	private void Start()
	{
		Application.targetFrameRate = 100;

		AvailableBeds = new Queue<Bed>(allBeds);
		AvailableFoodbowls = new Queue<FoodBowl>(allFoodbowls);
		AvailableLitterTrays = new Queue<LitterTray>(allLitterTrays);
		AvailableToys = new Queue<Toy>(allToys);
	}

	// Update is called once per frame
	void Update()
	{

	}

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
}
