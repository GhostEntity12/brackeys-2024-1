using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
	enum Behaviours { Idle, Eating, Playing, Sleeping, SleepingInPlace, Pooping, PoopingInPlace }

	private NavMeshAgent agent;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private Behaviours behaviour = Behaviours.Idle;
	private AnimalInfo.Attributes attributes;
	float baseValue;

	// Needs
	private float foodDecayRate = 0.001f;
	private float entertainmentDecayRate = 0.001f;
	private float attentionDecayRate = 0.001f;
	private float sleepDecayRate = 0.001f;
	private float bladderDecayRate = 0.001f;
	private float groomingDecayRate = 0.001f;
	
	private float food;
	private float entertainment;
	private float attention;
	private float sleep;
	private float bladder;
	private float grooming;

	[SerializeField] private Vector2 timeBetweenMovements = new(2, 8);
	[SerializeField] private float wanderRadius = 3;

	private float timer;
	private float petTimer;
	private float groomTimer;
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
	public bool CanBePet => petTimer <= 0;
	public bool CanBeGroomed => groomTimer <= 0;

	private Behaviours Behaviour
	{
		get { return behaviour; }
		set
		{
			Debug.Log($"Behaviour changing to {value}");
			behaviour = value;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		// Get components
		agent = GetComponent<NavMeshAgent>();
		spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		animator = transform.GetChild(0).GetComponent<Animator>();

		// Flip V2 if values are in wrong order
		if (timeBetweenMovements.x > timeBetweenMovements.y)
		{
			(timeBetweenMovements.y, timeBetweenMovements.x) = (timeBetweenMovements.x, timeBetweenMovements.y);
		}
		timer = Random.Range(timeBetweenMovements.x, timeBetweenMovements.y);

		// Initialize needs (between 0.25 and 0.75)
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
		// State machine
		switch (Behaviour)
		{
			case Behaviours.Idle:
				ActionIdle();
				break;
			case Behaviours.Eating:
				ActionEat();
				break;
			case Behaviours.Playing:
				ActionPlay();
				break;
			case Behaviours.Sleeping:
				ActionSleep();
				break;
			case Behaviours.Pooping:
				ActionPoop();
				break;
			case Behaviours.SleepingInPlace:
				break;
			case Behaviours.PoopingInPlace:
				break;
			default:
				break;
		}
		// Manage needs
		DecrementNeeds();
		// Set sprite variables
		animator.SetFloat("MoveSpeed", agent.velocity.magnitude);
		spriteRenderer.flipX = transform.position.x > agent.destination.x;
		if (petTimer > 0) 
			petTimer -= Time.deltaTime;
		if (groomTimer > 0) 
			groomTimer -= Time.deltaTime;
	}

	public (float food, float entertainment, float attention, float sleep, float bladder, float grooming) GetStats()
		=> (Food, Entertainment, Attention, Sleep, Bladder, Grooming);

	private void ActionIdle()
	{
		// If not at destination, return
		if (Vector3.Distance(agent.destination, transform.position) > 0.1f) return;
		
			// Animal is at destination and is idling.
		// Decrease idle timer. Return while above 0
		timer -= Time.deltaTime;
		if (timer > 0) return;

		// When the idle timer reaches 0, choose what to do.
		Behaviour = ChooseBehaviour();
	}

	private void ActionPoop()
	{
		// Don't do anything until the animal has reached it's destination
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// On reach destination
		// Cache the equipment in use
		LitterTray lt = equipmentInUse as LitterTray;
		if (!usingEquipment)
		{
			Debug.Log($"Reached LitterTray {lt.name}");
			usingEquipment = true;
			animator.SetBool("Pooping", true);
			timer = lt.UseTime;
			return;
		}

		// Equipment being used. Wait UseTime seconds
		timer -= Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished
		// Improve need
		Bladder += lt.Use();

		animator.SetBool("Pooping", false);
		BeginIdle();
	}

	private void ActionEat()
	{
		// Don't do anything until the animal has reached it's destination
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// On reach destination
		// Cache equipment
		FoodBowl fb = equipmentInUse as FoodBowl;
		if (!usingEquipment)
		{
			Debug.Log($"Reached FoodBowl {fb.name}");
			usingEquipment = true;
			animator.SetBool("Eating", true);
			timer = fb.UseTime;
			return;
		}

		// Equipment being used. Wait UseTime seconds
		timer -= Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished
		// Improve need
		Food += fb.Use();
		// If no more uses or would overfill, leave
		if (!fb.Usable || Food + fb.FoodReward > 1)
		{
			usingEquipment = false;
			animator.SetBool("Eating", false);
			BeginIdle();
		}
		else
		{
			// Eat again
			timer = fb.UseTime;
		}
	}

	private void ActionPlay()
	{
		// Don't do anything until the animal has reached it's destination
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// On reach destination
		// Cache equipment
		Toy t = equipmentInUse as Toy;
		if (!usingEquipment)
		{
			Debug.Log($"Reached Toy {t.name}");
			usingEquipment = true;
			animator.SetBool("Playing", true);
			timer = t.UseTime;
			return;
		}

		// Equipment being used. Wait UseTime seconds
		timer -= Time.deltaTime;
		Entertainment += t.EntertainmentReward * Time.deltaTime;
		if (timer > 0) return;

		// On wait time finished, roll to stop playing
		float stopPlayRoll = Random.value;
		Debug.Log($"Ending play roll: {stopPlayRoll}/{Entertainment - 0.3f}");
		if (stopPlayRoll < Entertainment - 0.3f)
		{
			usingEquipment = false;
			animator.SetBool("Playing", false);
			BeginIdle();
			Debug.Log("Ending play");
			return;
		}
		timer = t.UseTime;
	}

	private void ActionSleep()
	{
		if (Vector3.Distance(transform.position, agent.destination) >= 0.1f) return;

		// Cache equipment
		Bed b = equipmentInUse as Bed;

		// On reach destination
		if (!usingEquipment)
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
		float stopSleepRoll = Random.value;
		Debug.Log($"Ending sleep roll: {stopSleepRoll}/{Sleep - 0.1f}");
		if (stopSleepRoll < Sleep - 0.1f)
		{
			usingEquipment = false;
			animator.SetBool("Sleeping", false);
			BeginIdle();
			return;
		}
		timer = b.UseTime;
	}

	private void BeginIdle()
	{
		if (equipmentInUse != null)
		{
			GameManager.Instance.ReturnEquipment(equipmentInUse);
			equipmentInUse = null;
		}
		FindNewWanderLocation();
		Behaviour = Behaviours.Idle;
	}

	public void Pet()
	{
		attention += 0.3f;
		petTimer = 10f;
	}

	public void Groom()
	{
		grooming += 0.5f;
		groomTimer = 30f;
	}

	private Behaviours ChooseBehaviour()
	{
		// Using one roll for the whole check (could be split into multiple)
		float roll = Random.value;
		// Piecewise function to determine chance of attempt
		float foodCheck = Food < 0.5f ? 1f - (1.4f * Food) : 0;
		//Debug.Log($"Food Check: <color={(roll < foodCheck ? "red" : "green")}>{roll}/{foodCheck}</color>");
		if (roll < foodCheck && GameManager.Instance.TryGetFoodBowl(out FoodBowl bowl))
		{	
			equipmentInUse = bowl;
			agent.SetDestination(equipmentInUse.InteractLocation.position);
			return Behaviours.Eating;
		}

		if (Bladder > 0)
		{
			// Piecewise function to determine chance of attempt
			float bladderCheck = Bladder < 0.6f ? 1 - (1.5f * Bladder) : 0;
			//Debug.Log($"Bladder Check: <color={(roll < bladderCheck ? "red" : "green")}>{roll}/{bladderCheck}</color>");
			if (roll < bladderCheck && GameManager.Instance.TryGetLitterTray(out LitterTray tray))
			{
				Debug.Log($"{name} is attempting to poop");
				equipmentInUse = tray;
				agent.SetDestination(equipmentInUse.InteractLocation.position);
				return Behaviours.Pooping;
			}
		}
		else
		{
			Debug.Log("Bladder empty - pooping on floor");
			// Poop on floor
			return Behaviours.PoopingInPlace;
		}


		switch (Sleep)
		{
			case float and <= 0.1f:
				float sleepCheck = 1f - (2f * Sleep);
				//Debug.Log($"Sleep Check (<0.1): <color={(roll < sleepCheck ? "red" : "green")}>{roll}/{sleepCheck}</color>");
				if (roll >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out Bed bed))
				{
					Debug.Log($"Sleeping in bed {bed.name}");
					equipmentInUse = bed;
					agent.SetDestination(equipmentInUse.InteractLocation.position);
					return Behaviours.Sleeping;
				}
				else
				{
					Debug.Log("No beds, sleeping on floor");
					return Behaviours.SleepingInPlace;
				}
			case float and <= 0.8f:
				sleepCheck = 1.13f - (1.3f * Sleep);
				//Debug.Log($"Sleep Check (<0.8): <color={(roll < sleepCheck ? "red" : "green")}>{roll}/{sleepCheck}</color>");
				if (roll >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out bed))
				{
					Debug.Log($"Sleeping in bed {bed.name}");
					equipmentInUse = bed;
					agent.SetDestination(equipmentInUse.InteractLocation.position);
					return Behaviours.Sleeping;
				}
				break;
		}

		float entertainmentCheck = 1 - (0.9f * entertainment);
		//Debug.Log($"Entertainment Check: <color={(roll < entertainmentCheck ? "red" : "green")}>{roll}/{entertainmentCheck}</color>");
		if (roll < entertainmentCheck && GameManager.Instance.TryGetToy(out Toy toy))
		{
			equipmentInUse = toy;
			agent.SetDestination(equipmentInUse.InteractLocation.position);
			return Behaviours.Playing;
		}

		Debug.Log("No matches, wandering");
		// No behaviours chosen, choose a new spot to nav to
		FindNewWanderLocation();
		return Behaviours.Idle;
	}

	void FindNewWanderLocation()
	{
		// Generate random point
		Vector3 newDest = Random.insideUnitCircle * wanderRadius;
		// Offset point
		newDest = new Vector3(transform.position.x + newDest.x, transform.position.y, transform.position.z + newDest.y);

		if (NavMesh.SamplePosition(newDest, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
		{
			agent.SetDestination(hit.position);

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
