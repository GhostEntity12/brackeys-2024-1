using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
	enum Behaviours { Idle, Eating, Playing, Sleeping, SleepingInPlace, Pooping, PoopingInPlace }

	private NavMeshAgent agent;
	private SpriteRenderer spriteRenderer;
	private SpritesheetAnimator spritesheetAnimator;
	private Animator animator;

	private Behaviours behaviour = Behaviours.Idle;
	float baseValue;

	[SerializeField] private float wanderRadius = 3;

	private float timer;
	private float petTimer;
	private float groomTimer;
	private bool usingEquipment;
	IEquipment equipmentInUse = null;

	[field: SerializeField] public AnimalInfo AnimalInfo { get; private set; }

	public float Food
	{
		get
		{
			return AnimalInfo.food;
		}
		set
		{
			AnimalInfo.food = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Entertainment
	{
		get
		{
			return AnimalInfo.entertainment;
		}
		set
		{
			AnimalInfo.entertainment = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Attention
	{
		get
		{
			return AnimalInfo.attention;
		}
		set
		{
			AnimalInfo.attention = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Sleep
	{
		get
		{
			return AnimalInfo.sleep;
		}
		set
		{
			AnimalInfo.sleep = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Bladder
	{
		get
		{
			return AnimalInfo.bladder;
		}
		set
		{
			AnimalInfo.bladder = Mathf.Clamp(value, 0, 1);
		}
	}
	public float Grooming
	{
		get
		{
			return AnimalInfo.grooming;
		}
		set
		{
			AnimalInfo.grooming = Mathf.Clamp(value, 0, 1);
		}
	}
	public bool CanBePet => petTimer <= 0;
	public bool CanBeGroomed => groomTimer <= 0;
	public (float food, float entertainment, float attention, float sleep, float bladder, float grooming) Stats =>
		(Food, Entertainment, Attention, Sleep, Bladder, Grooming);
	private Behaviours Behaviour
	{
		get { return behaviour; }
		set
		{
			//Debug.Log($"Behaviour changing to {value}");
			behaviour = value;
		}
	}


	// Start is called before the first frame update
	void Start()
	{
		// Get components
		agent = GetComponent<NavMeshAgent>();
		Transform spriteTransform = transform.GetChild(0);
		spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
		spritesheetAnimator = spriteTransform.GetComponent<SpritesheetAnimator>();
		animator = spriteTransform.GetComponent<Animator>();
		timer = Random.Range(AnimalInfo.timeBetweenMovements.x, AnimalInfo.timeBetweenMovements.y);

		name = AnimalInfo.name;
		spritesheetAnimator.SetSpritesheet(AnimalInfo.spriteAtlasName);
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
			//case Behaviours.SleepingInPlace:
			//	break;
			//case Behaviours.PoopingInPlace:
			//	break;
			//default:
			//	break;
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
			case <= 0.1f:
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
			case <= 0.8f:
				sleepCheck = 1.13f - (1.3f * Sleep);
				//Debug.Log($"Sleep Check (<0.8): <color={(roll < sleepCheck ? "red" : "green")}>{roll}/{sleepCheck}</color>");
				if (roll >= sleepCheck) break;

				if (GameManager.Instance.TryGetBed(out bed))
				{
					//Debug.Log($"Sleeping in bed {bed.name}");
					equipmentInUse = bed;
					agent.SetDestination(equipmentInUse.InteractLocation.position);
					return Behaviours.Sleeping;
				}
				break;
		}

		float entertainmentCheck = 1 - (0.9f * AnimalInfo.entertainment);
		//Debug.Log($"Entertainment Check: <color={(roll < entertainmentCheck ? "red" : "green")}>{roll}/{entertainmentCheck}</color>");
		if (roll < entertainmentCheck && GameManager.Instance.TryGetToy(out Toy toy))
		{
			equipmentInUse = toy;
			agent.SetDestination(equipmentInUse.InteractLocation.position);
			return Behaviours.Playing;
		}

		//Debug.Log("No matches, wandering");
		// No behaviours chosen, choose a new spot to nav to
		FindNewWanderLocation();
		return Behaviours.Idle;
	}
	
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
			//Debug.Log($"Reached LitterTray {lt.name}");
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
			//Debug.Log($"Reached FoodBowl {fb.name}");
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
			//Debug.Log($"Reached Toy {t.name}");
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
		//Debug.Log($"Ending play roll: {stopPlayRoll}/{Entertainment - 0.3f}");
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
		//Debug.Log($"Ending sleep roll: {stopSleepRoll}/{Sleep - 0.1f}");
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
		AnimalInfo.attention += 0.3f;
		petTimer = 10f;
	}

	public void Groom()
	{
		AnimalInfo.grooming += 0.5f;
		groomTimer = 30f;
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
			timer = Random.Range(AnimalInfo.timeBetweenMovements.x, AnimalInfo.timeBetweenMovements.y);
		}
	}

	void DecrementNeeds()
	{
		float entertainmentModifier = Mathf.Lerp(1.5f, 1f, Entertainment * 3.3f);

		Food -= 0.001f * AnimalInfo.foodMod * entertainmentModifier * GameManager.Instance.GlobalFoodDecayModifier * Time.deltaTime;
		Attention -= 0.01f * AnimalInfo.attentionMod * entertainmentModifier * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
		Sleep -= 0.001f * AnimalInfo.sleepMod * entertainmentModifier * GameManager.Instance.GlobalSleepDecayModifier * Time.deltaTime;
		Bladder -= 0.001f * AnimalInfo.bladderMod * entertainmentModifier * GameManager.Instance.GlobalBladderDecayModifier * Time.deltaTime;
		Grooming -= 0.01f * AnimalInfo.groomingMod * entertainmentModifier * GameManager.Instance.GlobalGroomingDecayModifier * Time.deltaTime;
		Entertainment -= 0.001f * AnimalInfo.entertainmentMod * GameManager.Instance.GlobalEntertainmentDecayModifier * Time.deltaTime;
	}

	public void SetInfo(AnimalInfo info) => AnimalInfo = info;
}
