using UnityEngine;

public class FoodBowl : MonoBehaviour, IEquipment, IActionable, ILimitedUseable
{
	public int Uses { get; private set; } = 0;

	[field: SerializeField] public float FoodReward { get; private set; }
	[field: SerializeField] public float UseTime { get; private set; } = 1f;
	[field: SerializeField] public Transform InteractLocation { get; private set; }
	[field: SerializeField] public Sprite Icon { get; private set; }

	public bool Usable => Uses > 0;
	public bool IsActionable => Uses != maxUses;

	[SerializeField] SpriteRenderer food;
	[SerializeField] int maxUses = 3;

	private void Start()
	{
		food.enabled = Uses > 0;
	}

	public float Use()
	{
		Uses--;
		if (Uses == 0)
			food.enabled = false;
		return FoodReward;
	}

	public void Action()
	{
		Uses = maxUses;
		food.enabled = true;
		GameManager.Instance.ReturnEquipment(this);
	}
}
