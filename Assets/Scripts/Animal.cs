using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
	enum Behaviour { Idle, Eating, Playing, Sleeping, SleepingInPlace, Pooping, PoopingInPlace }

	private NavMeshAgent agent;
	private float idleTimer;
	private SpriteRenderer spriteRenderer;
	private Behaviour behaviour = Behaviour.Idle;

	float baseValue;

	private float foodDecayRate = 0.001f;
	private float entertainmentDecayRate = 0.001f;
	private float attentionDecayRate = 0.001f;
	private float sleepDecayRate = 0.001f;
	private float bladderDecayRate = 0.001f;
	private float groomingDecayRate = 0.001f;

	// Needs
	[Range(0f, 1f)]
	[SerializeField] private float food;
	[Range(0f, 1f)]
	[SerializeField] private float entertainment;
	[Range(0f, 1f)]
	[SerializeField] private float attention;
	[Range(0f, 1f)]
	[SerializeField] private float sleep;
	[Range(0f, 1f)]
	[SerializeField] private float bladder;
	[Range(0f, 1f)]
	[SerializeField] private float grooming;

	[SerializeField] private Vector2 timeBetweenMovements = new(2, 8);
	[SerializeField] private float wanderRadius = 3;

	IEquipment equipmentInUse = null;

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

		// Flip V2 if values are in wrong order
		if (timeBetweenMovements.x > timeBetweenMovements.y)
		{
			(timeBetweenMovements.y, timeBetweenMovements.x) = (timeBetweenMovements.x, timeBetweenMovements.y);
		}
		idleTimer = Random.Range(timeBetweenMovements.x, timeBetweenMovements.y);

		// Initialize needs
		food = Random.value;
		entertainment = Random.value;
		attention = Random.value;
		sleep = Random.value;
		bladder = Random.value;
		grooming = Random.value;
	}

	// Update is called once per frame
	void Update()
	{
		switch (behaviour)
		{
			case Behaviour.Idle:
				Idle();
				break;
			case Behaviour.Eating:
				break;
			case Behaviour.Playing:
				break;
			case Behaviour.Sleeping:
				break;
			case Behaviour.Pooping:
				break;
			case Behaviour.SleepingInPlace:
				break;
			case Behaviour.PoopingInPlace:
				break;
			default:
				break;
		}
		DecrementNeeds();
	}

	void Idle()
	{
		// If not at destination, return
		if (Vector3.Distance(agent.destination, transform.position) > 0.1f) return;

		// Decrease idle timer. Return while above 0
		idleTimer -= Time.deltaTime;
		if (idleTimer > 0) return;

		// When the idle timer reaches 0, choose what to do.
		ChooseBehaviour();

	}

	void Poop()
	{
		if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
		{
			equipmentInUse.Use(this);
			// Use Litter Tray
		}
	}

	void Eat()
	{
		if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
		{
			equipmentInUse.Use();
			// Use Litter Tray
		}
	}

	void Play()
	{
		if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
		{
			// Use Litter Tray
		}
	}

	void Sleep()
	{
		if (Vector3.Distance(transform.position, agent.destination) < 0.1f)
		{
			// Use Litter Tray
		}
	}

	Behaviour ChooseBehaviour()
	{
		float foodCheck = food > 0.5f ? 0 : Mathf.Lerp(1f, 0.3f, food * 2);
		if (Random.value < foodCheck && GameManager.Instance.TryGetFoodBowl(out FoodBowl bowl))
		{
			equipmentInUse = bowl;
			agent.destination = equipmentInUse.InteractLocation.position;
			return Behaviour.Eating;
		}

		if (bladder > 0)
		{
			float bladderCheck = bladder > 0.6f ? 0 : Mathf.Lerp(1f, 0.2f, bladder * 1.67f);
			if (Random.value < bladderCheck && GameManager.Instance.TryGetLitterTray(out LitterTray tray))
			{
				equipmentInUse = tray;
				agent.destination = equipmentInUse.InteractLocation.position;
				return Behaviour.Pooping;
			}
		}
		else
		{
			// Poop on floor
			return Behaviour.PoopingInPlace;
		}


		switch (sleep)
		{
			case float s when s <= 0.1f:
				float sleepCheck = Mathf.Lerp(1f, 0.8f, bladder * 10f);
				if (Random.value >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out Bed bed))
				{
					equipmentInUse = bed;
					agent.destination = equipmentInUse.InteractLocation.position;
					return Behaviour.Sleeping;
				}
				else
				{
					return Behaviour.SleepingInPlace;
				}
			case float s when s <= 0.8f:
				sleepCheck = Mathf.Lerp(1f, 0.1f, (bladder - 0.1f) * 1.43f);
				if (Random.value > sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out bed))
				{
					equipmentInUse = bed;
					agent.destination = equipmentInUse.InteractLocation.position;
					return Behaviour.Sleeping;
				}
				break;
		}

		float entertainmentRoll = Mathf.Lerp(0.1f, 1f, entertainment);
		if (Random.value < entertainmentRoll && GameManager.Instance.TryGetToy(out Toy toy))
		{
			equipmentInUse = toy;
			agent.destination = equipmentInUse.InteractLocation.position;
			return Behaviour.Playing;
		}

		// No behaviours chosen, choose a new spot to nav to
		FindNewWanderLocation();
		return Behaviour.Idle;
	}

	void FindNewWanderLocation()
	{
		// Generate random point
		Vector3 newDest = Random.insideUnitCircle * wanderRadius;
		// Offset point
		newDest = new Vector3(transform.position.x + newDest.x, transform.position.y, transform.position.z + newDest.y);

		if (NavMesh.SamplePosition(newDest, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
		{
			agent.destination = hit.position;

			spriteRenderer.flipX = transform.position.x > hit.position.x;

			// Reset timer
			idleTimer = Random.Range(timeBetweenMovements.x, timeBetweenMovements.y);
		}
	}

	void DecrementNeeds()
	{
		float entertainmentModifier = Mathf.Lerp(1.5f, 1f, entertainment * 3.3f);

		food -= foodDecayRate * entertainmentModifier * GameManager.Instance.GlobalFoodDecayModifier * Time.deltaTime;
		attention -= attentionDecayRate * entertainmentModifier * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
		sleep -= sleepDecayRate * entertainmentModifier * GameManager.Instance.GlobalSleepDecayModifier * Time.deltaTime;
		bladder -= bladderDecayRate * entertainmentModifier * GameManager.Instance.GlobalBladderDecayModifier * Time.deltaTime;
		grooming -= groomingDecayRate * entertainmentModifier * GameManager.Instance.GlobalGroomingDecayModifier * Time.deltaTime;
		entertainment -= entertainmentDecayRate * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
	}
}
