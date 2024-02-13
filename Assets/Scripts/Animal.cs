using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
	enum Behaviour { Idle, Eating, Playing, Sleeping, SleepingInPlace, Pooping, PoopingInPlace }

	private NavMeshAgent agent;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private Behaviour behaviour = Behaviour.Idle;
	float baseValue;

	// Needs
	private float foodDecayRate = 0.001f;
	private float entertainmentDecayRate = 0.001f;
	private float attentionDecayRate = 0.001f;
	private float sleepDecayRate = 0.001f;
	private float bladderDecayRate = 0.001f;
	private float groomingDecayRate = 0.001f;

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

	private float timer;
	private bool usingEquipment;
	IEquipment equipmentInUse = null;

	public float Food
	{
		get
		{
			return food;
		}
		set
		{
			food = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Entertainment
	{
		get
		{
			return entertainment;
		}
		set
		{
			entertainment = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Attention
	{
		get
		{
			return attention;
		}
		set
		{
			attention = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Sleep
	{
		get
		{
			return sleep;
		}
		set
		{
			sleep = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Bladder
	{
		get
		{
			return bladder;
		}
		set
		{
			bladder = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Grooming
	{
		get
		{
			return grooming;
		}
		set
		{
			grooming = Mathf.Clamp(value, 0, 1);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		animator = transform.GetChild(0).GetComponent<Animator>();

		// Flip V2 if values are in wrong order
		if (timeBetweenMovements.x > timeBetweenMovements.y)
		{
			(timeBetweenMovements.y, timeBetweenMovements.x) = (timeBetweenMovements.x, timeBetweenMovements.y);
		}
		timer = Random.Range(timeBetweenMovements.x, timeBetweenMovements.y);

		// Initialize needs
		Food = 0.25f + (Random.value * 0.5f);
		Entertainment = 0.25f + (Random.value * 0.5f);
		Attention = 0.25f + (Random.value * 0.5f);
		Sleep = 0.25f + (Random.value * 0.5f);
		Bladder = 0.25f + (Random.value * 0.5f);
		Grooming = 0.25f + (Random.value * 0.5f);
	}

	// Update is called once per frame
	void Update()
	{
		switch (behaviour)
		{
			case Behaviour.Idle:
				ActionIdle();
				break;
			case Behaviour.Eating:
				ActionEat();
				break;
			case Behaviour.Playing:
				ActionPlay();
				break;
			case Behaviour.Sleeping:
				ActionSleep();
				break;
			case Behaviour.Pooping:
				ActionPoop();
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

	void ActionIdle()
	{
		// If not at destination, return
		if (Vector3.Distance(agent.destination, transform.position) > 0.1f) return;

		// Decrease idle timer. Return while above 0
		timer -= Time.deltaTime;
		if (timer > 0) return;

		// When the idle timer reaches 0, choose what to do.
		behaviour = ChooseBehaviour();
	}

	void ActionPoop()
	{
		// Walk to equipment
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// Cache equipment
		LitterTray lt = equipmentInUse as LitterTray;

		// On reach destination
		if (usingEquipment == false)
		{
			Debug.Log($"Reached LitterTray {lt.name}");
			usingEquipment = true;
			animator.SetBool("Pooping", true);
			timer = lt.UseTime;
			return;
		}

		timer -= Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished
		Bladder += lt.Use();
		animator.SetBool("Eating", false);
		BeginIdle();
	}

	void ActionEat()
	{
		// Walk to equipment
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// Cache equipment
		FoodBowl fb = equipmentInUse as FoodBowl;

		// On reach destination
		if (usingEquipment == false)
		{
			Debug.Log($"Reached FoodBowl {fb.name}");
			usingEquipment = true;
			animator.SetBool("Eating", true);
			timer = fb.UseTime;
			return;
		}

		timer -= Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished
		Food += fb.Use();
		// If no more uses or would overfill, leave
		if (!fb.Usable || Food + fb.FoodReward > 1)
		{
			usingEquipment = false;
			animator.SetBool("Eating", false);
			BeginIdle();
			return;
		}
		timer = fb.UseTime;
	}

	void ActionPlay()
	{
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// Cache equipment
		Toy t = equipmentInUse as Toy;

		// On reach destination
		if (usingEquipment == false)
		{
			Debug.Log($"Reached Toy {t.name}");
			usingEquipment = true;
			animator.SetBool("Playing", true);
			timer = t.UseTime;
			return;
		}

		timer -= Time.deltaTime;
		Entertainment += t.EntertainmentReward * Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished, roll to stop playing
		float endPlayCheck = Random.value;
		float endPlayThreshold = 0.9f - Entertainment;
		Debug.Log($"{endPlayCheck}/{endPlayThreshold}");
		if (endPlayCheck > endPlayThreshold)
		{
			usingEquipment = false;
			animator.SetBool("Playing", false);
			BeginIdle();
			return;
		}
		timer = t.UseTime;
	}

	void ActionSleep()
	{
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// Cache equipment
		Bed b = equipmentInUse as Bed;

		// On reach destination
		if (usingEquipment == false)
		{
			usingEquipment = true;
			animator.SetBool("Sleeping", true);
			timer = b.UseTime;
			return;
		}

		timer -= Time.deltaTime;
		Sleep += b.SleepReward * Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished, roll to stop sleeping
		if (Random.value < Entertainment - 0.1f)
		{
			usingEquipment = false;
			animator.SetBool("Sleeping", false);
			BeginIdle();
			return;
		}
		timer = b.UseTime;
	}

	void BeginIdle()
	{
		if (equipmentInUse != null)
		{
			GameManager.Instance.ReturnEquipment(equipmentInUse);
			equipmentInUse = null;
		}
		behaviour = Behaviour.Idle;
	}

	Behaviour ChooseBehaviour()
	{
		float foodCheck = Food > 0.5f ? 0 : Mathf.Lerp(1f, 0.3f, Food * 2);
		float roll = Random.value;
		Debug.Log($"Food Check: <color={(roll < foodCheck ? "red" : "green")}>{roll}/{foodCheck}</color>");
		if (roll < foodCheck && GameManager.Instance.TryGetFoodBowl(out FoodBowl bowl))
		{
			agent.destination = equipmentInUse.InteractLocation.position;
			equipmentInUse = bowl;
			Debug.Log($"{name} is attempting to eat");
			return Behaviour.Eating;
		}

		if (Bladder > 0)
		{
			float bladderCheck = Bladder > 0.6f ? 0 : Mathf.Lerp(1f, 0.2f, Bladder * 1.67f);
			roll = Random.value;
			Debug.Log($"Bladder Check: <color={(roll < bladderCheck ? "red" : "green")}>{roll}/{bladderCheck}</color>");
			if (roll < bladderCheck && GameManager.Instance.TryGetLitterTray(out LitterTray tray))
			{
				Debug.Log($"{name} is attempting to poop");
				equipmentInUse = tray;
				agent.destination = equipmentInUse.InteractLocation.position;
				return Behaviour.Pooping;
			}
		}
		else
		{
			Debug.Log("Bladder empty - pooping on floor");
			// Poop on floor
			return Behaviour.PoopingInPlace;
		}


		switch (Sleep)
		{
			case float s when s <= 0.1f:
				float sleepCheck = Mathf.Lerp(1f, 0.8f, Bladder * 10f);
				roll = Random.value;
				Debug.Log($"Sleep Check (<0.1): <color={(roll < sleepCheck ? "red" : "green")}>{roll}/{sleepCheck}</color>");
				if (roll >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out Bed bed))
				{
					Debug.Log($"Sleeping in bed {bed.name}");
					equipmentInUse = bed;
					agent.destination = equipmentInUse.InteractLocation.position;
					return Behaviour.Sleeping;
				}
				else
				{
					Debug.Log("No beds, sleeping on floor");
					return Behaviour.SleepingInPlace;
				}
			case float s when s <= 0.8f:
				sleepCheck = Mathf.Lerp(1f, 0.1f, (Bladder - 0.1f) * 1.43f);
				roll = Random.value;
				Debug.Log($"Sleep Check (<0.8): <color={(roll < sleepCheck ? "red" : "green")}>{roll}/{sleepCheck}</color>");
				if (roll >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out bed))
				{
					Debug.Log($"Sleeping in bed {bed.name}");
					equipmentInUse = bed;
					agent.destination = equipmentInUse.InteractLocation.position;
					return Behaviour.Sleeping;
				}
				break;
		}

		float entertainmentCheck = Mathf.Lerp(0.1f, 1f, Entertainment);
		roll = Random.value;
		Debug.Log($"Entertainment Check: <color={(roll < entertainmentCheck ? "red" : "green")}>{roll}/{entertainmentCheck}</color>");
		if (roll < entertainmentCheck && GameManager.Instance.TryGetToy(out Toy toy))
		{
			equipmentInUse = toy;
			agent.destination = equipmentInUse.InteractLocation.position;
			return Behaviour.Playing;
		}

		Debug.Log("No matches, wandering");
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
			timer = Random.Range(timeBetweenMovements.x, timeBetweenMovements.y);
		}
	}

	void DecrementNeeds()
	{
		float entertainmentModifier = Mathf.Lerp(1.5f, 1f, Entertainment * 3.3f);

		Food -= foodDecayRate * entertainmentModifier * GameManager.Instance.GlobalFoodDecayModifier * Time.deltaTime;
		Attention -= attentionDecayRate * entertainmentModifier * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
		Sleep -= sleepDecayRate * entertainmentModifier * GameManager.Instance.GlobalSleepDecayModifier * Time.deltaTime;
		Bladder -= bladderDecayRate * entertainmentModifier * GameManager.Instance.GlobalBladderDecayModifier * Time.deltaTime;
		Grooming -= groomingDecayRate * entertainmentModifier * GameManager.Instance.GlobalGroomingDecayModifier * Time.deltaTime;
		Entertainment -= entertainmentDecayRate * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
	}
}
